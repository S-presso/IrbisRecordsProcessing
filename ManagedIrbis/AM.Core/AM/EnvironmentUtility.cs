/* EnvironmentUtility.cs -- environment study routines
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM
{
    /// <summary>
    /// Enviromnent study routines.
    /// </summary>
    [Done]
    public static class EnvironmentUtility
    {
        /// <summary>
        /// Gets a value indicating whether system is 32-bit.
        /// </summary>
        /// <value><c>true</c> if system is 32-bit; otherwise,
        /// <c>false</c>.</value>
        public static bool Is32Bit
        {
            [DebuggerStepThrough]
            get
            {
                return ( IntPtr.Size == 4 );
            }
        }

        /// <summary>
        /// Gets a value indicating whether system is 64-bit.
        /// </summary>
        /// <value><c>true</c> if system is 64-bit; otherwise,
        /// <c>false</c>.</value>
        public static bool Is64Bit
        {
            [DebuggerStepThrough]
            get
            {
                return ( IntPtr.Size == 8 );
            }
        }

        /// <summary>
        /// Checks whether application hosted on NT-platform.
        /// </summary>
        public static bool IsNT
        {
            [DebuggerStepThrough]
            get
            {
                return ( Environment.OSVersion.Platform == PlatformID.Win32NT );
            }
        }

        /// <summary>
        /// Checks whether application hosted on Win9x-platform.
        /// </summary>
        public static bool Is9x
        {
            [DebuggerStepThrough]
            get
            {
                return ( Environment.OSVersion.Platform
                         == PlatformID.Win32Windows );
            }
        }

        /// <summary>
        /// Gets a value indicating the central processor unit architecture.
        /// </summary>
        /// <returns><see cref="ProcessorArchitecture"/> enumeration
        /// representing processor architecture.</returns>
        public static ProcessorArchitecture GetProcessorArchitecture ( )
        {
            string architecture = Environment.GetEnvironmentVariable
                ( "PROCESSOR_ARCHITECTURE" );

            if ( string.IsNullOrEmpty ( architecture ) )
            {
                return ProcessorArchitecture.Unknown;
            }

            switch ( architecture.ToUpperInvariant () )
            {
                case "X86":
                    return ProcessorArchitecture.X86;
                case "X64":
                case "AMD64":
                    return ProcessorArchitecture.X64;
                case "IA64":
                    return ProcessorArchitecture.IA64;
                default:
                    return ProcessorArchitecture.Unknown;
            }
        }

        /// <summary>
        /// System uptime.
        /// </summary>
        /// <value></value>
        public static TimeSpan Uptime
        {
            [DebuggerStepThrough]
            get
            {
                return new TimeSpan ( Environment.TickCount );
            }
        }
    }
}
