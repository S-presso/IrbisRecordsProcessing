/* PftSimpleMfn.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// —сылка на MFN записи (без указани€ длины).
    /// </summary>
    [Serializable]
    public sealed class PftSimpleMfn
        : PftAst
    {
        #region Construction

        public PftSimpleMfn(PftParser.SimpleMfnContext node)
            : base(node)
        {
        }

        #endregion

        #region PftAst members

        public override void Execute
            (
                PftContext context
            )
        {
            string format = new string('0', 10);
            string text = context.Record.Mfn.ToString(format);
            context.Write(text);
        }

        #endregion
    }
}