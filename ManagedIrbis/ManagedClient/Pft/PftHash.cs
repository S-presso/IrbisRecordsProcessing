/* PftHash.cs
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
    public sealed class PftHash
        : PftAst
    {
        #region Construction

        public PftHash(PftParser.HashNewLineContext node)
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
            context.WriteLine();
        }

        #endregion
    }
}