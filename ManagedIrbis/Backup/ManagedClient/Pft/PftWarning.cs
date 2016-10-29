/* PftWarning.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Предупреждение.
    /// Выводится в особый поток контекста.
    /// </summary>
    [Serializable]
    public sealed class PftWarning
        : PftAst
    {
        #region Properties

        public PftStatement Statement { get; set; }

        #endregion

        #region Construction

        public PftWarning(PftParser.WarningContext node)
            : base(node)
        {
            Statement = new PftStatement(node.statement());
        }

        #endregion

        #region PftAst members

        public override void Execute
            (
                PftContext context
            )
        {
            string text = context.Evaluate(Statement);
            context.Output.Warning.WriteLine(text);
        }

        #endregion
    }
}