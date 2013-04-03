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
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();

            Setup();
        }

        private void Setup()
        {
            txtMaxScore.Text = Global.Working.ScoreMax.ToString();
            txtCompletionScore.Text = Global.Working.CompleteScore.ToString();
            txtFlagThreshold.Text = Global.Working.FlagThreshold.ToString();
            cmbFlagMode.DataSource = Enum.GetValues(typeof(Manuscript.FlagMode));
            cmbFlagMode.SelectedIndex = Convert.ToInt32(Global.Working.FlagOn);
            SetFlagThresholdDesc();

            chkOpenLast.Checked = Properties.Settings.Default.OpenLast;
            chkIncludeDanger.Checked = Properties.Settings.Default.FlagDanger;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMaxScore.Text))
            {
                int maxscore = 0;

                Int32.TryParse(txtMaxScore.Text, out maxscore);

                if (maxscore > 0)
                    Global.Working.ScoreMax = maxscore;
                else
                {
                    MessageBox.Show("To assign the Maximum Paragraph Score please enter a valid positive integer.");
                    txtMaxScore.Text = string.Empty;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(txtCompletionScore.Text))
            {
                int completionscore = 0;

                Int32.TryParse(txtCompletionScore.Text, out completionscore);

                if (completionscore > 0 && completionscore <= Global.Working.ScoreMax)
                    Global.Working.CompleteScore = completionscore;
                else
                {
                    MessageBox.Show("The completion score must be above zero and less than or equal to the Maximum Score for the manuscript.");
                    txtCompletionScore.Text = string.Empty;
                    return;
                }
            }

            if (!string.IsNullOrEmpty(txtFlagThreshold.Text))
            {
                int threshold = 0;

                Int32.TryParse(txtFlagThreshold.Text, out threshold);

                if (threshold > 0)
                    Global.Working.FlagThreshold = threshold;
                else
                {
                    MessageBox.Show("To assign the flag threshold please enter a valid positive integer.");
                    txtFlagThreshold.Text = string.Empty;
                    return;
                }
            }
            Global.Working.FlagOn = (Manuscript.FlagMode)cmbFlagMode.SelectedValue;

            Global.SaveFile();

            Properties.Settings.Default.OpenLast = chkOpenLast.Checked;
            Properties.Settings.Default.FlagDanger = chkIncludeDanger.Checked;
            Global.SaveSettings();

            this.Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cmbFlagMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Global.Working.FlagOn = (Manuscript.FlagMode)cmbFlagMode.SelectedValue;

            SetFlagThresholdDesc();
        }

        private void SetFlagThresholdDesc()
        {
            switch (Global.Working.FlagOn)
            {
                case Manuscript.FlagMode.Off:
                    lblFlagModeDesc.Text = "Flagging is currently off for this manuscript.";
                    break;
                case Manuscript.FlagMode.Top:
                    lblFlagModeDesc.Text = "Only the top [flag threhold] most frequently used words will be flagged.";
                    break;
                case Manuscript.FlagMode.Normal:
                default:
                    lblFlagModeDesc.Text = "All words used above the current [flag threshold] will be flagged.";
                    break;
            }
        }
    }
}
