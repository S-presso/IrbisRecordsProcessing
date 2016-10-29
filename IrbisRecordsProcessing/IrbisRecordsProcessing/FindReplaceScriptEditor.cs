using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using ManagedClient;

namespace IrbisRecordsProcessing
{
    public partial class FindReplaceScriptEditor : Form
    {
        public List<SearchScriptResult> SearchScriptResultList;
        
        public ManagedClient64 client;
        public ManagedClient.ManagedClient64.IrbisDatabase curDatabase;
        public String SelectRecordCondition, SelectFieldCondition, SelectSubfieldCondition;
        public String FindTextString, ReplaceTextString;
        public String ScriptForPrologCode, ScriptForEpilogCode, ScriptForRecordCode, ScriptForFieldCode, ScriptForSubfieldCode;
        public int activeTabIndex = 0;
        public ExtractedValue recordsData;
        Configuration config;


        public FindReplaceScriptEditor(ExtractedValue recordsData)
        {
            this.recordsData = recordsData;
            config = null;
            string exeConfigPath = this.GetType().Assembly.Location;
            try
            {
                config = ConfigurationManager.OpenExeConfiguration(exeConfigPath);

                if (config != null)
                {
                    activeTabIndex = Convert.ToByte(GetAppSetting(config, "activeTabIndex"));
                    string connectionString = GetAppSetting(config, "connection-string");
                    client = new ManagedClient64();
                    client.ParseConnectionString(connectionString);
                    try
                    {
                        client.Connect();

                        if (!String.IsNullOrEmpty(recordsData.DBName))
                            client.Database = recordsData.DBName;

                        curDatabase = client.IrbisDatabases.dataBases[client.IrbisDatabases.SelectedIndex];
                    }
                    catch
                    {
                        client.Shutdown();
                    }

                    InitializeComponent();
                    cmbTemplateType.SelectedIndex = 1;
                }
            }
            catch (Exception ex)
            {
                //handle errror here.. means DLL has no sattelite configuration file.
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private string GetAppSetting(Configuration config, string key)
        {
            KeyValueConfigurationElement element = config.AppSettings.Settings[key];
            if (element != null)
            {
                string value = element.Value;
                if (!string.IsNullOrEmpty(value))
                    return value;
            }
            return string.Empty;
        }

        private void FindReplaceScriptEditor_Load(object sender, EventArgs e)
        {            
            /*FindTextStr.Text = FindTextStr2.Text = FindTextString;
            ReplaceTextStr.Text = ReplaceTextString;*/
            
            tabControlFindReplace.SelectedIndex = activeTabIndex;            
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SelectRecordCondition = SelectRecordConditionScriptEdit.Text;
            SelectFieldCondition = SelectFieldConditionScriptEdit.Text;
            SelectSubfieldCondition = SelectSubfieldConditionScriptEdit.Text;

            if (tabControlFindReplace.SelectedIndex != activeTabIndex)
            {
                activeTabIndex = tabControlFindReplace.SelectedIndex;
                config.AppSettings.Settings["activeTabIndex"].Value = activeTabIndex.ToString();
                config.Save();
            }

            if (activeTabIndex != 0)
                FindTextString = FindTextStr.Text;
            else
                FindTextString = FindTextStr2.Text;
                
            ReplaceTextString = ReplaceTextStr.Text;

            ScriptForPrologCode = ScriptForProlog.Text;
            ScriptForEpilogCode = ScriptForEpilog.Text;
            ScriptForRecordCode = ScriptForRecord.Text;
            ScriptForFieldCode = ScriptForField.Text;
            ScriptForSubfieldCode = ScriptForSubfield.Text;
        }

        private void tabControlFindReplace_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlFindReplace.SelectedIndex == 0)
            {
                FindTextStr2.Text = FindTextStr.Text;
                chkMatchCase2.Checked = chkMatchCase.Checked;
            }
            else
            {
                FindTextStr.Text = FindTextStr2.Text;
                chkMatchCase.Checked = chkMatchCase2.Checked;
            }
        }        
    }

    public class SearchScriptResult
    {
        public int mfn;
        //public IrbisRecord record;
        public String fieldTag;
        public int fieldOcc;
        public String foundStr, replaceToStr;
        public String contextStr, contextModStr;
        public int index;
        public bool waitProcessed, applyResult;
    }

    public class MyMatch
    {
        public int Index;
        public String Value;
    }    
}
