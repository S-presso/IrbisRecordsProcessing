/* DictionaryCounter.cs -- simple dictionary to count values.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Simple dictionary to count values.
    /// </summary>
    [Serializable]
    public sealed class DictionaryCounter < TKey > : Dictionary < TKey, double >
    {
        #region Properties

        /// <summary>
        /// Gets the total.
        /// </summary>
        /// <value>The total.</value>
        public double Total
        {
            get
            {
                lock ( _SyncRoot )
                {
                    double result = 0.0;
                    foreach ( double value in Values )
                    {
                        result += value;
                    }
                    return result;
                }
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DictionaryCounter{TKey}"/> class.
        /// </summary>
        public DictionaryCounter ( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DictionaryCounter{TKey}"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        public DictionaryCounter ( int capacity )
            : base ( capacity )
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DictionaryCounter{TKey}"/> class.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        public DictionaryCounter ( DictionaryCounter < TKey > dictionary )
            : base ( dictionary )
        {
        }

        #endregion

        #region Private members

        private object _SyncRoot
        {
            [DebuggerStepThrough]
            get
            {
                return ( ( (ICollection) this ).SyncRoot );
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Augments the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="increment">The value.</param>
        /// <returns>New value for given key.</returns>
        public double Augment
            (
            TKey key,
            double increment )
        {
            lock ( _SyncRoot )
            {
                double value;
                TryGetValue
                    (
                     key,
                     out value );
                value += increment;
                this [ key ] = value;
                return value;
            }
        }

        #endregion
    }
}
