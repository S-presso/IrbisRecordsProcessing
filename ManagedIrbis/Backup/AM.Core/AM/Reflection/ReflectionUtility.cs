/* ReflectionUtility.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public static class ReflectionUtility
    {
        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Получение списка всех типов, загруженных на данный момент
        /// в текущий домен.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Осторожно: могут быть загружены сборки только 
        /// для рефлексии. Типы в них непригодны для использования.
        /// </remarks>
        public static Type[] GetAllTypes ( )
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies ();
            List < Type > result = new List < Type > ();
            foreach ( Assembly assembly in assemblies )
            {
                result.AddRange ( assembly.GetTypes () );
            }
            return result.ToArray ();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public static T GetCustomAttribute < T > ( Type classType )
            where T : Attribute
        {
            object[] all = classType.GetCustomAttributes
                (
                 typeof ( T ),
                 false );
            return (T) ( ( all.Length == 0 )
                             ? null
                             : all [ 0 ] );
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <param name="classType">Type of the class.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns></returns>
        public static T GetCustomAttribute < T >
            (
            Type classType,
            bool inherit ) where T : Attribute
        {
            object[] all = classType.GetCustomAttributes
                (
                 typeof ( T ),
                 inherit );
            return (T) ( ( all.Length == 0 )
                             ? null
                             : all [ 0 ] );
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <param name="propertyInfo">The property info.</param>
        /// <returns></returns>
        public static T GetCustomAttribute < T > ( PropertyInfo propertyInfo )
            where T : Attribute
        {
            object[] all = propertyInfo.GetCustomAttributes
                (
                 typeof ( T ),
                 false );
            return (T) ( ( all.Length == 0 )
                             ? null
                             : all [ 0 ] );
        }

        /// <summary>
        /// Gets the custom attribute.
        /// </summary>
        /// <param name="propertyDescriptor">The property descriptor.</param>
        /// <returns></returns>
        public static T GetCustomAttribute < T >
            ( PropertyDescriptor propertyDescriptor ) where T : Attribute
        {
            return (T) propertyDescriptor.Attributes [ typeof ( T ) ];
        }

        /// <summary>
        /// Get default constructor for given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ConstructorInfo GetDefaultConstructor ( Type type )
        {
            ConstructorInfo result = type.GetConstructor
                (
                 BindingFlags.Instance | BindingFlags.Public,
                 null,
                 Type.EmptyTypes,
                 null );
            return result;
        }

        /// <summary>
        /// Get field value either public or private.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetFieldValue < T >
            (
            T target,
            string fieldName )
        {
            if ( string.IsNullOrEmpty ( fieldName ) )
            {
                throw new ArgumentNullException ( "fieldName" );
            }
            FieldInfo finfo = typeof ( T ).GetField
                (
                 fieldName,
                 BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Instance | BindingFlags.Static );
            if ( finfo == null )
            {
                throw new ArgumentException ( "fieldName" );
            }
            return finfo.GetValue ( target );
        }

        /// <summary>
        /// Determines whether the specified type has attribute.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="inherit">if set to <c>true</c> [inherit].</param>
        /// <returns>
        /// 	<c>true</c> if the specified type has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute < T >
            (
            Type type,
            bool inherit ) where T : Attribute
        {
            ArgumentUtility.NotNull
                (
                 type,
                 "type" );

            return ( GetCustomAttribute < T >
                         (
                          type,
                          inherit ) != null );
        }

        /// <summary>
        /// Set field value either public or private.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="target"></param>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public static void SetFieldValue < T, V >
            (
            T target,
            string fieldName,
            V value ) where T : class
        {
            if ( string.IsNullOrEmpty ( fieldName ) )
            {
                throw new ArgumentNullException ( "fieldName" );
            }
            FieldInfo finfo = typeof ( T ).GetField
                (
                 fieldName,
                 BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Instance | BindingFlags.Static );
            if ( finfo == null )
            {
                throw new ArgumentException ( "fieldName" );
            }
            finfo.SetValue
                (
                 target,
                 value );
        }

        /// <summary>
        /// Get property value either public or private.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetPropertyValue < T >
            (
            T target,
            string propertyName )
        {
            if ( string.IsNullOrEmpty ( propertyName ) )
            {
                throw new ArgumentNullException ( "propertyName" );
            }
            PropertyInfo pinfo = typeof ( T ).GetProperty
                (
                 propertyName,
                 BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Instance | BindingFlags.Static );
            if ( pinfo == null )
            {
                throw new ArgumentException ( "propertyName" );
            }
            return pinfo.GetValue
                (
                 target,
                 null );
        }

        /// <summary>
        /// Gets the properties and fields.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="bindingFlags">The binding flags.</param>
        /// <returns></returns>
        public static PropertyOrField[] GetPropertiesAndFields
            (
            Type type,
            BindingFlags bindingFlags )
        {
            ArgumentUtility.NotNull
                (
                 type,
                 "type" );

            List < PropertyOrField > result = new List < PropertyOrField > ();
            foreach (
                PropertyInfo property in type.GetProperties ( bindingFlags ) )
            {
                result.Add ( new PropertyOrField ( property ) );
            }
            foreach ( FieldInfo field in type.GetFields ( bindingFlags ) )
            {
                result.Add ( new PropertyOrField ( field ) );
            }

            return result.ToArray ();
        }

        /// <summary>
        /// Set property value either public or private.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public static void SetPropertyValue < T, V >
            (
            T target,
            string propertyName,
            V value )
        {
            if ( string.IsNullOrEmpty ( propertyName ) )
            {
                throw new ArgumentNullException ( "propertyName" );
            }
            PropertyInfo pinfo = typeof ( T ).GetProperty
                (
                 propertyName,
                 BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Instance | BindingFlags.Static );
            if ( pinfo == null )
            {
                throw new ArgumentException ( "propertyName" );
            }
            pinfo.SetValue
                (
                 target,
                 value,
                 null );
        }

        #endregion
    }
}
