/* PftOutputBuffer.cs
 */

#region Using directives

using System.IO;
using System.Text;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Выходные потоки форматтера.
    /// </summary>
    public sealed class PftOutputBuffer
    {
        #region Properties

        public PftOutputBuffer Parent { get { return _parent; } }

        public TextWriter Normal { get { return _normal; } }

        public TextWriter Warning { get { return _warning; } }

        public TextWriter Error { get { return _error; } }

        public string Text { get { return Normal.ToString(); } }

        public string WarningText { get { return Warning.ToString(); } }

        public string ErrorText { get { return Error.ToString(); } }

        public bool HaveText { get { return _HaveText(_normal); } }

        public bool HaveWarning { get { return _HaveText(_warning); } }

        public bool HaveError { get { return _HaveText(_error); } }
        #endregion

        #region Construction

        public PftOutputBuffer
            (
               PftOutputBuffer parent
            )
        {
            _parent = parent;
            _normal = new StringWriter();
            _warning = new StringWriter();
            _error = new StringWriter();
        }

        #endregion

        #region Private members

        private readonly PftOutputBuffer _parent;

        private readonly StringWriter _normal;
        private readonly StringWriter _warning;
        private readonly StringWriter _error;

        private static bool _HaveText
            (
               StringWriter writer
            )
        {
            return (writer.GetStringBuilder().Length != 0);
        }

        #endregion

        #region Public methods

        public PftOutputBuffer ClearText()
        {
            _normal.GetStringBuilder().Length = 0;
            return this;
        }
        public PftOutputBuffer ClearWarning()
        {
            _warning.GetStringBuilder().Length = 0;
            return this;
        }

        public PftOutputBuffer ClearError()
        {
            _error.GetStringBuilder().Length = 0;
            return this;
        }

        public PftOutputBuffer Push()
        {
            PftOutputBuffer result = new PftOutputBuffer(this);
            return result;
        }

        public string Pop()
        {
            if (!ReferenceEquals(Parent, null))
            {
                string warningText = WarningText;
                if (!string.IsNullOrEmpty(warningText))
                {
                    Parent.Warning.Write(warningText);
                }

                string errorText = ErrorText;
                if (!string.IsNullOrEmpty(errorText))
                {
                    Parent.Error.Write(errorText);
                }
            }

            return ToString();
        }

        public PftOutputBuffer Write
            (
                string format,
                params object[] arg
            )
        {
            if (!string.IsNullOrEmpty(format))
            {
                Normal.Write(format, arg);
            }
            return this;
        }

        public PftOutputBuffer Write
            (
                string value
            )
        {
            if (!string.IsNullOrEmpty(value))
            {
                Normal.Write(value);
            }
            return this;
        }

        public PftOutputBuffer WriteLine
            (
                string format,
                params object[] arg
            )
        {
            if (!string.IsNullOrEmpty(format))
            {
                Normal.WriteLine(format, arg);
            }
            return this;
        }

        public PftOutputBuffer WriteLine
            (
               string value
            )
        {
            if (!string.IsNullOrEmpty(value))
            {
                Normal.WriteLine(value);
            }
            return this;
        }

        public PftOutputBuffer WriteLine()
        {
            Normal.WriteLine();
            return this;
        }

        public int GetCaretPosition()
        {
            StringBuilder builder = _normal.GetStringBuilder();
            int pos;
            for (pos = builder.Length - 1; pos >= 0; pos--)
            {
                if (builder[pos] == '\n')
                    break;
            }
            return (builder.Length - pos);
        }

        public void RemoveEmptyLine()
        {
            StringBuilder builder = _normal.GetStringBuilder();
            int pos;
            for (pos = builder.Length - 1; pos >= 0; pos-- )
            {
                if (!char.IsWhiteSpace(builder[pos]))
                {
                    break;
                }
                builder.Length = pos;
            }
        }

        public bool HaveEmptyLine()
        {
            StringBuilder builder = _normal.GetStringBuilder();
            bool result = true;
            int pos;
            for (pos = builder.Length - 1; pos >= 0; pos--)
            {
                char c = builder[pos];
                if (c == '\n')
                {
                    break;
                }
                if (!char.IsWhiteSpace(c))
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Normal.ToString();
        }

        #endregion
    }
}