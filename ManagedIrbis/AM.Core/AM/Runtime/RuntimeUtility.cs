/* RuntimeUtility.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Diagnostics;
using System.IO;

#endregion

namespace AM.Runtime
{
    /// <summary>
    /// Some usefule methods for runtime.
    /// </summary>
    public static class RuntimeUtility
    {
        #region Properties

        /// <summary>
        /// Путь к файлам текущей версии Net Framework.
        /// </summary>
        /// <remarks>
        /// Типичная выдача:
        /// C:\WINDOWS\Microsoft.NET\Framework\v2.0.50215
        /// </remarks>
        public static string FrameworkLocation
        {
            get
            {
                return Path.GetDirectoryName
                    ( typeof ( int ).Assembly.Location );
            }
        }

        /// <summary>
        /// Имя исполняемого процесса.
        /// </summary>
        public static string ExecutableFileName
        {
            get
            {
                Process process = Process.GetCurrentProcess ();
                ProcessModule module = process.MainModule;
                return module.FileName;
            }
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        #endregion
    }
}
