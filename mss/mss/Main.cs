using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using mss_core;

namespace mss
{
    public partial class Main : Form
    {
        #region properties

        internal int _counter;
        internal int _hcounter;
        internal int _sequence;
        internal Paragraph _prev;
        internal Paragraph _main;
        internal Paragraph _next;
        internal bool _forward = true;
        internal bool _mainChanged;
        internal enum StepMode
        {
            Normal,
            Danger,
            Incomplete,
            Shuffle
        }
        internal StepMode _stepmode;
        internal enum GetPara
        {
            Previous = -1,
            Main = 0,
            Next = 1
        }
        
        #endregion

        #region setup

        public Main()
        {
            InitializeComponent();

            _mainChanged = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.LocationChanged += new System.EventHandler(this.Main_LocationChanged);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Properties.Settings.Default.Location;
            this.WindowState = Properties.Settings.Default.State;
            if (this.WindowState == FormWindowState.Normal) this.Size = Properties.Settings.Default.Size;

            Setup();
        }
        private void Setup()
        {
            ClearWorkSpace();

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent1))
            {
                AddRecentItem(Properties.Settings.Default.Recent1);
                Global.CurrentFile = Properties.Settings.Default.Recent1;
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent2))
                AddRecentItem(Properties.Settings.Default.Recent2);

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent3))
                AddRecentItem(Properties.Settings.Default.Recent3);

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent4))
                AddRecentItem(Properties.Settings.Default.Recent4);

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Recent5))
                AddRecentItem(Properties.Settings.Default.Recent5);

            if (Properties.Settings.Default.OpenLast && !string.IsNullOrEmpty(Global.CurrentFile))
            {
                Global.LoadCurrentFile();
                LoadMainArea();
            }
            else
            {
                Global.Working = new Manuscript();
                Global.Working.Author = "No Author";
                Global.Working.Title = "Untitled Manuscript";

                SetMaxScore();

                btnPrev.Enabled = false;
                btnNext.Enabled = false;
            }
        }
        private void ClearWorkSpace()
        {
            _counter = 0;
            _hcounter = -1;
            _sequence = 1;
            _prev = new Paragraph();
            _main = new Paragraph();
            _next = new Paragraph();
            _stepmode = (StepMode)Properties.Settings.Default.smode;
            _forward = !(_stepmode == StepMode.Shuffle);

            switch (_stepmode)
            {
                case StepMode.Danger:
                    setDangerButton();
                    break;
                case StepMode.Incomplete:
                    setIncompleteButton();
                    break;
                case StepMode.Normal:
                default:
                    setNormalButton();
                    break;
            }
        }
        private void LoadMainArea()
        {
            try
            {
                ClearWorkSpace();
                tstTitle.Text = Global.Working.Title;
                tstAuthor.Text = Global.Working.Author;
                SetMaxScore();
                if (Global.Working.CurrentBookMark > 1)
                {
                    _sequence = Global.Working.CurrentBookMark;
                    _counter = Global.Working.Body.SingleOrDefault(p => p.Sequence == _sequence).Number;
                }
                SetWorkArea();
            }
            catch { }
        }
        private void SetMaxScore()
        {
            tscParaScore.Items.Clear();

            for (int i = 0; i <= Global.Working.ScoreMax; i++)
            {
                tscParaScore.Items.Add(i.ToString());
            }

            tscParaScore.SelectedItem = tscParaScore.Items[0];
        }
        private void AddRecentItem(string recentfile)
        {
            ToolStripItem tsiRecent = new ToolStripMenuItem("..." + recentfile.Substring(recentfile.Length-20));
            tsiRecent.Tag = recentfile;
            tsiRecent.Click += new EventHandler(tsiRecent_Click);

            tshFile.DropDownItems.Add(tsiRecent);
        }
        private void tsiRecent_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsi = (ToolStripMenuItem)sender;
            Global.CurrentFile = tsi.Tag.ToString();
            Global.LoadCurrentFile();
            LoadMainArea();
        }
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_mainChanged)
            {
                DialogResult dr = MessageBox.Show("Current Manuscript Was Not Saved! Save Now?",
                    "Save Before Exit?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dr == DialogResult.Yes) Save(false);

                _mainChanged = false;
            }

            Properties.Settings.Default.Save();
        }
        private void Main_Resize(object sender, EventArgs e)
        {
            Properties.Settings.Default.State = this.WindowState;
            if (this.WindowState == FormWindowState.Normal)
                Properties.Settings.Default.Size = this.Size;
        }
        private void Main_LocationChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
                Properties.Settings.Default.Location = this.Location;
        }

        #endregion

        #region main interface

        private void SetWorkArea()
        {
            if (_forward) _counter = 1;
            if (Global.Working.Body.Count > 0 && _sequence > Global.Working.Body.Max(p=>p.Sequence))
                _sequence = Global.Working.Body.Max(p=>p.Sequence);
            if (_hcounter > Global.HeaderValue) _hcounter = Global.HeaderValue;

            _main = GetParagraph(Convert.ToInt32(GetPara.Main));
            txtMain.Text = _main ==null ? string.Empty :_main.Text;
            Global.Working.SetCurrentBookmark(_main.Sequence);
            if (Global.Working.DangerWords.Count > 0)
            {
                foreach (string word in Global.Working.DangerWords)
                {
                    HighlightPhrase(txtMain, " " + word + " ", Color.Red);
                }
            }

            if (_counter > 1)
            {
                _prev = GetParagraph(Convert.ToInt32(GetPara.Previous));
                txtPrev.Text = _prev.Text;
            }
            else
            {
                _prev = null;
                txtPrev.Text = String.Empty;
            }

            if (_counter < Global.MaxBody)
            {
                _next = GetParagraph(Convert.ToInt32(GetPara.Next));
                txtNext.Text = _next.Text;
            }
            else
            {
                _next = null;
                txtNext.Text = String.Empty;
            }

            btnPrev.Enabled = _prev != null;
            btnNext.Enabled = _next != null;

            tscParaScore.SelectedItem = _main == null ? "0" : _main.Score.ToString();

            SetInfoArea();

            if (Global.Working.CompletionPC > -1)
            {
                pgbCompletePC.Value = Global.Working.CompletionPC;
                sblCompletePC.Text = Global.Working.CompletionPC.ToString() + "% of " + Global.Working.Title + " complete.";
            }
        }
        private Paragraph GetParagraph(int modifier)
        {
            Paragraph rtnPara = Global.Working.Body.SingleOrDefault(p => p.Sequence == (_sequence + modifier));

            if (rtnPara != null)
            {
                if (modifier == 0)
                {
                    SetHeading(rtnPara);
                    _sequence = _forward ? ++_sequence : --_sequence;
                    _counter = rtnPara.Number;
                }
                else
                {
                    modifier = modifier > 0 ? ++modifier : --modifier;
                }

                rtnPara = GetParagraph(modifier);
            }

            return rtnPara;
        }
        private void HighlightPhrase(RichTextBox box, string phrase, Color color)
        {
            int pos = box.SelectionStart;
            string s = box.Text;
            for (int ix = 0; ; )
            {
                int jx = s.IndexOf(phrase, ix, StringComparison.CurrentCultureIgnoreCase);
                if (jx < 0) break;
                box.SelectionStart = jx;
                box.SelectionLength = phrase.Length;
                box.SelectionColor = color;
                ix = jx + 1;
            }
            box.SelectionStart = pos;
            box.SelectionLength = 0;
        }
        private void SetHeading(Paragraph para)
        {
            lblSection.Text = "Current Section: ";

            if (_forward)
            {
                _hcounter = para.Number;
                lblSection.Text += para.Text;
            }
            else
            {
                _hcounter = _hcounter < -1 ? (_hcounter + 1) : -1;
                lblSection.Text += Global.Working.Body.Single(p => p.Number == _hcounter).Text;
            }
        }
        private void SetInfoArea()
        {
            Global.Working.RefreshWordTable();

            flpDynCtls.Controls.Clear();

            string info = "No Info Yet.";

            if(_main != null)
            {
                info = "Current Paragraph: " + _main.Number.ToString() + " of " + Global.MaxBody.ToString();
                info += " | Word Count: " + _main.WordCount.ToString() + " of " + Global.Working.WordCount.ToString();
                info += "; Flagged Words: ";

                Label lblInfo = new Label();
                lblInfo.AutoSize = true;
                lblInfo.Text = info;

                flpDynCtls.Controls.Add(lblInfo);

                foreach (DictionaryEntry de in _main.WordTable)
                {
                    if (Global.Working.WordTable.ContainsKey(de.Key) && 
                        Convert.ToInt32(Global.Working.WordTable[de.Key]) > Global.Working.FlagThreshold)
                    {
                        string word = de.Key.ToString();

                        Label lblWord = new Label();
                        lblWord.Font = new Font("Tahoma", 12);
                        lblWord.Text = word + ":";
                        lblWord.AutoSize = true;
                        flpDynCtls.Controls.Add(lblWord);

                        GenerateWCtl(word, de.Value.ToString() + "/" + Global.Working.WordTable[de.Key].ToString(), Manuscript.WordType.Invisible);
                        GenerateWCtl(word, string.Empty, Manuscript.WordType.Danger);
                    }
                }
            }
        }
        private void GenerateWCtl(string word, string occurrences, Manuscript.WordType wt)
        {
            Button btnW = new Button();
            string strBtnText = !string.IsNullOrEmpty(occurrences) ? occurrences : "!";
            btnW.Text = strBtnText;
            btnW.AutoSize = true;
            btnW.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            List<object> listTag = new List<object>();
            listTag.Add(wt);
            listTag.Add(word);
            btnW.Tag = listTag;

            btnW.Click += new EventHandler(btnW_Click);

            toolTip1.SetToolTip(btnW, "Add To " + wt.ToString());

            flpDynCtls.Controls.Add(btnW);
        }
        private void btnW_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            List<object> listTag = (List<object>)btn.Tag;

            Manuscript.WordType wt = (Manuscript.WordType)listTag[0];
            string word = listTag[1].ToString();
            string msg = string.Format("Do you wish to add the word \"{0}\" to the " + wt.ToString() + " word list.", word);
            DialogResult rslt = MessageBox.Show(msg, "Add " + wt.ToString() + " Word?", MessageBoxButtons.YesNo);

            if (rslt == DialogResult.Yes)
            {
                switch (wt)
                {
                    case Manuscript.WordType.Danger:
                        AddToDanger(word);
                        break;
                    case Manuscript.WordType.Invisible:
                    default:
                        AddToInvisible(word);
                        break;
                }
            }
        }
        private void btnPrev_Click(object sender, EventArgs e)
        { 
            SaveMain();
            --_counter;
            --_sequence;
            _forward = false;
            SetWorkArea();
        }
        private void btnNext_Click(object sender, EventArgs e)
        {
            SaveMain();
            ++_counter;
            ++_sequence;
            _forward = true;
            SetWorkArea();
        }
        private void SaveMain()
        {
            Paragraph mainP = Global.Working.Body.Single(p => p.Number == _counter);
            
            mainP.Text = txtMain.Text;
            mainP.Score = Convert.ToInt32(tscParaScore.SelectedItem);
        }
        private void txtMain_TextChanged(object sender, EventArgs e)
        {
            _mainChanged = true;
        }

        #endregion

        #region cmi methods

        private void cmiSetHeading_Click(object sender, EventArgs e)
        {
            _main.Heading = true;
            SaveMain();
            RecalcHeaders(); 
        }
        private void RecalcHeaders()
        {
            Global.Working.RecalculateForHeaders();
            SetHeading(_main);
            ++_sequence;
            MessageBox.Show("Recalculation Complete");
            SetWorkArea();
        }
        private void cmiCut_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMain.SelectedText))
            {
                Clipboard.SetText(txtMain.SelectedText);
                txtMain.Text = txtMain.Text.Replace(txtMain.SelectedText, string.Empty);
            }
        }
        private void cmiCopy_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(txtMain.SelectedText))
            {
                Clipboard.SetText(txtMain.SelectedText);
            }
        }
        private void cmiPaste_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Clipboard.GetText()))
            {
                txtMain.Text = txtMain.Text.Insert(txtMain.SelectionStart, Clipboard.GetText());
            }
        }
        private void cmiDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete the current paragraph from the document. Proceed?", "Delete Paragraph", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Global.Working.Body.Remove(Global.Working.Body.Single(p => p.Number == _counter));
                Global.Working.Recalculate();
                SetWorkArea();
            }
        }
        private void cmiSplit_Click(object sender, EventArgs e)
        {
            if (txtMain.SelectionStart > 0)
            {
                string newpara = txtMain.Text.Substring(txtMain.SelectionStart).Trim();
                Global.Working.Body.Single(p => p.Number == _counter).Text = txtMain.Text.Remove(txtMain.SelectionStart).Trim();

                Paragraph para = new Paragraph();

                para.SetNumber(_next.Number);
                para.SetSequence(_next.Sequence);
                para.Text = newpara;

                Global.Working.RecalculateForInsert(_next.Sequence);

                int idx = Global.Working.Body.IndexOf(Global.Working.Body.Single(p => p.Number == _next.Number));

                Global.Working.Body.Insert(idx, para);

                SetWorkArea();
            }
        }
        private void cmiMerge_Click(object sender, EventArgs e)
        {
            string newpara = txtMain.Text.Trim() + " ";
            newpara += _next.Text;
            _main.Text = newpara;
            int nextseq = _next.Sequence;

            Global.Working.Body.RemoveAt(nextseq - 1);

            Global.Working.Recalculate();

            SetWorkArea();
        }
        private void cmiAddInvisible_Click(object sender, EventArgs e)
        {
            AddToInvisible(txtMain.SelectedText.Trim());
        }
        private void cmiAddDanger_Click(object sender, EventArgs e)
        {
            AddToDanger(txtMain.SelectedText.Trim());
        }
        private void AddToInvisible(string word)
        {
            Global.Working.InvisibleWords.Add(word);
            SetInfoArea();
        }
        private void AddToDanger(string word)
        {
            Global.Working.DangerWords.Add(word);
            HighlightPhrase(txtMain, " " + word + " ", Color.Red);
            SetInfoArea();
        }
        
        #endregion

        #region menubar methods

        #region file menu
        private void tsmNew_Click(object sender, EventArgs e)
        {
            Setup();
        }
        private void tsmOpen_Click(object sender, EventArgs e)
        {
            Global.OpenFile();
            LoadMainArea();
        }
        private void tsmSave_Click(object sender, EventArgs e)
        {
            Save(false);
        }
        private void tsmSaveAs_Click(object sender, EventArgs e)
        {
            Save(true);
        }
        private void Save(bool full)
        {
            if (!string.IsNullOrEmpty(tstAuthor.Text)) Global.Working.Author = tstAuthor.Text;
            if (!string.IsNullOrEmpty(tstTitle.Text)) Global.Working.Title = tstTitle.Text;
            SaveMain();
            Global.SaveFile(full);
            SetSaveLabel();
            _mainChanged = false;
        }
        #endregion

        #region tools menu 
        private void tsmViewIW_Click(object sender, EventArgs e)
        {
            Words frmWords = new Words();
            frmWords.ShowDialog();
            Recover();
        }
        private void tsmViewDW_Click(object sender, EventArgs e)
        {
            Words frmWords = new Words();
            frmWords.wordType = Manuscript.WordType.Danger;
            frmWords.ShowDialog();
            Recover();
        }
        private void tsmExportIW_Click(object sender, EventArgs e)
        {
            Global.SaveFile(true,Global.Working.WordsToXDocument(Manuscript.WordType.Invisible));
        }
        private void tsmImportIW_Click(object sender, EventArgs e)
        {
            Global.OpenFile(Manuscript.WordType.Invisible);
        }
        private void tsmExportDW_Click(object sender, EventArgs e)
        {
            Global.SaveFile(true, Global.Working.WordsToXDocument(Manuscript.WordType.Danger));
        }
        private void tsmImportDW_Click(object sender, EventArgs e)
        {
            Global.OpenFile(Manuscript.WordType.Danger);
        }
        private void tsmOptions_Click(object sender, EventArgs e)
        {
            Options frmOpt = new Options();
            frmOpt.ShowDialog();
            Recover();
            SetWorkArea();
        }
        private void Recover()
        {
            SetInfoArea();
            this.BringToFront();
        }
        private void tsmRecalculate_Click(object sender, EventArgs e)
        {
            RecalcHeaders();
        }
        #endregion
        
        #endregion
        
        #region toolstrip methods

        private void tsoNormalMode_Click(object sender, EventArgs e)
        {
            _stepmode = StepMode.Normal;
            setSMode();
            setNormalButton();
        }
        private void setNormalButton()
        {
            tssbStepMode.Image = Properties.Resources.map;
            tstStepNo.Enabled = true;
        }
        private void tsoDangerMode_Click(object sender, EventArgs e)
        {
            _stepmode = StepMode.Danger;
            setSMode();
            setDangerButton();
        }
        private void setDangerButton()
        {
            tssbStepMode.Image = Properties.Resources.flag_red;
            tstStepNo.Enabled = false;
        }
        private void tsoIncompleteMode_Click(object sender, EventArgs e)
        {
            _stepmode = StepMode.Incomplete;
            setSMode();
            setIncompleteButton();
            
        }
        private void setIncompleteButton()
        {
            tssbStepMode.Image = Properties.Resources.pencil;
            tstStepNo.Enabled = false;
            _counter = 1;
        }
        private void tsoShuffle_Click(object sender, EventArgs e)
        {
            _stepmode = StepMode.Shuffle;
            setSMode();
            setShuffleButton();
        }
        private void setShuffleButton()
        {
            tssbStepMode.Image = Properties.Resources.arrow_out;
            tstStepNo.Enabled = false;
            stepForward();
        }
        private void setSMode()
        {
            Properties.Settings.Default.smode = Convert.ToInt32(_stepmode);
            Properties.Settings.Default.Save();
        }
        private void tsbStart_Click(object sender, EventArgs e)
        {
            SaveMain();
            _sequence = 1;
            _counter = 1;
            _forward = true;
            SetWorkArea();
        }
        private void tsbStepForward_Click(object sender, EventArgs e)
        {
            stepForward();
        }
        private void stepForward()
        {
            SaveMain();
            _forward = true;
            Step();
            SetWorkArea();
        }
        private void tsbStepBack_Click(object sender, EventArgs e)
        {
            SaveMain();
            _forward = false;
            Step();
            SetWorkArea();
        }
        private void tsbEnd_Click(object sender, EventArgs e)
        {
            SaveMain();
            _counter = Global.MaxBody;
            _sequence = Global.Working.Body.Where(p => !p.Heading).Max(p => p.Sequence);
            _forward = true;
            SetWorkArea();
        }
        private void Step()
        {
            switch (_stepmode)
            {
                case StepMode.Danger:
                    if (Global.Working.DangerWords.Count > 0)
                    {
                        int _newsequence = 0;

                        foreach (string s in Global.Working.DangerWords)
                        {
                            if ((_newsequence = GetNextDanger(" " + s + " ")) > 0)
                            {
                                _sequence = _newsequence;
                                break;
                            }
                        }
                        if(_newsequence == 0) MessageBox.Show("Danger words not found");
                    }
                    else
                        MessageBox.Show("There are no danger words defined in this manuscript.\r\n\r\nPlease define some danger words to use this step mode or change the step mode.");
                    break;
                case StepMode.Incomplete:

                    if (Global.Working.Body.Select(p => p.Score < Global.Working.CompleteScore).Count() > 0)
                    {
                        int _newsequence;

                        if (_forward)
                        {
                            for (_newsequence = _sequence+1; _newsequence <= Global.Working.Body.Max(p=>p.Sequence); _newsequence++)
                            {
                                if (GetNextIncomplete(_newsequence) > 0)
                                {
                                    _sequence = _newsequence;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (_newsequence = _sequence-1; _newsequence > 0; _newsequence--)
                            {
                                if (GetNextIncomplete(_newsequence) > 0)
                                {
                                    _sequence = _newsequence;
                                    break;
                                }
                            }
                        }
                    }
                    else
                        MessageBox.Show("All paragraphs are above the completion score.");
                    
                    break;
                case StepMode.Shuffle:
                    Random _r = new Random();
                    _sequence = _r.Next(0,Global.MaxBody);
                    _counter = _sequence;
                    lblSection.Text = "Shuffle Mode!";
                    break;
                case StepMode.Normal:
                default:
                    bool _err = false;
                    if (!string.IsNullOrEmpty(tstStepNo.Text))
                    {
                        int _step = 0;

                        if (Int32.TryParse(tstStepNo.Text, out _step))
                            if (_forward)
                                _sequence = (_sequence + _step) > Global.Working.Body.Max(p => p.Sequence) ? 
                                    Global.Working.Body.Max(p => p.Sequence) : _sequence + _step;
                            else
                                _sequence = (_sequence - _step) < 1 ? 1 : _sequence - _step;
                        else
                            _err = true;
                    }
                    else _err = true;

                    if (_err)
                        MessageBox.Show("Please enter a number to use the step function.");
                    break;
            }
        }
        private int GetNextDanger(string s)
        {
            Paragraph para = null;

            if (_forward)
                para = Global.Working.Body.FirstOrDefault(p => (p.Sequence > _sequence)
                    && (p.Text.Contains(s))
                    && (!p.Heading));
            else
            {
                for (int _newsequence = _sequence - 1; _newsequence > 0; _newsequence--)
                {
                    para = Global.Working.Body.FirstOrDefault(p => (p.Sequence == _newsequence)
                        && (p.Text.Contains(s))
                        && (!p.Heading));

                    if (para != null) break;
                }
            }

            if (para != null) return para.Sequence;
            else return 0;
        }
        private int GetNextIncomplete(int _newsequence)
        {
            Paragraph para = Global.Working.Body.FirstOrDefault(p => (p.Sequence == _newsequence)
                && p.Score < Global.Working.CompleteScore
                && p.Number > 0);

            if (para != null) return para.Sequence;
            else return 0;
        }

        #endregion

        #region status bar

        private void tsiRemoveHeading_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will just unmark the heading. Proceed?", "Remove Heading", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Paragraph para = Global.Working.Body.SingleOrDefault(p => p.Number == _hcounter);

                if (para != null)
                {
                    para.Heading = false;
                    RecalcHeaders();
                }
            }
        }
        private void tsiDeleteHeading_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This will delete the heading from the document. Proceed?", "Delete Heading", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Global.Working.Body.Remove(Global.Working.Body.Single(p => p.Number == _hcounter));
                Global.Working.Recalculate();
                _counter--;
                _sequence--;
                SetWorkArea();
            }
        }

        private void SetSaveLabel()
        {
            lblSaveStatus.Text = "Last Saved at " + DateTime.Now.ToString("dd MMMM yyyy HH:mm");
        }

        #endregion

    }
}
