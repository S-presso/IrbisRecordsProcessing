/* PftPercent.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class PftPercent
        : PftAst
    {
        #region Construction

        public PftPercent(PftParser.PercentNewLineContext node)
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
            context.Output.RemoveEmptyLine();
        }

        #endregion
    }
}