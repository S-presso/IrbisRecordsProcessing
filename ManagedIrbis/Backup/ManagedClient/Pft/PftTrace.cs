/* PftTrace.cs
 */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Трассировочное сообщение.
    /// Выводится в консоли отладчика
    /// и в зарегистрированных листенерах.
    /// </summary>
    [Serializable]
    public sealed class PftTrace
        : PftAst
    {
        #region Properties

        public PftStatement Statement { get; set; }

        #endregion

        #region Construction

        public PftTrace(PftParser.TraceContext node)
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
            Trace.WriteLine(text);
        }

        #endregion
    }
}