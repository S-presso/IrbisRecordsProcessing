/* PftSlash.cs
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
    public sealed class PftSlash
        : PftAst
    {
        #region Construction

        public PftSlash(PftParser.SlashNewLineContext node)
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
            if (!context.Output.HaveEmptyLine())
            {
                context.WriteLine();
            }
        }

        #endregion
    }
}