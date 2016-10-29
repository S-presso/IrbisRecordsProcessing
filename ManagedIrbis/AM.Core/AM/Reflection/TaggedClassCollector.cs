/* TaggedClassCollector.cs -- collects tagged classes in given assemblies 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// Collects tagged classes in given assemblies.
    /// </summary>
    public static class TaggedClassesCollector
    {
        #region Public methods

        /// <summary>
        /// Collect tagged class in given assembly.
        /// </summary>
        /// <param name="asm">Assembly to scan.</param>
        /// <param name="tagName">Tag.</param>
        /// <returns>Found classes.</returns>
        public static Type[] Collect
            (
            Assembly asm,
            string tagName )
        {
            List < Type > list = new List < Type > ();

            foreach ( Type type in asm.GetTypes () )
            {
                object[] attrs = type.GetCustomAttributes
                    (
                     typeof ( TaggedClassAttribute ),
                     true );

                foreach ( TaggedClassAttribute attr in attrs )
                {
                    if ( attr.Tag == tagName )
                    {
                        list.Add ( type );
                    }
                }
            }

            return list.ToArray ();
        }

        /// <summary>
        /// Collect tagged classes in all asseblies.
        /// </summary>
        /// <param name="tagName">Tag.</param>
        /// <returns>Found classes.</returns>
        public static Type[] Collect ( string tagName )
        {
            List < Type > list = new List < Type > ();
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies ();

            foreach ( Assembly asm in asms )
            {
                Type[] types = Collect
                    (
                     asm,
                     tagName );
                list.AddRange ( types );
            }

            return list.ToArray ();
        }

        #endregion
    }
}
