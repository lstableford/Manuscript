using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NotBooksCore;

namespace mss_core
{
    public class Manuscript
    {
        #region constructors and setup

        public Manuscript()
        {
            Init();
        }
        public Manuscript(string fulltext)
        {
            Init();
            ParseFromText(fulltext);
        }
        public Manuscript(XDocument xDocMss)
        {
            Init();
            ParseFromXElement(xDocMss.Descendants("manuscript").SingleOrDefault());
        }
        public void Init()
        {
            _body = new List<Paragraph>();
            InvisibleWords = new List<string>();
            DangerWords = new List<string>();
            ScoreMax = _defaultSM;
            _completeScore = ScoreMax;
            FlagThreshold = _defaultFT;
            FlagOn = FlagMode.Normal;
            _htWT = new Hashtable();
        }
        
        #endregion

        #region properties
        
        private int _defaultSM = 5;
        private int _defaultFT = 50;
        private List<Paragraph> _body;
        public string Title { get; set; }
        public string Author { get; set; }
        public List<Paragraph> Body {
            get { return _body;}
            set { _body = value;}
        }
        public int FlagThreshold { get; set; }
        public FlagMode FlagOn { get; set; }
        public int WordCount
        {
            get
            {
                int count = 0;
                if (Body.Count > 0)
                {
                    foreach (Paragraph p in Body)
                    {
                        count = count + p.WordCount;
                    }
                }
                return count;
            }
        }
        public int ScoreMax { get; set; }
        private int _completeScore;
        public int CompleteScore
        {
            get { return _completeScore; }
            set
            {
                _completeScore = value;
                if (value < 0) _completeScore = 0;
                if (value > ScoreMax) _completeScore = ScoreMax;
            }
        }
        public int CompletionPC
        {
            get
            {
                float paracount = Body.Count(p=>!p.Heading);
                float compparas = 0;
                
                foreach (Paragraph p in Body.Where(p=>!p.Heading))
                {
                    if (p.Score >= _completeScore) ++compparas;
                }

                float divided = Convert.ToSingle(compparas / paracount);

                return (int)(divided * 100);
            }
        }
        private Hashtable _htWT;
        public Hashtable WordTable
        {
            get
            {
                return _htWT;
            }
        }
        public List<string> InvisibleWords { get; set; }
        public List<string> DangerWords { get; set; }
        public enum WordType
        {
            Invisible,
            Danger
        }
        public enum FlagMode
        {
            Off,
            Normal,
            Top
        }
        public int CurrentBookMark { get; private set; }

        #endregion

        #region methods

        #region Data Out Methods

        public XDocument ToXDocument()
        {
           return new XDocument(ToXElement());
        }
        public XDocument WordsToXDocument(WordType wordType)
        {
            string xename = wordType == WordType.Invisible ? "invisiblewords" : "dangerwords";

            return new XDocument(new XElement(xename,GetWords(wordType)));
        }
        public XElement ToXElement()
        {
            XElement rtn = new XElement("manuscript",
                new XAttribute("scoremax", ScoreMax),
                new XAttribute("completescore", _completeScore),
                new XAttribute("flagthreshold", FlagThreshold),
                new XAttribute("currentbookmark", CurrentBookMark),
                new XAttribute("flagon", Convert.ToInt32(FlagOn)),
                new XElement("title", Title),
                new XElement("author", Author),
                CreateParagraphs()
                );

            if (InvisibleWords.Count > 0)
            {
                XElement xIW = new XElement("invisiblewords", GetWords(WordType.Invisible));
                rtn.Add(xIW);
            }

            if (DangerWords.Count > 0)
            {
                XElement xDW = new XElement("dangerwords", GetWords(WordType.Danger));
                rtn.Add(xDW);
            }

            return rtn;
        }
        public List<XElement> CreateParagraphs()
        {
            List<XElement> xRtn = new List<XElement>();

            foreach (Paragraph p in Body)
            {
                xRtn.Add(p.ToXElement());
            }

            return xRtn;
        }
        private List<XElement> GetWords(WordType type)
        {
            List<XElement> rtn = new List<XElement>();

            List<string> wordlist = new List<string>();

            switch (type)
            {
                case WordType.Danger:
                    wordlist = DangerWords;
                    break;
                case WordType.Invisible:
                default:
                    wordlist = InvisibleWords;
                    break;
            }

            foreach (string word in wordlist)
            {
                rtn.Add(new XElement("word", word));
            }

            return rtn;
        }
        public string ExportToText()
        {
            string _text = "";

            _text += Title + " by " + Author + "\r\n";

            for (int i = 1; i <= Body.Max(p => p.Sequence); i++)
            {
                Paragraph para = Body.Single(p => p.Sequence == i);
                if (para.Heading)
                    _text += "[" + System.Math.Abs(para.Number) + "]" + para.Text + "\r\n";
                else
                    _text += para.Text + "\r\n";
            }

            return _text;
        }
        public string[] ExportToTextArray()
        {
            return ExportToTextArray(true);
        }
        public string[] ExportToTextArray(bool spacer)
        {
            List<string> _lines = new List<string>();

            _lines.Add(Title);
            _lines.Add("by");
            _lines.Add(Author);

            for (int i = 1; i <= Body.Max(p => p.Sequence); i++)
            {
                if(spacer) _lines.Add(string.Empty);
                Paragraph para = Body.Single(p => p.Sequence == i);
                if (para.Heading)
                    _lines.Add("[" + System.Math.Abs(para.Number) + "] " + para.Text);
                else
                    _lines.Add(para.Text);
            }

            return _lines.ToArray();
        }

        #endregion

        #region Data In Methods
        
        private void ParseFromXElement(XElement xMss)
        {
            ScoreMax = !XmlUtil.XENull(xMss, "scoremax", true) ? Convert.ToInt32(xMss.Attribute("scoremax").Value) : _defaultSM;
            _completeScore = !XmlUtil.XENull(xMss, "completescore", true) ? Convert.ToInt32(xMss.Attribute("completescore").Value) : _defaultSM;
            FlagThreshold = !XmlUtil.XENull(xMss, "flagthreshold", true) ? Convert.ToInt32(xMss.Attribute("flagthreshold").Value) : _defaultFT;
            CurrentBookMark = !XmlUtil.XENull(xMss, "currentbookmark", true) ? Convert.ToInt32(xMss.Attribute("currentbookmark").Value) : 1;
            FlagOn = (FlagMode)(!XmlUtil.XENull(xMss, "flagon", true) ? Convert.ToInt32(xMss.Attribute("flagon").Value) : 1);
            Title = !XmlUtil.XENull(xMss, "title") ? xMss.Element("title").Value : string.Empty;
            Author = !XmlUtil.XENull(xMss, "author") ? xMss.Element("author").Value : string.Empty;
            LoadXBody(xMss);
            if (!XmlUtil.XENull(xMss, "invisiblewords")) LoadXWords(xMss.Element("invisiblewords"),WordType.Invisible);
            if (!XmlUtil.XENull(xMss, "dangerwords")) LoadXWords(xMss.Element("dangerwords"), WordType.Danger);
        }
        private void LoadXBody(XElement xMss)
        {
            var paras = xMss.Descendants("paragraph").ToList();

            foreach (XElement p in paras)
            {
                Body.Add(new Paragraph(p));
            }

            Recalculate();
        }
        public void LoadXWords(XElement xWords, WordType wordType)
        {
            foreach (XElement xel in xWords.Descendants("word"))
            {
                switch (wordType)
                {
                    case WordType.Danger:
                        DangerWords.Add(xel.Value);
                        break;
                    case WordType.Invisible:
                    default:
                        InvisibleWords.Add(xel.Value);
                        break;
                }
            }
        }
        private void ParseFromText(string text)
        {
            string[] paras = text.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < paras.Length; i++)
            {
                Paragraph para = new Paragraph();

                para.Text = paras[i];
                para.SetNumber(i + 1);
                para.SetSequence(i + 1);

                Body.Add(para);
            }
        }

        #endregion

        public void RefreshWordTable()
        {
            _htWT.Clear();

            foreach (Paragraph p in Body)
            {
                p.RefreshWordTable();

                foreach (DictionaryEntry de in p.WordTable)
                {
                    string word = de.Key.ToString();

                    if (!InvisibleWords.Contains(word))
                    {
                        int occurrences = Convert.ToInt32(de.Value);

                        if (_htWT.ContainsKey(word)) _htWT[word] = Convert.ToInt32(_htWT[word]) + occurrences;
                        else _htWT.Add(word, occurrences);
                    }
                }
            }
        }
        public void Recalculate()
        {
            int counter = 1;
            int hcounter = -1;

            for (int i = 0; i < Body.Count; i++)
            {
                Paragraph para = Body[i];

                if (para.Heading)
                {
                    para.SetNumber(hcounter);
                    hcounter--;
                }
                else
                {
                    para.SetNumber(counter);
                    ++counter;
                }
                para.SetSequence(i + 1);
            }
        }
        public void RecalculateForInsert(int startPara)
        {
            foreach (Paragraph p in Body)
            {
                if (p.Sequence >= startPara)
                {
                    if (!p.Heading) p.SetNumber(p.Number + 1);
                    p.SetSequence(p.Sequence + 1);
                }
            }
        }
        public void RecalculateForHeaders()
        {
            SetHeaders(0);
            SetHeaders(-1);
            Recalculate();
        }
        private void SetHeaders(int _hcounter)
        {
            for (int i = 0; i < Body.Count(p => p.Heading); i++)
            {
                Paragraph para = null;

                if (_hcounter == 0)
                    para = Body.FirstOrDefault(p => p.Number < 0
                        && p.Heading);
                else
                    para = Body.FirstOrDefault(p => p.Number == 0
                        && p.Heading);

                if (para == null) continue;
                else
                {
                    para.SetNumber(_hcounter);
                    if (_hcounter < 0) _hcounter--;
                }
            }
        }
        public void SetCurrentBookmark(int _bm)
        {
            CurrentBookMark = _bm;
        }
        #endregion

        
    }
}
