/* PftContext.cs
 */

#region Using directives

using System.Collections.Generic;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Контекст форматирования
    /// </summary>
    public sealed class PftContext
    {
        #region Properties

        public PftContext Parent { get { return _parent; } }

        public ManagedClient64 Client { get; set; }

        public IrbisRecord Record { get; set; }

        public PftOutputBuffer Output { get; internal set; }

        public string Text { get { return Output.ToString(); } }

        public PftMode OutputMode { get; set; }

        public bool UpperMode { get; set; }

        public PftGlobalManager Globals { get; private set; }

        public PftCache Cache { get; set; }

        #endregion

        #region Construction

        public PftContext
            (
                PftContext parent
            )
        {
            _parent = parent;

            PftOutputBuffer parentBuffer = (parent == null)
                ? null
                : parent.Output;

            Output = new PftOutputBuffer(parentBuffer);

            Globals = (parent == null)
                ? new PftGlobalManager()
                : parent.Globals;

            Record = (parent == null)
                ? new IrbisRecord()
                : parent.Record;

            Client = (parent == null)
                ? new ManagedClient64()
                : parent.Client;
        }

        #endregion

        #region Private members

        private readonly PftContext _parent;

        #endregion

        #region Public methods

        public PftContext ClearAll()
        {
            Output.ClearText();
            Output.ClearError();
            Output.ClearWarning();
            return this;
        }

        public PftContext ClearText()
        {
            Output.ClearText();
            return this;
        }

        public PftContext Push()
        {
            PftContext result = new PftContext(this);
            return result;
        }

        public void Pop()
        {
            if (!ReferenceEquals(Parent, null))
            {
                //string warningText = WarningText;
                //if (!string.IsNullOrEmpty(warningText))
                //{
                //    Parent.Output.Warning.Write(warningText);
                //}

                //string errorText = ErrorText;
                //if (!string.IsNullOrEmpty(errorText))
                //{
                //    Parent.Output.Error.Write(errorText);
                //}
            }
        }

        public PftContext Write
            (
                string format,
                params object[] arg
            )
        {
            if (!string.IsNullOrEmpty(format))
            {
                Output.Write(format, arg);
            }
            return this;
        }

        public PftContext Write
            (
                string value
            )
        {
            if (!string.IsNullOrEmpty(value))
            {
                Output.Write(value);
            }
            return this;
        }

        public PftContext WriteLine
            (
                string format,
                params object[] arg
            )
        {
            if (!string.IsNullOrEmpty(format))
            {
                Output.WriteLine(format, arg);
            }
            return this;
        }

        public PftContext WriteLine
            (
                string value
            )
        {
            if (!string.IsNullOrEmpty(value))
            {
                Output.WriteLine(value);
            }
            return this;
        }

        public PftContext WriteLine()
        {
            Output.WriteLine();
            return this;
        }

        public string Evaluate
            (
                PftAst ast
            )
        {
            PftContext copy = Push();
            ast.Execute(copy);
            return copy.ToString();
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Output.ToString();
        }

        #endregion
    }
}