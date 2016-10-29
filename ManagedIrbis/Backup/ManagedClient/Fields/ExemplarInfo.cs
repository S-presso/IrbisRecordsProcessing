/* ExemplarInfo.cs
 */

#region Using directives

using System;

using ManagedClient.Mapping;

#endregion

namespace ManagedClient.Fields
{
    /// <summary>
    /// Экземпляры (поле 910).
    /// </summary>
    [Serializable]
    public sealed class ExemplarInfo
    {
        #region Properties

        /// <summary>
        /// Статус. Подполе a.
        /// </summary>
        [SubField('a')]
        public string Status { get; set; }

        /// <summary>
        /// Инвентарный номер. Подполе b.
        /// </summary>
        [SubField('b')]
        public string Number { get; set; }

        /// <summary>
        /// Дата поступления. Подполе c.
        /// </summary>
        [SubField('c')]
        public string Date { get; set; }

        /// <summary>
        /// Место хранения. Подполе d.
        /// </summary>
        [SubField('d')]
        public string Place { get; set; }

        /// <summary>
        /// Наименование коллекции. Подполе q.
        /// </summary>
        [SubField('q')]
        public string Collection { get; set; }

        /// <summary>
        /// Расстановочный шифр. Подполе r.
        /// </summary>
        [SubField('r')]
        public string ShelfIndex { get; set; }

        /// <summary>
        /// Цена экземпляра. Подполе e.
        /// </summary>
        [SubField('e')]
        public string Price { get; set; }

        /// <summary>
        /// Штрих-код/радиометка. Подполе h.
        /// </summary>
        [SubField('h')]
        public string Barcode { get; set; }

        /// <summary>
        /// Число экземпляров. Подполе 1.
        /// </summary>
        [SubField('1')]
        public string Amount { get; set; }

        /// <summary>
        /// Специальное назначение фонда. Подполе t.
        /// </summary>
        [SubField('t')]
        public string Purpose { get; set; }

        /// <summary>
        /// Коэффициент многоразового использования. Подполе =.
        /// </summary>
        [SubField('=')]
        public string Coefficient { get; set; }

        /// <summary>
        /// Экземпляры не на баланс. Подполе 4.
        /// </summary>
        [SubField('4')]
        public string OffBalance { get; set; }

        /// <summary>
        /// Номер записи КСУ. Подполе u.
        /// </summary>
        [SubField('u')]
        public string KsuNumber1 { get; set; }

        /// <summary>
        /// Номер акта. Подполе y.
        /// </summary>
        [SubField('y')]
        public string ActNumber1 { get; set; }

        /// <summary>
        /// Канал поступления. Подполе f.
        /// </summary>
        [SubField('f')]
        public string Channel { get; set; }

        /// <summary>
        /// Число выданных экземпляров. Подполе 2.
        /// </summary>
        [SubField('2')]
        public string OnHand { get; set; }

        /// <summary>
        /// Номер акта списания. Подполе v.
        /// </summary>
        [SubField('v')]
        public string ActNumber2 { get; set; }

        /// <summary>
        /// Количество списываемых экземпляров. Подполе x.
        /// </summary>
        [SubField('x')]
        public string WriteOff { get; set; }

        /// <summary>
        /// Количество экземпляров для докомплектования. Подполе k.
        /// </summary>
        [SubField('k')]
        public string Completion { get; set; }

        /// <summary>
        /// Номер акта передачи в другое подразделение. Подполе w.
        /// </summary>
        [SubField('w')]
        public string ActNumber3 { get; set; }

        /// <summary>
        /// Количество передаваемых экземпляров. Подполе z.
        /// </summary>
        [SubField('z')]
        public string Moving { get; set; }

        /// <summary>
        /// Нове место хранения. Подполе m.
        /// </summary>
        [SubField('m')]
        public string NewPlace { get; set; }

        /// <summary>
        /// Дата проверки фонда. Подполе s.
        /// </summary>
        [SubField('s')]
        public string CheckedDate { get; set; }

        /// <summary>
        /// Число проверенных экземпляров. Подполе 0.
        /// </summary>
        [SubField('0')]
        public string CheckedAmount { get; set; }

        /// <summary>
        /// Реальное место нахождения книги. Подполе !.
        /// </summary>
        [SubField('!')]
        public string RealPlace { get; set; }

        #endregion

        #region Public methods

        public static ExemplarInfo Parse
            (
                RecordField field
            )
        {
            ExemplarInfo result = new ExemplarInfo
                {
                    Status = field.GetSubFieldText ( 'a', 0 ),
                    Number = field.GetSubFieldText ( 'b', 0 ),
                    Date = field.GetSubFieldText ( 'c', 0 ),
                    Place = field.GetSubFieldText ( 'd', 0 ),
                    Collection = field.GetSubFieldText ( 'q', 0 ),
                    ShelfIndex = field.GetSubFieldText ( 'r', 0 ),
                    Price = field.GetSubFieldText ( 'e', 0 ),
                    Barcode = field.GetSubFieldText ( 'h', 0 ),
                    Amount = field.GetSubFieldText ( '1', 0 ),
                    Purpose = field.GetSubFieldText ( 't', 0 ),
                    Coefficient = field.GetSubFieldText ( '=', 0 ),
                    OffBalance = field.GetSubFieldText ( '4', 0 ),
                    KsuNumber1 = field.GetSubFieldText ( 'u', 0 ),
                    ActNumber1 = field.GetSubFieldText ( 'y', 0 ),
                    Channel = field.GetSubFieldText ( 'f', 0 ),
                    OnHand = field.GetSubFieldText ( '2', 0 ),
                    ActNumber2 = field.GetSubFieldText ( 'v', 0 ),
                    WriteOff = field.GetSubFieldText ( 'x', 0 ),
                    Completion = field.GetSubFieldText ( 'k', 0 ),
                    ActNumber3 = field.GetSubFieldText ( 'w', 0 ),
                    Moving = field.GetSubFieldText ( 'z', 0 ),
                    NewPlace = field.GetSubFieldText ( 'm', 0 ),
                    CheckedDate = field.GetSubFieldText ( 's', 0 ),
                    CheckedAmount = field.GetSubFieldText ( '0', 0 ),
                    RealPlace = field.GetSubFieldText ( '!', 0 )
                };
            return result;
        }

        public RecordField ToField ()
        {
            RecordField result = new RecordField("910")
                .AddNonEmptySubField ( 'a', Status )
                .AddNonEmptySubField ( 'b', Number )
                .AddNonEmptySubField ( 'c', Date )
                .AddNonEmptySubField ( 'd', Place )
                .AddNonEmptySubField ( 'q', Collection )
                .AddNonEmptySubField ( 'r', ShelfIndex )
                .AddNonEmptySubField ( 'e', Price )
                .AddNonEmptySubField ( 'h', Barcode )
                .AddNonEmptySubField ( '1', Amount )
                .AddNonEmptySubField ( 't', Purpose )
                .AddNonEmptySubField ( '=', Coefficient )
                .AddNonEmptySubField ( '4', OffBalance )
                .AddNonEmptySubField ( 'u', KsuNumber1 )
                .AddNonEmptySubField ( 'y', ActNumber1 )
                .AddNonEmptySubField ( 'f', Channel )
                .AddNonEmptySubField ( '2', OnHand )
                .AddNonEmptySubField ( 'v', ActNumber2 )
                .AddNonEmptySubField ( 'x', WriteOff )
                .AddNonEmptySubField ( 'k', Completion )
                .AddNonEmptySubField ( 'w', ActNumber3 )
                .AddNonEmptySubField ( 'z', Moving )
                .AddNonEmptySubField ( 'm', NewPlace )
                .AddNonEmptySubField ( 's', CheckedDate )
                .AddNonEmptySubField ( '0', CheckedAmount )
                .AddNonEmptySubField ( '!', RealPlace );
            return result;
        }

        #endregion
    }
}
