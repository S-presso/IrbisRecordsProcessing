/* VisitInfo.cs
 */

#region Using directives

using System;
using System.Text;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// Информация о посещении.
    /// </summary>
    [Serializable]
    public sealed class VisitInfo
    {
        #region Properties

        /// <summary>
        /// подполе G, имя БД каталога.
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// подполе A, шифр документа.
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// подполе B, инвентарный номер экземпляра
        /// </summary>
        public string Inventory { get; set; }

        /// <summary>
        /// подполе H, штрих-код экземпляра.
        /// </summary>
        public string Barcode { get; set; }

        /// <summary>
        /// подполе K, место хранения экземпляра
        /// </summary>
        public string Sigla { get; set; }

        /// <summary>
        /// подполе D, дата выдачи
        /// </summary>
        public string DateGivenString { get; set; }

        /// <summary>
        /// подполе V, место выдачи
        /// </summary>
        public string Department { get; set; }

        /// <summary>
        /// подполе E, дата предполагаемого возврата
        /// </summary>
        public string DateExpectedString { get; set; }

        /// <summary>
        /// подполе F, дата фактического возврата
        /// </summary>
        public string DateReturnedString { get; set; }

        /// <summary>
        /// подполе L, дата продления
        /// </summary>
        public string DateProlongString { get; set; }

        /// <summary>
        /// подполе U, признак утерянной книги
        /// </summary>
        public string Lost { get; set; }

        /// <summary>
        /// подполе C, краткое библиографическое описание
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// подполе I, ответственное лицо
        /// </summary>
        public string Responsible { get; set; }

        /// <summary>
        /// подполе 1, время начала визита в библиотеку
        /// </summary>
        public string TimeIn { get; set; }

        /// <summary>
        /// подполе 2, время окончания визита в библиотеку
        /// </summary>
        public string TimeOut { get; set; }

        /// <summary>
        /// Не посещение ли?
        /// </summary>
        public bool IsVisit
        {
            get { return string.IsNullOrEmpty(Index); }
        }

        /// <summary>
        /// Возвращена ли книга?
        /// </summary>
        public bool IsReturned
        {
            get
            {
                return !string.IsNullOrEmpty(DateReturnedString)
                       && !DateReturnedString.StartsWith("*");
            }
        }

        public DateTime DateGiven
        {
            get
            {
                return DateGivenString.ParseIrbisDate();
            }
        }

        public DateTime DateReturned
        {
            get
            {
                return DateReturnedString.ParseIrbisDate();
            }
        }

        public DateTime DateExpected
        {
            get
            {
                return DateExpectedString.ParseIrbisDate();
            }
        }

        #endregion

        #region Private members

        private static string FM
            (
                RecordField field,
                char code
            )
        {
            return field.GetSubFieldText(code, 0);
        }

        #endregion

        #region Public methods

        public static VisitInfo Parse(RecordField field)
        {
            VisitInfo result = new VisitInfo
            {
                Database = FM(field, 'g'),
                Index = FM(field, 'a'),
                Inventory = FM(field, 'b'),
                Barcode = FM(field, 'h'),
                Sigla = FM(field, 'k'),
                DateGivenString = FM(field, 'd'),
                Department = FM(field, 'v'),
                DateExpectedString = FM(field, 'e'),
                DateReturnedString = FM(field, 'f'),
                DateProlongString = FM(field, 'l'),
                Lost = FM(field, 'u'),
                Description = FM(field, 'c'),
                Responsible = FM(field, 'i'),
                TimeIn = FM(field, '1'),
                TimeOut = FM(field, '2')
            };

            return result;
        }

        #endregion

        #region Object members

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result
                .AppendFormat("Посещение: \t\t\t{0}", IsVisit)
                .AppendLine()
                .AppendFormat("Описание: \t\t\t{0}", Description)
                .AppendLine()
                .AppendFormat("Шифр документа: \t\t{0}", Index)
                .AppendLine()
                .AppendFormat("Штрих-код: \t\t\t{0}", Barcode)
                .AppendLine()
                .AppendFormat("Место хранения: \t\t{0}", Sigla)
                .AppendLine()
                .AppendFormat("Дата выдачи: \t\t\t{0:d}", DateGiven)
                .AppendLine()
                .AppendFormat("Место выдачи: \t\t\t{0}", Department)
                .AppendLine()
                .AppendFormat("Ответственное лицо: \t\t{0}", Responsible)
                .AppendLine()
                .AppendFormat("Дата предполагаемого возврата: \t{0:d}", DateExpected)
                .AppendLine()
                .AppendFormat("Возвращена: \t\t\t{0}", IsReturned)
                .AppendLine()
                .AppendFormat("Дата возврата: \t\t\t{0:d}", DateReturned)
                .AppendLine()
                .AppendLine(new string('-', 60));

            return result.ToString();
        }

        #endregion
    }
}
