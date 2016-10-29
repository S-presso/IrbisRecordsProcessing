/* PftBreak.cs
 */

#region Using directives

using System;

using Antlr4.Runtime.Tree;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Прерывание обработки повторяющейся группы
    /// </summary>
    [Serializable]
    public sealed class PftBreak
        : PftGroupItem
    {
        #region Properties

        #endregion

        #region Construciton

        public PftBreak(PftParser.BreakContext node) 
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
            if (Group != null)
            {
                Group.BreakEncountered = true;
            }
        }

        #endregion
    }
}
