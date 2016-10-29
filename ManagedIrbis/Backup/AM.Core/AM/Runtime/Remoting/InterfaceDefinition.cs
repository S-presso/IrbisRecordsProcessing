/* InterfaceDefinition.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Xml.Serialization;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [XmlRoot ( "define" )]
    public sealed class InterfaceDefinition
    {
        #region Properties

        /// <summary>
        /// Interface.
        /// </summary>
        [XmlAttribute ( "interface" )]
        public string Interface;

        /// <summary>
        /// Implementor.
        /// </summary>
        [XmlAttribute ( "implementor" )]
        public string Implementor;

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public InterfaceDefinition ( )
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="interfaceName"></param>
        /// <param name="implementorName"></param>
        public InterfaceDefinition
            (
            string interfaceName,
            string implementorName )
        {
            Interface = interfaceName;
            Implementor = implementorName;
        }

        #endregion
    }
}
