/* PftCommandX.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    [Serializable]
    public sealed class PftCommandX
        : PftAst
    {
        #region Properties

        public int Position { get; set; }

        #endregion

        #region Construction

        public PftCommandX(PftParser.CommandXContext node)
            : base(node)
        {
            Position = int.Parse(node.COMMANDX().GetText().Substring(1));
        }

        #endregion

        #region PftAst members

        public override void Execute
            (
                PftContext context
            )
        {
            if (Position > 0)
            {
                context.Write(new string(' ', Position));
            }
        }

        #endregion
    }
}