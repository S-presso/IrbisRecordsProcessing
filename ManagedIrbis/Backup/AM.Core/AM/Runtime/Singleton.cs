/* Singleton.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;

#endregion

namespace AM.Runtime
{
    /// <summary>
    /// Хранилище объектов по принципу "каждого типа по одной штуке".
    /// </summary>
    public static class Singleton
    {
        #region Private members

        private static Dictionary < Type, object > _dictionary =
            new Dictionary < Type, object > ();

        #endregion

        #region Public methods

        /// <summary>
        /// Выдает объект указанного типа.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Instance < T > ( ) where T : class
        {
            T result;
            Type type = typeof ( T );
            lock ( _dictionary )
            {
                object value;
                if ( _dictionary.TryGetValue
                    (
                     type,
                     out value ) )
                {
                    result = (T) value;
                }
                else
                {
                    result = Activator.CreateInstance < T > ();
                }
            }
            return result;
        }

        #endregion
    }
}
