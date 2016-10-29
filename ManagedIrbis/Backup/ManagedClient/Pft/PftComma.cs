using System;

namespace ManagedClient.Pft
{
    /// <summary>
    /// Запятая.
    /// Пустой оператор. 
    /// Может быть безболезненно удалён в большинстве случаев.
    /// </summary>
    [Serializable]
    public sealed class PftComma
        : PftAst
    {
        #region Construction

        public PftComma(PftParser.CommaContext node)
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
            // Really nothing to do!
        }

        #endregion
    }
}