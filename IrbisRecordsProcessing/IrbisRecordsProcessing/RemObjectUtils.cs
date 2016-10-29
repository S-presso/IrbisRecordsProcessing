using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ManagedClient;
using RemObjects.Script;
using RemObjects.Script.EcmaScript;

namespace IrbisRecordsProcessing
{
    public static class RemObjectUtils
    {
        public static T GetUnwrappedVariable<T>
        (
                this EcmaScriptComponent script,
                string variableName
        )
        {
            EcmaScriptObjectWrapper wrapped
            = (EcmaScriptObjectWrapper)script.Globals.GetVariable(variableName);
            if (ReferenceEquals(wrapped, null))
            {
                return default(T);
                // Или бросаемся исключениями
            }
            return (T)wrapped.Value;
        }

        public static void SetMutableVariable
        (
                this EcmaScriptComponent script,
                string variableName,
                object variableValue
        )
        {
            script.Globals.CreateMutableBinding(variableName, false);
            // Задаём значение переменной
            script.Globals.SetMutableBinding(variableName, variableValue, false);
        }

        public static void ExposeAssembly
        (
                EcmaScriptComponent script,
                Assembly assembly
        )
        {
            foreach (Type type in assembly.GetExportedTypes())
            {
                script.ExposeType(type, null);
            }
        }

        public static bool ScriptEval(EcmaScriptComponent script, String expression, string[] argNames, params object[] arguments)
        {
            script.Source = expression;
            bool result = false;
            try
            {
                int index = 0;
                foreach (var arg in arguments)
                    script.Globals.SetVariable(argNames[index++], arg);

                script.Run();
                result = (bool)script.RunResult;
            }
            catch (Exception ex)
            {
            }

            return result;
        }

        public static void ScriptRun(EcmaScriptComponent script, String code, string[] argNames, params object[] arguments)
        {
            if (String.IsNullOrWhiteSpace(code))
                return;

            script.Source = code;
            int index = 0;
            foreach (var arg in arguments)
                if (!script.Globals.GetVariable(argNames[index]).Equals(arg))
                script.Globals.SetVariable(argNames[index++], arg);

            try
            {
                script.Run();
            }
            catch (Exception ex)
            {
                Syncfusion.Windows.Forms.MessageBoxAdv.Show(String.Format("Ошибка выполнения сценария. Строка: {0}, позиция: {1}. Сообщение: {2}",
                                                 script.DebugLastPos.StartRow, script.DebugLastPos.StartCol, ex.ToString()),
                                                 "Внимание", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }           
        }

        public static void ScriptRun(EcmaScriptComponent script, String code)
        {
            ScriptRun(script, code, new string[0], new object[0]);
        }

        public static RecordField[] GetField(IrbisRecord record, String fieldTag)
        {
            return record.Fields.GetField(fieldTag);
        }        
    }
}
