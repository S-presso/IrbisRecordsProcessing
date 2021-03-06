/* TypeUtility.cs -- useful routines for type manipulations
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;

#endregion

namespace AM
{
    /// <summary>
    /// Сборник полезных методов, работающих с информацией о типах.
    /// </summary>
    public static class TypeUtility
    {
        /// <summary>
        /// Получение списка всех типов-наследников указанного типа.
        /// </summary>
        /// <param name="parentType"></param>
        /// <returns></returns>
        public static Type[] GetAllDescendants ( Type parentType )
        {
            ArgumentUtility.NotNull
                (
                 parentType,
                 "parentType" );

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
            List < Type > result = new List < Type > ();
            foreach ( Assembly assembly in assemblies )
            {
                foreach ( Type type in assembly.GetTypes () )
                {
                    if ( type.IsSubclassOf ( parentType ) )
                    {
                        result.Add ( type );
                    }
                }
            }

            return result.ToArray ();
        }

        /// <summary>
        /// Получение закрытого generic-типа, параметризованного указанными 
        /// типами.
        /// </summary>
        /// <param name="genericTypeName"></param>
        /// <param name="typeList"></param>
        /// <returns></returns>
        public static Type GetGenericType
            (
            string genericTypeName,
            params string[] typeList )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 genericTypeName,
                 "genericTypeName" );

            // construct the mangled name
            string mangledName = genericTypeName + "`" + typeList.Length;

            // get the open generic type
            Type genericType = Type.GetType ( mangledName );

            // construct the array of generic type parameters
            Type[] typeArgs = new Type[typeList.Length];
            for ( int i = 0; i < typeList.Length; i++ )
            {
                typeArgs [ i ] = Type.GetType ( typeList [ i ] );
            }

            // get the closed generic type
            Type constructed = genericType.MakeGenericType ( typeArgs );

            return constructed;
        }

        /// <summary>
        /// Gets type of the argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        public static Type GetType < T > ( T arg ) where T : class
        {
            return ( arg == null )
                       ? typeof ( T )
                       : arg.GetType ();
        }
    }
}
