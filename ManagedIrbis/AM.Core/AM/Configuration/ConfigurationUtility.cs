/* ConfigurationUtility.cs -- some useful routines for System.Configuration
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Globalization;

using AM.Runtime;

using CM = System.Configuration.ConfigurationManager;

#endregion

namespace AM.Configuration
{
    /// <summary>
    /// Some useful routines for System.Configuration.
    /// </summary>
    public static class ConfigurationUtility
    {
        #region Private members

        private static IFormatProvider _FormatProvider
        {
            get
            {
                return CultureInfo.InvariantCulture;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Application.exe.config file name with full path.
        /// </summary>
        public static string ConfigFileName
        {
            get
            {
                return string.Concat
                    (
                     RuntimeUtility.ExecutableFileName,
                     ".config" );
            }
        }

        /// <summary>
        /// Get boolean value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool GetBoolean
            (
            string key,
            bool defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = ConversionUtility.ToBoolean ( s );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get 16-bit integer value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short GetInt16
            (
            string key,
            short defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = short.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get 32-bit integer value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int GetInt32
            (
            string key,
            int defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = int.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get 64-bit integer value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long GetInt64
            (
            string key,
            long defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = long.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get single-precision float value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float GetSingle
            (
            string key,
            float defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = float.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get double-precision float value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double GetDouble
            (
            string key,
            double defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = double.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get decimal value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal GetDecimal
            (
            string key,
            decimal defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = decimal.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        /// Get string value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetString
            (
            string key,
            string defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = s;
            }
            return defaultValue;
        }

        /// <summary>
        /// Get date or time value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime GetDateTime
            (
            string key,
            DateTime defaultValue )
        {
            string s = CM.AppSettings [ key ];
            if ( !string.IsNullOrEmpty ( s ) )
            {
                defaultValue = DateTime.Parse
                    (
                     s,
                     _FormatProvider );
            }
            return defaultValue;
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static bool Get
            (
            string key,
            bool defaultValue )
        {
            return GetBoolean
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static short Get
            (
            string key,
            short defaultValue )
        {
            return GetInt16
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int Get
            (
            string key,
            int defaultValue )
        {
            return GetInt32
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long Get
            (
            string key,
            long defaultValue )
        {
            return GetInt64
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static float Get
            (
            string key,
            float defaultValue )
        {
            return GetSingle
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static double Get
            (
            string key,
            double defaultValue )
        {
            return GetDouble
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal Get
            (
            string key,
            decimal defaultValue )
        {
            return GetDecimal
                (
                 key,
                 defaultValue );
        }

        /// <summary>
        ///  Get value from application configuration.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime Get
            (
            string key,
            DateTime defaultValue )
        {
            return GetDateTime
                (
                 key,
                 defaultValue );
        }

        #endregion
    }
}
