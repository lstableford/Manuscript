using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mss_core;

namespace mss
{
    public partial class Words : Form
    {
        public Manuscript.WordType wordType { get; set; }

        public Words()
        {
            InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            wordType = Manuscript.WordType.Invisible;
        }
        internal void Build()
        {
            flpWords.Controls.Clear();

            switch (wordType)
            {
                case Manuscript.WordType.Danger:
                    makeControls(Global.Working.DangerWords);
                    break;
                case Manuscript.WordType.Invisible:
                default:
                    makeControls(Global.Working.InvisibleWords);
                    break;
            }
        }

        private void makeControls(List<string> words)
        {
            words.Sort((x, y) => string.Compare(x, y));

            string _wt = wordType.ToString();

            foreach (string word in words)
            {
                Label lblWord = new Label();
                lblWord.Text = word;
                lblWord.AutoSize = true;
                lblWord.Font = new Font("Tahoma", 14);
                flpWords.Controls.Add(lblWord);
                Button btnDelete = new Button();
                btnDelete.AutoSize = true;
                btnDelete.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                btnDelete.Text = "[x]";
                btnDelete.Tag = word;
                toolTip1.SetToolTip(btnDelete, string.Format("Delete \"{0}\" From {1} Word List",word, _wt));
                btnDelete.Click += new EventHandler(btnDelete_Click);
                flpWords.Controls.Add(btnDelete);
                flpWords.SetFlowBreak(btnDelete, true);
            }

            Label lblSpacer = new Label();
            lblSpacer.Text = " ";
            flpWords.Controls.Add(lblSpacer);

            this.SetTopLevel(true);
        }

        void btnDelete_Click(object sender, EventArgs e)
        {
            Button clicker = (Button)sender;
            string word = clicker.Tag.ToString();
            string _wt = wordType.ToString();
            string msg = string.Format("Do you wish to remove the word {0} from the {1} Word List?", word, _wt);

            if (MessageBox.Show(msg, "Remove Word", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                switch (wordType)
                {
                    case Manuscript.WordType.Danger:
                        Global.Working.DangerWords.Remove(word);
                        break;
                    case Manuscript.WordType.Invisible:
                    default:
                        Global.Working.InvisibleWords.Remove(word);
                        break;
                }

                Build();
            }
        }

        

        private void Words_Shown(object sender, EventArgs e)
        {
            Build();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
