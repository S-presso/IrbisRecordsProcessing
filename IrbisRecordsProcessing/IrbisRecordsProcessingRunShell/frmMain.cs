/*
Printer++ Virtual Printer Processor
Copyright (C) 2012 - Printer++

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
*/

using IrbisRecordsProcessing;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IrbisRecordsProcessing
{
    public partial class frmMain : Form
    {
        private bool closedFromMenu = false;        

        public frmMain()
        {
            InitializeComponent();
        }

        public frmMain(string fileName)
        {
            try
            {
                InitializeComponent();
                
                if (!string.IsNullOrWhiteSpace(fileName))
                    txtFileName.Text = fileName;

                Process(fileName);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            Process(txtFileName.Text);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            try
            {
                if (fDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFileName.Text = fDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void Process(string fileName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {

                    string text = HtmlToPlainText(System.IO.File.ReadAllText(fileName));
                    Processor.ProcessResult(new StreamReader(GenerateStreamFromString(text)));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }

        }

        private MemoryStream GenerateStreamFromString(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""));
        }

        private string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        #region Form
        private void ShowError(Exception ex)
        {
            //IO.Log(ex.Message);
            MessageBox.Show(ex.Message, "Unhandled Exception");
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
                closedFromMenu = true;

            if (closedFromMenu == false)
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
                e.Cancel = true;
            }            
        }
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            mnuTray_ShowHide_Click(sender, e);
        }
        #endregion

        #region mnuTray
        private void mnuTray_Exit_Click(object sender, EventArgs e)
        {
            closedFromMenu = true;
            this.Close();
        }
        private void mnuTray_About_Click(object sender, EventArgs e)
        {

        }
        private void mnuTray_ShowHide_Click(object sender, EventArgs e)
        {
            //this.Visible = !this.Visible;
            //if (!this.ShowInTaskbar)
            //    this.ShowInTaskbar = true;
            this.ShowInTaskbar = !this.ShowInTaskbar;

            if (this.WindowState == FormWindowState.Minimized)
                this.WindowState = FormWindowState.Normal;
            else
                this.WindowState = FormWindowState.Minimized;
        }
        #endregion

    }
}
