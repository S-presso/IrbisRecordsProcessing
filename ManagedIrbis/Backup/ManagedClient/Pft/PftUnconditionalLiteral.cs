/* PftUnconditionalLiteral.cs
 */

#region Using directives

using System;

using Antlr4.Runtime;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Безусловный литерал
    /// </summary>
    [Serializable]
    public sealed class PftUnconditionalLiteral
        : PftAst
    {
        #region Properties

        public string RealText { get; set; }

        #endregion

        #region Construction

        public PftUnconditionalLiteral(PftParser.UnconditionalLiteralContext node)
            : base(node)
        {
            RealText = node.UNCONDITIONAL().GetText();
            if (!string.IsNullOrEmpty(RealText))
            {
                RealText = RealText.Substring(1, RealText.Length - 2);
            }
        }

        public PftUnconditionalLiteral(IToken token)
        {
            RealText = token.Text;
            if (!string.IsNullOrEmpty(RealText))
            {
                RealText = RealText.Substring(1, RealText.Length - 2);
            }
        }

        #endregion

        #region PftAst members

        public override void Execute
            (
                PftContext context
            )
        {
            context.Write(RealText);
        }

        #endregion
    }
}