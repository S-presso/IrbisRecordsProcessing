/* SubFieldCollection.cs -- ������ �������� ���� �������� ����������.
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// ��������� ��������.
    /// ���������� ���, ��� ������������� �� ���������
    /// �������� <c>null</c>.
    /// </summary>
    [Serializable]
    public sealed class SubFieldCollection
        : Collection<SubField>
    {
        #region Public methods

        public void AddRange
            (
                IEnumerable<SubField> subFields
            )
        {
            foreach (SubField subField in subFields)
            {
                Add(subField);
            }
        }

        public SubField Find
            (
                Predicate<SubField> predicate
            )
        {
            return this
                .FirstOrDefault(subField => predicate(subField));
        }

        public SubField[] FindAll
            (
                Predicate<SubField> predicate
            )
        {
            return this
                .Where(subField => predicate(subField))
                .ToArray();
        }

        #endregion

        #region Collection<T> members

        protected override void InsertItem
            (
                int index,
                SubField item
            )
        {
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item");
            }

            base.InsertItem(index, item);
        }

        protected override void SetItem
            (
                int index,
                SubField item
            )
        {
            if (ReferenceEquals(item, null))
            {
                throw new ArgumentNullException("item");
            }

            base.SetItem(index, item);
        }

        #endregion
    }
}