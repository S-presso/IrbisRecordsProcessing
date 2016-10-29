/* PftFatal.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Фатальная ошибка.
    /// </summary>
    [Serializable]
    public sealed class PftFatal
        : PftAst
    {
        #region Properties

        public PftStatement Statement { get; set; }

        #endregion

        #region Construction

        public PftFatal(PftParser.FatalContext node)
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
            context.Output.Error.WriteLine(text);
            throw new ApplicationException(text);
        }

        #endregion
    }
}