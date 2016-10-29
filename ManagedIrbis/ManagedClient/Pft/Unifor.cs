/* Unifor.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ManagedClient.Pft
{
    public sealed class Unifor
    {
        #region Properties

        public PftContext Context { get; set; }

        #endregion

        #region Construction

        public Unifor()
        {
        }

        public Unifor
            (
                PftContext context
            )
        {
            Context = context;
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        public string Evaluate
            (
                string format
            )
        {
            if (string.IsNullOrEmpty(format))
            {
                return string.Empty;
            }

            char firstLetter = char.ToUpperInvariant(format[0]);
            char secondLetter = '\0';
            if (format.Length > 1)
            {
                secondLetter = char.ToUpperInvariant(format[1]);
            }
            format = format.Substring(1);
            
            StringBuilder result = new StringBuilder();

            switch (firstLetter)
            {
                case '3':
                {
                    DateTime now = DateTime.Now;
                    switch (secondLetter)
                    {
                        case '0':
                            result.AppendFormat("{0:yyyy}", now);
                            break;
                        case '1':
                            result.AppendFormat("{0:MM}", now);
                            break;
                        case '2':
                            result.AppendFormat("{0:dd}", now);
                            break;
                        case '3':
                            result.AppendFormat("{0:yy}", now);
                            break;
                        case '4':
                            result.AppendFormat("{0:M}", now);
                            break;
                        case '5':
                            result.AppendFormat("{0:d}", now);
                            break;
                        case '9':
                            result.AppendFormat("{0:T}", now);
                            break;
                        default:
                            result.AppendFormat("{0:yyyyMMdd}", now);
                            break;
                    }
                }
                    break;
                case '9':
                    result.Append(format.Replace("\"", string.Empty));
                    break;
                case 'Q':
                    result.Append(format.ToLowerInvariant());
                    break;
            }



            return result.ToString();
        }

        #endregion
    }
}
