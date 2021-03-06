/* ExceptionUtility.cs -- exception helpers.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Globalization;

#endregion

namespace AM
{
    /// <summary>
    /// Some exception helpers.
    /// </summary>
    /// <example>
    /// <para>Here is simple example of <see cref="T:AM.ExceptionUtility"/>
    /// usage.</para>
    /// <code>
    /// public double ComputePercent ( double amount, double percent )
    /// {
    ///		AM.ExceptionUtility.ThrowIf ( percent &gt; 100.0, "percent too large" );
    ///		AM.ExceptionUtility.ThrowIf ( percent &lt; 0.0, "percent too small" );
    ///		return ( amount * percent / 100.0 );
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="T:AM.ArgumentUtility"/>
    public static class ExceptionUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Throws given exception when specified condition is true.
        /// </summary>
        /// <typeparam name="T">Type of exception to throw.
        /// </typeparam>
        /// <param name="condition">Condition to check.</param>
        /// <param name="format">Format string for exception
        /// message.</param>
        /// <param name="args">Arguments for 
        /// <paramref name="format"/> string to embed into
        /// message string.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="format"/> is <c>null</c>.
        /// </exception>
        public static void ThrowIf < T >
            (
            bool condition,
            string format,
            params object[] args ) where T : Exception
        {
            ArgumentUtility.NotNull
                (
                 format,
                 "format" );

            if ( condition )
            {
                string message = string.Format
                    (
                     CultureInfo.CurrentCulture,
                     format,
                     args );
                T exception = (T) Activator.CreateInstance
                                      (
                                       typeof ( T ),
                                       message );
                throw exception;
            }
        }

        /// <summary>
        /// Throws given exception when specified condition is true.
        /// </summary>
        /// <typeparam name="T">Type of exception to throw.
        /// </typeparam>
        /// <param name="condition">Condition to check.
        /// </param>
        /// <param name="message">Message to show.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="message"/> is <c>null</c>.
        /// </exception>
        public static void ThrowIf < T >
            (
            bool condition,
            string message ) where T : Exception
        {
            ArgumentUtility.NotNull
                (
                 message,
                 "message" );
            if ( condition )
            {
                T exception = (T) Activator.CreateInstance
                                      (
                                       typeof ( T ),
                                       message );
                throw exception;
            }
        }

        #endregion
    }
}
