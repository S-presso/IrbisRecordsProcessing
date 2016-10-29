/* InterfaceDefinitionList.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;

using AM.Reflection;
using AM.Xml;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// 
    /// </summary>
    public class InterfaceDefinitionList : List < InterfaceDefinition >
    {
        #region Public methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <param name="implementorName"></param>
        public void Add
            (
            string interfaceName,
            string implementorName )
        {
            Add
                (
                 new InterfaceDefinition
                     (
                     interfaceName,
                     implementorName ) );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="implementorType"></param>
        public void Add
            (
            Type interfaceType,
            Type implementorType )
        {
            Add
                (
                 new InterfaceDefinition
                     (
                     interfaceType.AssemblyQualifiedName,
                     implementorType.AssemblyQualifiedName ) );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public void ToFile ( string fileName )
        {
            XmlUtility.Serialize < InterfaceDefinitionList >
                (
                 fileName,
                 this );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static InterfaceDefinitionList FromFile ( string fileName )
        {
            return XmlUtility.Deserialize < InterfaceDefinitionList >
                ( fileName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static InterfaceDefinitionList FromAssembly ( Assembly assembly )
        {
            InterfaceDefinitionList result = new InterfaceDefinitionList ();

            foreach ( Type implementor in assembly.GetTypes () )
            {
                FactoryClassAttribute fca =
                    ReflectionUtility
                        .GetCustomAttribute < FactoryClassAttribute >
                        ( implementor );
                if ( fca != null )
                {
                    result.Add
                        (
                         fca.Interface,
                         implementor );
                }
            }

            return result;
        }

        #endregion
    }
}
