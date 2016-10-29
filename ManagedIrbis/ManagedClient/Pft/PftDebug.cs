/* PftDebug.cs
 */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Отладочное сообщение (выводится в консоли отладчика).
    /// </summary>
    [Serializable]
    public sealed class PftDebug
        : PftAst
    {
        #region Properties

        public PftStatement Statement { get; set; }

        #endregion

        #region Construction

        public PftDebug(PftParser.DebugContext node)
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
            Debug.WriteLine(text);
        }

        #endregion
    }
}