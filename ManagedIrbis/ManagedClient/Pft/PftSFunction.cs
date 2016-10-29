/* PftSFunction.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    [Serializable]
    public sealed class PftSFunction
        : PftAst
    {
        #region Properties

        public PftNonGrouped Arguments { get; set; }

        #endregion

        #region Construction

        public PftSFunction(PftParser.SFunctionContext node)
            : base(node)
        {
            Arguments = new PftNonGrouped(node.nonGrouped());
        }

        #endregion

        #region PftAst members

        public override void Execute
            (
                PftContext context
            )
        {
            Arguments.Execute(context);
        }

        #endregion

        #region Public methods

        public string Evaluate
            (
                PftContext context
            )
        {
            string result = context.Evaluate(this);
            return result;
        }

        #endregion
    }
}
