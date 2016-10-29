/* PftConditionString.cs
 */

#region Using directives

using System;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Сравнение строк в условии
    /// </summary>
    [Serializable]
    public sealed class PftConditionString
        : PftCondition
    {
        #region Properties

        /// <summary>
        /// Ссылка на поле.
        /// </summary>
        public PftFieldReference Field { get; set; }

        /// <summary>
        /// Безусловный литерал.
        /// </summary>
        public PftUnconditionalLiteral Unconditional { get; set; }

        /// <summary>
        /// Обратный порядок операндов.
        /// </summary>
        public bool Reverse { get; set; }

        /// <summary>
        /// Сравнение на равенство, а не на включение.
        /// </summary>
        public bool Equals { get; set; }

        #endregion

        #region Construction

        public PftConditionString(PftParser.ConditionStringContext node)
            : base(node)
        {
            // ReSharper disable CanBeReplacedWithTryCastAndCheckForNull
            PftParser.StringTestContext test = node.stringTest();

            if (test is PftParser.StringTestDirectContext)
            {
                Equals = ((PftParser.StringTestDirectContext) test).EQUALS() != null;
                Field = new PftFieldReference(((PftParser.StringTestDirectContext)test).left);
                Unconditional = new PftUnconditionalLiteral(((PftParser.StringTestDirectContext)test).right);
            }
            else if (test is PftParser.StringTestReverseContext)
            {
                Equals = ((PftParser.StringTestReverseContext)test).EQUALS() != null;
                Field = new PftFieldReference(((PftParser.StringTestReverseContext)test).right);
                Unconditional = new PftUnconditionalLiteral(((PftParser.StringTestReverseContext)test).left);
            }
            else
            {
                throw new ArgumentException();
            }
            // ReSharper restore CanBeReplacedWithTryCastAndCheckForNull
        }

        #endregion

        #region PftCondition members

        public override bool Evaluate
            (
                PftContext context
            )
        {
            string fieldText = Field.Evaluate(context).ToUpperInvariant();
            string unconditionalText = Unconditional.RealText.ToUpperInvariant();

            bool result = Equals
                ? ( fieldText == unconditionalText )
                : (
                    Reverse
                    ? unconditionalText.Contains(fieldText)
                    : fieldText.Contains(unconditionalText)
                  );

            return result;
        }

        #endregion
    }
}