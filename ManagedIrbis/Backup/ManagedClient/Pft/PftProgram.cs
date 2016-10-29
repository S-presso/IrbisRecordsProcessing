/* PftProgram.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Верхний элемент дерева разбора – собственно программа.
    /// </summary>
    [Serializable]
    public sealed class PftProgram
        : PftAst
    {
        #region Construction

        public PftProgram(PftParser.ProgramContext node)
            : base(node)
        {
            // Программа состоит из последовательности операторов
            foreach (PftParser.StatementContext statementContext 
                in node.statement())
            {
                Children.Add(new PftStatement(statementContext));
            }
        }

        #endregion

        #region PftAst members

        #endregion
    }
}