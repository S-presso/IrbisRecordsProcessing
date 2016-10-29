/* PftCommandC.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    [Serializable]
    public sealed class PftCommandC
        : PftAst
    {
        #region Properties

        public int Position { get; set; }

        #endregion

        #region Construction

        public PftCommandC(PftParser.CommandCContext node)
            : base(node)
        {
            Position = int.Parse(node.COMMANDC().GetText().Substring(1));
        }

        #endregion

        #region PftAst members

        public override void Execute
            (
                PftContext context
            )
        {
            int desired = Position*8;
            int current = context.Output.GetCaretPosition();
            int delta = desired - current;
            if (delta > 0)
            {
                context.Write(new string(' ', delta));
            }
        }

        #endregion
    }
}