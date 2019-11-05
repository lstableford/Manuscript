using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using mss_core;

namespace mss
{
    internal static class Global
    {
        internal static Manuscript Working { get; set; }
        internal static int MaxBody
        {
            get
            {
                if (Working.Body.Count > 0)
                    return Working.Body.Where(p => !p.Heading).Max(p => p.Number);
                else
                    return 1;
            }
        }
        internal static int HeaderValue
        {
            get
            {
                Paragraph para = Working.Body.FirstOrDefault(p => p.Heading);
                return para == null ? 0 : para.Number;
            }
        }
        internal static string CurrentFile{ get; set; }
        internal static void SetRecentFiles(string openedFile)
        {
            List<string> RecentFiles = new List<string>();

            RecentFiles.Add(Properties.Settings.Default.Recent1);
            RecentFiles.Add(Properties.Settings.Default.Recent2);
            RecentFiles.Add(Properties.Settings.Default.Recent3);
            RecentFiles.Add(Properties.Settings.Default.Recent4);
            RecentFiles.Add(Properties.Settings.Default.Recent5);

            bool replace = RecentFiles.Contains(openedFile);

            if (replace)
            {
                int replaceIndex = RecentFiles.IndexOf(openedFile);

                RecentFiles[replaceIndex] = null;

                for (int r = replaceIndex; r < 4; r++)
                {
                    if (!string.IsNullOrEmpty(RecentFiles[r + 1]))
                    {
                        RecentFiles[r] = RecentFiles[r + 1];
                        RecentFiles[r + 1] = null;
                    }
                }
            }

            for (int r = 4; r > 0; r--)
            {
                if (!string.IsNullOrEmpty(RecentFiles[r - 1])) RecentFiles[r] = RecentFiles[r - 1];
                else RecentFiles[r] = null;
            }


            Properties.Settings.Default.Recent1 = openedFile;
            Properties.Settings.Default.Recent2 = !string.IsNullOrEmpty(RecentFiles[1]) ? RecentFiles[1] : null;
            Properties.Settings.Default.Recent3 = !string.IsNullOrEmpty(RecentFiles[2]) ? RecentFiles[2] : null;
            Properties.Settings.Default.Recent4 = !string.IsNullOrEmpty(RecentFiles[3]) ? RecentFiles[3] : null;
            Properties.Settings.Default.Recent5 = !string.IsNullOrEmpty(RecentFiles[4]) ? RecentFiles[4] : null;
            
            Properties.Settings.Default.Save();
        }

        internal static void SaveSettings()
        {
            Properties.Settings.Default.Save();
        }

        internal static void OpenFile()
        {
            OpenFile(null);
        }
        internal static void OpenFile(Manuscript.WordType? wt)
        {
            string filter = wt == null ? "txt files (*.txt)|*.txt|xml files (*.xml)|*.xml" : "xml files (*.xml)|*.xml";

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.RestoreDirectory = true;
            ofd.Filter = filter;
            ofd.FilterIndex = 2;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (wt.HasValue)
                {
                    Manuscript.WordType _wt = wt.Value;
                    
                    XDocument xd = XDocument.Load(ofd.FileName);

                    switch (_wt)
                    {
                        case Manuscript.WordType.Danger:
                            Working.LoadXWords(xd.Element("dangerwords"),_wt);
                            break;
                        case Manuscript.WordType.Invisible:
                        default:
                            Working.LoadXWords(xd.Element("invisiblewords"),_wt);
                            break;
                    }

                }
                else
                {
                    CurrentFile = ofd.FileName;
                    doOpen(ofd.FilterIndex);
                }
            }
        }
        internal static void LoadCurrentFile()
        {
            string filter = CurrentFile.Substring(CurrentFile.LastIndexOf("."));
            int filteridx = filter == ".txt" ? 1 : 2;
            doOpen(filteridx);
        }
        private static void doOpen(int filter)
        {
            try
            {
                Working = new Manuscript();

                SetRecentFiles(CurrentFile);

                if (filter == 1)
                {
                    StreamReader s = File.OpenText(CurrentFile);
                    string read = s.ReadToEnd();
                    s.Close();
                    s.Dispose();

                    Working = new Manuscript(read);

                    CurrentFile = CurrentFile.Remove(CurrentFile.Length - 4) + ".xml";
                }
                else
                {
                    XDocument xd = XDocument.Load(CurrentFile);
                    Working = new Manuscript(xd);
                }
            }
            catch 
            {
                MessageBox.Show("Error opening file " + CurrentFile + " Load Aborted.");
                Working = new Manuscript();
            }
        }

        internal static void SaveFile()
        {
            SaveFile(false,null);
        }
        internal static void SaveFile(bool full)
        {
            SaveFile(full, null);
        }
        internal static void SaveFile(bool full, XDocument xd)
        {
            if (String.IsNullOrEmpty(CurrentFile) || full)
            {
                string filter = "xml files (*.xml)|*.xml|txt files (*.txt)|*.txt";

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = filter;
                sfd.FilterIndex = 1;
                sfd.RestoreDirectory = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (sfd.FilterIndex == 1)
                    {
                        if (xd != null)
                        {
                            xd.Save(sfd.FileName);
                        }
                        else Global.Working.ToXDocument().Save(sfd.FileName);
                    }
                    else
                    {
                        File.WriteAllLines(sfd.FileName, Global.Working.ExportToTextArray());
                    }
                }
            }
            else Working.ToXDocument().Save(CurrentFile);
        }
    }
}
