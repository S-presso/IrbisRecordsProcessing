/* DoublyLinkedListNode.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Ýëåìåíò äâàæäû-ñâÿçàííîãî ñïèñêà.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class DoublyLinkedListNode < T >
    {
        #region Public properties

        private DoublyLinkedList < T > _list;

        /// <summary>
        /// Ñïèñîê-âëàäåëåö.
        /// </summary>
        /// <value></value>
        public DoublyLinkedList < T > List
        {
            [DebuggerStepThrough]
            get
            {
                return _list;
            }
            [DebuggerStepThrough]
            internal set
            {
                _list = value;
            }
        }

        private DoublyLinkedListNode < T > _previous;

        /// <summary>
        /// Ïðåäûäóùèé ýëåìåíò.
        /// </summary>
        /// <value></value>
        public DoublyLinkedListNode < T > Previous
        {
            [DebuggerStepThrough]
            get
            {
                return _previous;
            }
            [DebuggerStepThrough]
            set
            {
                _previous = value;
            }
        }

        private DoublyLinkedListNode < T > _next;

        /// <summary>
        /// Ïîñëåäóþùèé ýëåìåíò.
        /// </summary>
        /// <value></value>
        public DoublyLinkedListNode < T > Next
        {
            [DebuggerStepThrough]
            get
            {
                return _next;
            }
            [DebuggerStepThrough]
            set
            {
                _next = value;
            }
        }

        private T _value;

        /// <summary>
        /// Õðàíèìîå çíà÷åíèå.
        /// </summary>
        /// <value></value>
        public T Value
        {
            [DebuggerStepThrough]
            get
            {
                return _value;
            }
            [DebuggerStepThrough]
            set
            {
                _value = value;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="T:DoublyLinkedListNode&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public DoublyLinkedListNode ( T value )
        {
            _value = value;
        }

        #endregion
    }
}
