using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NotBooksCore;

namespace mss_core
{
    public class Paragraph
    {
        public Paragraph()
        {
            Init();
        }
        public Paragraph(XElement xPara)
        {
            Init();
            ParseFromXElement(xPara);
        }
        public void Init()
        {
            Heading = false;
            Score = 0;
            Number = 0;
            Sequence = 0;
            Text = string.Empty;
        }

        private string[] _words;
        public string Text { get; set; }
        public int Number { get; private set; }
        public int Sequence { get; private set; }
        public int Score { get; set; }
        public string Notes { get; set; }
        public int WordCount
        {
            get
            {
                int count = 0;
                GetWords();
                count = _words == null ? 0 :_words.Count();
                return count;
            }
        }
        private Hashtable _htWT;
        public Hashtable WordTable
        {
            get
            {
                if (_htWT == null) RefreshWordTable();

                return _htWT;
            }
        }
        public bool Heading { get; set; }

        public XElement ToXElement()
        {
            return new XElement("paragraph",
                new XAttribute("number", Number),
                new XAttribute("score", Score),
                new XAttribute("heading", Heading),
                Text,
                new XElement("notes", (string.IsNullOrEmpty(Notes) ? null : Notes))
                );
        }

        private void ParseFromXElement(XElement xPara)
        {
            Text = xPara.Value;
            Number = !XmlUtil.XENull(xPara, "number", true) ? Convert.ToInt32(xPara.Attribute("number").Value) : 0;
            Score = !XmlUtil.XENull(xPara, "score", true) ? Convert.ToInt32(xPara.Attribute("score").Value) : 0;
            Notes = !XmlUtil.XENull(xPara, "notes") ? xPara.Element("notes").Value : null;
            Heading = !XmlUtil.XENull(xPara, "heading", true) ? Convert.ToBoolean(xPara.Attribute("heading").Value) : false;
        }

        public void SetNumber(int n)
        {
            Number = n;
        }
        public void SetSequence(int? s)
        {
            Sequence = s.HasValue ? s.Value : 0;
        }
        private void GetWords()
        {
            if (Text.Length > 0) _words = Text.Split(new char[] { ' ' });
        }
        public void RefreshWordTable()
        {
            _htWT = new Hashtable();

            GetWords();

            if (_words != null)
            {
                for (int i = 0; i < _words.Length; i++)
                {
                    string word = _words[i];

                    if (_htWT.ContainsKey(word)) _htWT[word] = Convert.ToInt32(_htWT[word]) + 1;
                    else _htWT.Add(word, 1);
                }
            }
        }
    }
}
