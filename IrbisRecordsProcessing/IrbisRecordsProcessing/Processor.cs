using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrinterPlusPlusSDK;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace IrbisRecordsProcessing
{
    public class Processor : PrinterPlusPlusSDK.IProcessor
    {
        public PrinterPlusPlusSDK.ProcessResult Process(string key, string psFilename)
        {
            //Convert PS to Text
            var txtFilename = System.IO.Path.GetTempFileName();
            ConvertPsToTxt(psFilename, txtFilename);
            
            //Process result data
            ProcessResult(System.IO.File.OpenText(txtFilename));

            return new ProcessResult();
        }

        public static void ProcessResult(System.IO.StreamReader sr)
        {
            //Process the converted Text Stream
            var extractedValue = ProcessTextStream(sr);

            //Process records
            ProcessRecords(extractedValue); 
        }

        private static string ConvertPsToTxt(string psFilename, string txtFilename)
        {
            var retVal = string.Empty;
            var errorMessage = string.Empty;
            var command = "C:\\ps2txt\\ps2txt.exe";
            var args = string.Format("-nolayout \"{0}\" \"{1}\"", psFilename, txtFilename);
            retVal = Shell.ExecuteShellCommand(command, args, ref errorMessage);
            return retVal;
        }

        private static ExtractedValue ProcessTextStream(System.IO.StreamReader sr)
        {
            var values = new ExtractedValue();      //Create the extracted values placeholders
            string [] strMarkers = { "DBN:", "MFN:" };
            string strValue;
            List<int> MfnList = new List<int>();

            //Read the text file
            using (sr)
            {
                while (sr.Peek() > -1)
                {
                    var currentLine = sr.ReadLine().Trim();

                    //Skip whitespaces
                    if (string.IsNullOrWhiteSpace(currentLine))
                        continue;

                    for (int index = 0; index < strMarkers.Length; index++)
                    {
                        string strMarker = strMarkers [index]; 
                        if (!currentLine.StartsWith(strMarker))
                            continue;

                        strValue = currentLine.Substring(strMarker.Length);
                        switch (index)
                        {
                            case 0:
                                values.DBName = strValue;
                                break;
                            case 1:
                                MfnList.Add(Convert.ToInt32(strValue));                                
                                break;
                        }

                    }
                }
            }
            MfnList.Sort();
            values.MfnList = MfnList.ToArray();
            return values;      //Return the extracted values
        }

        private static void ProcessRecords(ExtractedValue recordsData)
        {
            /*Microsoft.VisualBasic.Interaction.MsgBox(String.Format("Database: {0}", recordsData.DBName));
            foreach (int Mfn in recordsData.MfnList)
                Microsoft.VisualBasic.Interaction.MsgBox(String.Format("Mfn: {0}", Mfn));*/


            ShowScenarioFindReplaceDialog(recordsData);            
        }

        private static void ShowScenarioFindReplaceDialog(ExtractedValue recordsData)
        {
            FindReplaceScriptEditor find_replaceScriptEditor = new FindReplaceScriptEditor(recordsData);
            if (find_replaceScriptEditor.ShowDialog() == DialogResult.OK)
            {
                ScenarioFindReplaceResultsForm scenarioFindReplaceResultsForm = new ScenarioFindReplaceResultsForm(find_replaceScriptEditor);
                scenarioFindReplaceResultsForm.ShowDialog();                
            }
            if (find_replaceScriptEditor.client != null)
                find_replaceScriptEditor.client.Disconnect();
        }
    }    
}
