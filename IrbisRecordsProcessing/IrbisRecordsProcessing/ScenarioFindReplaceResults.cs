using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ManagedClient;
using RemObjects.Script;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using Microsoft.Win32;
using OMS.Ice.T4Generator;
using Xipton.Razor;


namespace IrbisRecordsProcessing
{
    public partial class ScenarioFindReplaceResultsForm : Form
    {        
        private FindReplaceScriptEditor find_replaceScriptEditor;
        private EcmaScriptComponent ScriptEngine;
        bool SelectRecordEmpty, SelectFieldEmpty, SelectSubfieldEmpty;
        String SelectRecordCondition, SelectFieldCondition, SelectSubfieldCondition;
        String FindTextStr, ReplaceTextStr;
        StringCollection applyActions, applyActionsToAll;
        bool cancelled;
        enum WorkMode { Search, Replace, Script, Template }
        WorkMode workMode;
        enum TemplateMode { T4, Razor }
        TemplateMode templateMode;
        StringBuilder templateStringBuilder = new StringBuilder();
 

        public ScenarioFindReplaceResultsForm(FindReplaceScriptEditor find_replaceScriptEditor)
        {
            InitializeComponent();            
            this.find_replaceScriptEditor = find_replaceScriptEditor;
            this.SelectRecordCondition = find_replaceScriptEditor.SelectRecordCondition;
            this.SelectFieldCondition = find_replaceScriptEditor.SelectFieldCondition;
            this.SelectSubfieldCondition = find_replaceScriptEditor.SelectSubfieldCondition;

            this.FindTextStr = find_replaceScriptEditor.FindTextString;
            this.ReplaceTextStr = find_replaceScriptEditor.ReplaceTextString;

            workMode = (WorkMode)find_replaceScriptEditor.activeTabIndex;
            templateMode = (TemplateMode)find_replaceScriptEditor.cmbTemplateType.SelectedIndex;
            
            applyActions = new StringCollection();
            applyActions.Add("ПОДТВЕРДИТЬ");
            applyActions.Add("ОТМЕНИТЬ");

            applyActionsToAll = new StringCollection();
            applyActionsToAll.Add("ПОДТВЕРДИТЬ ВСЕ");
            applyActionsToAll.Add("ОТМЕНИТЬ ВСЕ");


            ScriptEngine = new EcmaScriptComponent();
            RemObjectUtils.ExposeAssembly(ScriptEngine, typeof(ManagedClient.ManagedClient64).Assembly);
            ScriptEngine.Globals.SetVariable("client", find_replaceScriptEditor.client);
            ScriptEngine.Globals.SetVariable("curDatabase", find_replaceScriptEditor.curDatabase);            
        }        

        private void ScenarioFindReplaceResultsForm_Load(object sender, EventArgs e)
        {
            ShowScenarioFindReplaceResults();
            okButton.Enabled = false;
        }

        private void ScenarioFindReplaceResultsForm_Shown(object sender, EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PerformSearch();            
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar.Value = e.ProgressPercentage;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            saveOutputForm();

            if (workMode != WorkMode.Replace)
                return;

            DialogResult result = MessageBoxAdv.Show("Данные действия по изменению БД являются необратимыми. Выполнить обработку?", "Внимание!", MessageBoxButtons.OKCancel);
            if (result != DialogResult.OK)
                return;

            int count;
            ApplyResults(out count);
            MessageBoxAdv.Show(String.Format("Обработано записей: {0}", count));
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync();
                MessageBoxAdv.Show("Поиск прерван.");
            }
        }

        private void saveOutputForm()
        {
            DialogResult result;
            String resultText, resultMessage;

            if (workMode == WorkMode.Search || workMode == WorkMode.Template)
            {
                result = saveFileDialog.ShowDialog();
                if (result != DialogResult.OK)
                    return;

                if (workMode == WorkMode.Search)
                {
                    string[] strMarkers = { "DBN:", "MFN:" };
                    StringBuilder resultRecords = new StringBuilder();
                    resultRecords.AppendLine(strMarkers[0] + find_replaceScriptEditor.curDatabase.dbName);

                    List<int> MfnList = find_replaceScriptEditor.SearchScriptResultList.Select(x => x.mfn).Distinct().ToList();

                    foreach (int mfn in MfnList)
                        resultRecords.AppendLine(strMarkers[1] + mfn);

                    resultText = resultRecords.ToString();
                    resultMessage = String.Format("Найденные записи сохранены в файле {0}. Хотите просмотреть?", saveFileDialog.FileName);
                }
                else
                {
                    resultText = templateStringBuilder.ToString();
                    resultMessage = String.Format("Выходной шаблон сохранён в файле {0}. Хотите просмотреть?", saveFileDialog.FileName);
                }

                File.WriteAllText(saveFileDialog.FileName, resultText, Encoding.Default);
                result = MessageBoxAdv.Show(resultMessage, "Внимание!", MessageBoxButtons.OKCancel);

                if (result == DialogResult.OK)
                {
                    string appName;
                    if (Utils.TryGetRegisteredApplication(System.IO.Path.GetExtension(saveFileDialog.FileName), out appName))
                        if (appName == Application.ExecutablePath)
                        {
                            Process.GetCurrentProcess().WaitForExit(1000);
                        }

                    Process.Start(appName, saveFileDialog.FileName);


                    /*ProcessStartInfo pi = new ProcessStartInfo(saveFileDialog.FileName);
                    try
                    {
                        Process.Start(pi);
                    }
                    catch
                    {
                        Process.Start(Application.ExecutablePath, saveFileDialog.FileName);
                        Application.Exit();
                    }
                    

                    //var processes = Process.GetProcesses().Where(p => p.MainModule.FileName.StartsWith(saveFileDialog.FileName, true, CultureInfo.InvariantCulture));
                    /*foreach (Process proc in Process.GetProcesses())
                    {
                        proc.Kill();
                    }*/

                    //Process.GetCurrentProcess().Kill();

                    /*Process[] myProcess = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
                    foreach (Process process in myProcess)
                    {
                        process.CloseMainWindow();
                        //all the windows messages has to be processed in the msg queue
                        //hence call to Application DoEvents forces the MSG
                        Application.DoEvents();
                    }

                    ProcessStartInfo pi = new ProcessStartInfo(saveFileDialog.FileName);
                    pi.UseShellExecute = true;
                    Process.Start(pi);

                    /*Process newProc = new Process();
                    newProc.StartInfo.Arguments = Path.GetFileName(saveFileDialog.FileName);
                    //p.StartInfo.UseShellExecute = true;
                    newProc.StartInfo.WorkingDirectory = Path.GetDirectoryName(saveFileDialog.FileName);
                    newProc.StartInfo.FileName = saveFileDialog.FileName;
                    newProc.StartInfo.Verb = "OPEN";
                    newProc.Start();*/
                }
                return;
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            toolStripProgressBar.Value = 0;
            if (!cancelled && workMode != WorkMode.Script && workMode != WorkMode.Template)
                MessageBoxAdv.Show(String.Format("Обнаружено вхождений: {0}", find_replaceScriptEditor.SearchScriptResultList.Count));

            if (find_replaceScriptEditor.SearchScriptResultList.Count > 0)
            {
                okButton.Enabled = true;
                okButton.Focus();
            }

            if (workMode == WorkMode.Script || workMode == WorkMode.Template)
            {
                this.Close();
                saveOutputForm();
            }
        }

        private void PerformSearch()
        {
            SelectRecordEmpty = String.IsNullOrWhiteSpace(SelectRecordCondition);
            SelectFieldEmpty = String.IsNullOrWhiteSpace(SelectFieldCondition);
            SelectSubfieldEmpty = String.IsNullOrWhiteSpace(SelectSubfieldCondition);

            find_replaceScriptEditor.SearchScriptResultList = new List<SearchScriptResult>();
            
            IrbisRecord Record;
            MatchCollection searchMatches;
            List<MyMatch> searchMatchesList;
            RecordField[] fields;
            int fieldOcc;            
            String fieldText, fieldTextProtected;
            int index0, index1, index2;
            String[] RecordStr = new string[] { "Record" };
            String[] FieldStr = new string[] { "Field" };
            String[] SubFieldStr = new string[] { "Subfield" };

            RegexOptions regexMatchCase = find_replaceScriptEditor.chkMatchCase.Checked ? RegexOptions.IgnoreCase : RegexOptions.None;
            int index = 0;

            StringBuilder templateProlog = new StringBuilder();

            if (templateMode == TemplateMode.T4)
            {
                templateProlog.AppendLine("<#@ template language=\"C#\" #>");
                templateProlog.AppendLine("<#@ assembly name=\"System.Core.dll\" #>");
                templateProlog.AppendLine("<#@ import namespace=\"System.Collections.Generic\" #>");
                templateProlog.AppendLine("<#@ assembly name=\"$(ProjectDir)$(OutDir)ManagedClient.dll\" #>");
                templateProlog.AppendLine("<#@ import namespace=\"ManagedClient\" #>");
                templateProlog.AppendLine("<#@ parameter type=\"ManagedClient64\" name=\"client\" #>");
            }
            else
            {
                templateProlog.AppendLine("@using ManagedClient");
                templateProlog.AppendLine("@using IrbisRecordsProcessing");
                templateProlog.AppendLine("@{ String EoL = Environment.NewLine; }");
            }

            ScriptEngine.Globals.SetVariable("client", find_replaceScriptEditor.client);
            ScriptEngine.Globals.SetVariable("curDatabase", find_replaceScriptEditor.curDatabase);

                        
            StringBuilder templatePrologStr;

            if (workMode == WorkMode.Script)
                RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.PrologScriptCode);
            else if (workMode == WorkMode.Template)
                if (templateMode == TemplateMode.T4)
                    Utils.AppendGeneratedT4Template(find_replaceScriptEditor.PrologTemplate.Text, templateProlog, ref templateStringBuilder, find_replaceScriptEditor.client);
                else
                    Utils.AppendGeneratedRazorTemplate(find_replaceScriptEditor.PrologTemplate.Text, templateProlog, ref templateStringBuilder,
                        new { client = find_replaceScriptEditor.client });
            
            int[] mfnRange = find_replaceScriptEditor.recordsData.MfnList;            
            
            
            foreach (int mfn in mfnRange)
            {
                if (backgroundWorker.CancellationPending)
                {
                    cancelled = true;
                    break;
                }

                Record = find_replaceScriptEditor.client.ReadRecord(mfn);
                backgroundWorker.ReportProgress((int)(((float)mfn / find_replaceScriptEditor.curDatabase.Length) * 100));

                if (workMode == WorkMode.Template)
                {
                    templatePrologStr = new StringBuilder();
                    templatePrologStr.Append(templateProlog);
                    if (templateMode == TemplateMode.T4)
                    {
                        templatePrologStr.Append(templateProlog).AppendLine("<#@ parameter type=\"IrbisRecord\" name=\"record\" #>");
                        Utils.AppendGeneratedT4Template(find_replaceScriptEditor.WorkingTemplate.Text, templatePrologStr, ref templateStringBuilder, find_replaceScriptEditor.client, Record);
                    }
                    else
                        Utils.AppendGeneratedRazorTemplate(find_replaceScriptEditor.WorkingTemplate.Text, templatePrologStr, ref templateStringBuilder,
                            new { client = find_replaceScriptEditor.client, record = Record });
                    continue;                    
                }

                if (SelectRecordEmpty || RemObjectUtils.ScriptEval(ScriptEngine, SelectRecordCondition, RecordStr, Record))
                {
                    if (workMode == WorkMode.Script)
                    {
                        RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.PrologRecordScriptCode, RecordStr, Record);
                        if (String.IsNullOrWhiteSpace(find_replaceScriptEditor.SubfieldScriptCode) && String.IsNullOrWhiteSpace(find_replaceScriptEditor.SubfieldScriptCode))
                        {
                            RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.EpilogRecordScriptCode);
                            continue;
                        }
                    }
                    
                    foreach (RecordField Field in Record.Fields)
                    {
                        searchMatchesList = new List<MyMatch>();
                        fieldText = Field.ToText();
                        fieldTextProtected = Regex.Replace(fieldText, "\\^\\S", "  ");

                        if (SelectFieldEmpty || RemObjectUtils.ScriptEval(ScriptEngine, SelectFieldCondition, FieldStr, Field))
                        {
                            if (workMode == WorkMode.Script)
                            {
                                RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.FieldScriptCode, FieldStr, Field);
                                if (String.IsNullOrWhiteSpace(find_replaceScriptEditor.SubfieldScriptCode))
                                    continue;
                            }
                            
                            if (SelectSubfieldEmpty && workMode != WorkMode.Script)
                            {
                                searchMatches = Regex.Matches(fieldText, FindTextStr, regexMatchCase);
                                foreach (Match nextMatch in searchMatches)
                                {
                                    MyMatch newMatch = new MyMatch();
                                    newMatch.Value = nextMatch.Value;
                                    newMatch.Index = nextMatch.Index;
                                    searchMatchesList.Add(newMatch);
                                }
                            }
                            else
                            {
                                foreach (SubField Subfield in Field.SubFields)
                                    if (SelectSubfieldEmpty || RemObjectUtils.ScriptEval(ScriptEngine, SelectSubfieldCondition, SubFieldStr, Subfield))
                                    {
                                        if (workMode == WorkMode.Script)
                                        {
                                            RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.SubfieldScriptCode, SubFieldStr, Subfield);
                                            continue;
                                        }
                                        
                                        searchMatches = Regex.Matches(Subfield.Text, FindTextStr, regexMatchCase);

                                        foreach (Match nextMatch in searchMatches)
                                        {
                                            MyMatch newMatch = new MyMatch();
                                            newMatch.Value = nextMatch.Value;
                                            newMatch.Index = nextMatch.Index + fieldText.IndexOf("^" + Subfield.Code) + 2;

                                            searchMatchesList.Add(newMatch);
                                        }
                                    }
                            }


                            if (workMode == WorkMode.Script)
                                continue;                            

                            foreach (MyMatch nextMatch in searchMatchesList)
                            {
                                SearchScriptResult searchScriptResult = new SearchScriptResult();
                                searchScriptResult.foundStr = nextMatch.Value;
                                searchScriptResult.index = nextMatch.Index;
                                searchScriptResult.mfn = mfn;

                                index0 = searchScriptResult.index;
                                if (index0 > 0)
                                    index1 = fieldTextProtected.LastIndexOf(" ", index0 - 1);
                                else
                                    index1 = 0;
                                if (index1 == -1)
                                    index1 = 0;

                                index0 += searchScriptResult.foundStr.Length;

                                if (index0 < fieldTextProtected.Length - 1)
                                    index2 = fieldTextProtected.IndexOf(" ", index0);
                                else
                                    index2 = -1;

                                if (index2 == -1)
                                    index2 = fieldTextProtected.Length;

                                if (index1 > 0)
                                    searchScriptResult.contextStr = fieldTextProtected.Substring(index1 + 1, index2 - index1 - 1);
                                else
                                    searchScriptResult.contextStr = fieldTextProtected.Substring(0, index2);

                                index++;

                                if (find_replaceScriptEditor.activeTabIndex != 0)
                                {
                                    searchScriptResult.replaceToStr = Regex.Replace(nextMatch.Value, FindTextStr, ReplaceTextStr, regexMatchCase);
                                    
                                    searchScriptResult.contextModStr = Regex.Replace(searchScriptResult.contextStr, FindTextStr, ReplaceTextStr, regexMatchCase);
                                    
                                    searchScriptResult.fieldTag = Field.Tag;

                                    fields = Record.Fields.GetField(Field.Tag);
                                    for (fieldOcc = 0; fieldOcc < fields.Length; fieldOcc++)
                                        if (fields[fieldOcc] == Field)
                                            break;
                                    searchScriptResult.fieldOcc = fieldOcc;

                                    find_replaceScriptEditor.SearchScriptResultList.Add(searchScriptResult);
                                    
                                    if (!backgroundWorker.CancellationPending)
                                        this.Invoke((MethodInvoker)delegate
                                            {
                                                AdvStringGrid.BeginUpdate();
                                                AdvStringGrid.RowCount = index;
                                                AdvStringGrid[index, 1].Text = searchScriptResult.foundStr;
                                                AdvStringGrid[index, 2].Text = searchScriptResult.replaceToStr;
                                                AdvStringGrid[index, 3].Text = searchScriptResult.contextStr;
                                                AdvStringGrid[index, 4].Text = searchScriptResult.contextModStr;
                                                AdvStringGrid[index, 5].CellType = GridCellTypeName.ComboBox;
                                                AdvStringGrid[index, 5].DataSource = applyActions;
                                                AdvStringGrid[index, 5].CellValue = applyActions[0];
                                                AdvStringGrid.ScrollCellInView(GridRangeInfo.Row(index), GridScrollCurrentCellReason.Any);
                                                AdvStringGrid.EndUpdate();
                                            });
                                    else
                                    {
                                        cancelled = true;
                                        break;
                                    }

                                }
                                else
                                {
                                    find_replaceScriptEditor.SearchScriptResultList.Add(searchScriptResult);
                                    
                                    if (!backgroundWorker.CancellationPending)
                                        this.Invoke((MethodInvoker)delegate
                                        {
                                            AdvStringGrid.BeginUpdate();
                                            AdvStringGrid.RowCount = index;
                                            AdvStringGrid[index, 1].Text = searchScriptResult.foundStr;
                                            AdvStringGrid[index, 2].Text = searchScriptResult.contextStr;
                                            AdvStringGrid.ScrollCellInView(GridRangeInfo.Row(index), GridScrollCurrentCellReason.Any);
                                            AdvStringGrid.EndUpdate();
                                        });
                                    else
                                    {
                                        cancelled = true;
                                        break;
                                    }
                                }                                
                            }
                        }
                    }                    
                }
                if (workMode == WorkMode.Script)                
                    RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.EpilogRecordScriptCode);
            }
            
            if (workMode == WorkMode.Script)
                RemObjectUtils.ScriptRun(ScriptEngine, find_replaceScriptEditor.EpilogScriptCode);
            else if (workMode == WorkMode.Template)
                if (templateMode == TemplateMode.T4)
                    Utils.AppendGeneratedT4Template(find_replaceScriptEditor.EpilogTemplate.Text, templateProlog, ref templateStringBuilder, find_replaceScriptEditor.client);
                else
                    Utils.AppendGeneratedRazorTemplate(find_replaceScriptEditor.EpilogTemplate.Text, templateProlog, ref templateStringBuilder,
                        new { client = find_replaceScriptEditor.client });
        }
        

        private void ShowScenarioFindReplaceResults()
        {
            if (workMode == WorkMode.Script)
                return;
            
            if (workMode == WorkMode.Replace)
                AdvStringGrid.ColCount = 5;
            else
                AdvStringGrid.ColCount = 2;

            AdvStringGrid.Model.Options.NumberedRowHeaders = false;

            int colIndex = 1;
            AdvStringGrid[0, colIndex].Text = "Найденная строка";
            AdvStringGrid.ColWidths[colIndex++] = 100;
            if (find_replaceScriptEditor.activeTabIndex != 0)
            {
                AdvStringGrid[0, colIndex].Text = "Заменить на";
                AdvStringGrid.ColWidths[colIndex++] = 100;
            }
            AdvStringGrid[0, colIndex].Text = "Контекст";
            AdvStringGrid.ColWidths[colIndex++] = 200;
            if (find_replaceScriptEditor.activeTabIndex != 0)
            {
                AdvStringGrid[0, colIndex].Text = "Предполагаемая замена";
                AdvStringGrid.ColWidths[colIndex++] = 200;
                AdvStringGrid[0, colIndex].CellValue = "Действие по замене";
                AdvStringGrid[0, colIndex].CellType = GridCellTypeName.ComboBox;
                AdvStringGrid[0, colIndex].CellAppearance = GridCellAppearance.Raised;
                AdvStringGrid[0, colIndex].DataSource = applyActionsToAll;
                AdvStringGrid.ColWidths[colIndex] = 110;
            }            
        }

        private void AdvStringGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (AdvStringGrid.CurrentCell.RowIndex != 0 || AdvStringGrid.CurrentCell.ColIndex != 5)
                return;

            int selIndex = applyActionsToAll.IndexOf(AdvStringGrid.CurrentCell.Renderer.ControlValue.ToString());
            if (selIndex != -1)
                for (int rowIndex = 0; rowIndex < AdvStringGrid.RowCount; rowIndex++)
                    AdvStringGrid[rowIndex + 1, 5].CellValue = applyActions[selIndex];
        }

        private void UpdateFieldOcc(IrbisRecord record, String Tag, int Occ, String Text)
        {
            RecordField field = record.Fields.GetField(Tag).Skip(Occ).FirstOrDefault();
            field.Text = Text;            
        }

        private RecordField GetProcessedField(int mfn, String Tag, int Occ, out IrbisRecord record)
        {
            record = find_replaceScriptEditor.client.ReadRecord(mfn);
            return GetProcessedField(mfn, Tag, Occ, record);
        }

        private RecordField GetProcessedField(int mfn, String Tag, int Occ, IrbisRecord record)
        {            
            RecordField[] fields = record.Fields.GetField(Tag);
            return fields[Occ];
        }
        

        private void ApplyResults(out int count)
        {
            IrbisRecord record;
            RecordField processedField;
            SearchScriptResult prevResult;
            bool notProcessedExists;

            do
            {                
                prevResult = null;
                notProcessedExists = false;

                for (int i = 0; i < find_replaceScriptEditor.SearchScriptResultList.Count; i++)
                {
                    toolStripProgressBar.Value = (int)(((float)i / find_replaceScriptEditor.SearchScriptResultList.Count) * 100);

                    if (prevResult != null && find_replaceScriptEditor.SearchScriptResultList[i].mfn == prevResult.mfn && find_replaceScriptEditor.SearchScriptResultList[i].fieldTag == prevResult.fieldTag && find_replaceScriptEditor.SearchScriptResultList[i].fieldOcc == prevResult.fieldOcc)
                    {
                        find_replaceScriptEditor.SearchScriptResultList[i].index = find_replaceScriptEditor.SearchScriptResultList[i].index + prevResult.replaceToStr.Length - prevResult.foundStr.Length;                        
                        notProcessedExists = true;
                        continue;
                    }

                    if (!find_replaceScriptEditor.SearchScriptResultList[i].applyResult)
                        if (AdvStringGrid[i + 1, 5].CellValue.ToString() == applyActions[0].ToString())
                        {
                            find_replaceScriptEditor.SearchScriptResultList[i].applyResult = true;
                            prevResult = find_replaceScriptEditor.SearchScriptResultList[i];                            
                        }
                }                
            }
            while (notProcessedExists);
            
            record = null;
            prevResult = null;            
            String fieldText = null;
            count = 0;

            for (int i = 0; i < find_replaceScriptEditor.SearchScriptResultList.Count; i++)
                if (find_replaceScriptEditor.SearchScriptResultList[i].applyResult)
                {
                    toolStripProgressBar.Value = (int)(((float)i / find_replaceScriptEditor.SearchScriptResultList.Count) * 100);

                    if (prevResult != null && find_replaceScriptEditor.SearchScriptResultList[i].mfn == prevResult.mfn)
                    {
                        if (find_replaceScriptEditor.SearchScriptResultList[i].fieldTag != prevResult.fieldTag || find_replaceScriptEditor.SearchScriptResultList[i].fieldOcc != prevResult.fieldOcc)
                        {
                            UpdateFieldOcc(record, prevResult.fieldTag, prevResult.fieldOcc, fieldText);
                            processedField = GetProcessedField(find_replaceScriptEditor.SearchScriptResultList[i].mfn, find_replaceScriptEditor.SearchScriptResultList[i].fieldTag, find_replaceScriptEditor.SearchScriptResultList[i].fieldOcc, record);
                            fieldText = processedField.ToText();
                        }                            
                    }
                    else
                    {
                        if (prevResult != null)
                            UpdateFieldOcc(record, prevResult.fieldTag, prevResult.fieldOcc, fieldText);
                        
                        if (record != null)
                        {
                            try
                            {
                                find_replaceScriptEditor.client.WriteRecord(record, false, true);
                                count++;
                            }
                            catch (IrbisException)
                            {
                            }                            
                        }
                        processedField = GetProcessedField(find_replaceScriptEditor.SearchScriptResultList[i].mfn, find_replaceScriptEditor.SearchScriptResultList[i].fieldTag, find_replaceScriptEditor.SearchScriptResultList[i].fieldOcc, out record);
                        fieldText = processedField.ToText();
                    }

                    fieldText = fieldText.Substring(0, find_replaceScriptEditor.SearchScriptResultList[i].index) + find_replaceScriptEditor.SearchScriptResultList[i].replaceToStr
                              + fieldText.Substring(find_replaceScriptEditor.SearchScriptResultList[i].index + find_replaceScriptEditor.SearchScriptResultList[i].foundStr.Length);
                    prevResult = find_replaceScriptEditor.SearchScriptResultList[i];
                }

            if (prevResult != null && prevResult.mfn == record.Mfn)
            {
                UpdateFieldOcc(record, prevResult.fieldTag, prevResult.fieldOcc, fieldText);

                try
                {
                    find_replaceScriptEditor.client.WriteRecord(record, false, true);
                    count++;
                }
                catch (IrbisException)
                {
                }
            }
        }
    }

    public static class Utils
    {
        public static bool TryGetRegisteredApplication(string extension, out string registeredApp)
        {
            string extensionId = GetClassesRootKeyDefaultValue(extension);
            if (extensionId == null)
            {
                registeredApp = null;
                return false;
            }

            string openCommand = GetClassesRootKeyDefaultValue(
                    Path.Combine(new[] { extensionId, "shell", "open", "command" }));

            if (openCommand == null)
            {
                registeredApp = null;
                return false;
            }

            if (openCommand.Contains("\""))
            {
                openCommand = openCommand.Split(new[] { "\"" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            else
            {
                openCommand = openCommand.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];
            }

            registeredApp = openCommand;

            return true;
        }

        private static string GetClassesRootKeyDefaultValue(string keyPath)
        {
            using (var key = Registry.ClassesRoot.OpenSubKey(keyPath))
            {
                if (key == null)
                {
                    return null;
                }

                var defaultValue = key.GetValue(null);
                if (defaultValue == null)
                {
                    return null;
                }

                return defaultValue.ToString();
            }
        }

        public static void AppendGeneratedT4Template(string templateText, StringBuilder templateProlog, ref StringBuilder templateStringBuilder, params object[] parameters)
        {
            IGenerator generator = new Generator();
            MemoryStream stream = new MemoryStream();
            StreamWriter textWriter = new StreamWriter(stream);
            StreamReader textReader;            
            
            StringBuilder templateStr = new StringBuilder();
            templateStr.AppendLine(templateProlog.ToString());
            templateStr.AppendLine(templateText);

            string templateFilePath = GetTempFilePathWithExtension(".t4");
            File.WriteAllText(templateFilePath, templateStr.ToString());


            generator.Generate(templateFilePath, textWriter, parameters);
            //generator.Generate(textWriter, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Part2\Part2.t4"), friends);
            stream.Seek(0, SeekOrigin.Begin);
            textReader = new StreamReader(stream);
            templateStringBuilder.Append(textReader.ReadToEnd());
            File.Delete(templateFilePath);            
        }

        public static void AppendGeneratedRazorTemplate(string templateText, StringBuilder templateProlog, ref StringBuilder templateStringBuilder, object model)
        {
            StringBuilder templateStr = new StringBuilder();
            templateStr.AppendLine(templateProlog.ToString());
            templateStr.AppendLine(templateText);
            
            RazorMachine rm = new RazorMachine();
            ITemplate template = rm.ExecuteContent(templateStr.ToString(), model);
            templateStringBuilder.Append(template.Result);
        }

        public static string GetTempFilePathWithExtension(string extension)
        {
            var path = Path.GetTempPath();
            var fileName = Guid.NewGuid().ToString() + extension;
            return Path.Combine(path, fileName);
        }
    }

    public static class GeneratorExtension
    {
        public static void Generate(this IGenerator generator, TextWriter output, string template, IList<string> friends)
        {
            generator.Generate(template, output, friends);
        }
    }
}
