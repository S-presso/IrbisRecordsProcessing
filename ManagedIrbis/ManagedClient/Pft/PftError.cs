/* PftError.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Сообщение об ошибке.
    /// Выводится в специальный поток
    /// контекста форматирования.
    /// </summary>
    [Serializable]
    public sealed class PftError
        : PftAst
    {
        #region Properties

        /// <summary>
        /// Команды, формирующие сообщение об ошибке.
        /// </summary>
        public PftStatement Statement { get; set; }

        #endregion

        #region Construction

        public PftError(PftParser.ErrorContext node)
            : base (node)
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
        }

        #endregion
    }
}