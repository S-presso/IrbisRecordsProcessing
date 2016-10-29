/* SerializableDictionary.cs -- generic dictionary with XML-serialization support.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Generic dictionary with XML-serialization support.
    /// </summary>
    /// <typeparam name="TKey">Type of key.</typeparam>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <remarks>Both <typeparamref name="TKey"/> and
    /// <typeparamref name="TValue"/>must be XML-serializable.
    /// </remarks>
    public class SerializableDictionary < TKey, TValue >
        : Dictionary < TKey, TValue >,
          IXmlSerializable
    {
        #region IXmlSerializable members

        /// <summary>
        /// This property is reserved, apply the 
        /// <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/>
        /// to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/>
        /// that describes the XML representation of the object that 
        /// is produced by the 
        /// <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/>
        /// method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema ( )
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The 
        /// <see cref="T:System.Xml.XmlReader"/> stream from which 
        /// the object is deserialized.</param>
        void IXmlSerializable.ReadXml ( XmlReader reader )
        {
            XmlSerializer keySerializer = new XmlSerializer ( typeof ( TKey ) );
            XmlSerializer valueSerializer = new XmlSerializer
                ( typeof ( TValue ) );

            reader.Read ();
            reader.ReadStartElement ( "dictionary" );
            while ( reader.NodeType != XmlNodeType.EndElement )
            {
                reader.ReadStartElement ( "pair" );

                reader.ReadStartElement ( "key" );
                TKey key = (TKey) keySerializer.Deserialize ( reader );
                reader.ReadEndElement ();

                reader.ReadStartElement ( "value" );
                TValue value = (TValue) valueSerializer.Deserialize ( reader );
                reader.ReadEndElement ();

                Add
                    (
                     key,
                     value );
                reader.ReadEndElement ();
                reader.MoveToContent ();
            }
            reader.ReadEndElement ();
            reader.ReadEndElement ();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/>
        /// stream to which the object is serialized.</param>
        void IXmlSerializable.WriteXml ( XmlWriter writer )
        {
            XmlSerializer keySerializer = new XmlSerializer ( typeof ( TKey ) );
            XmlSerializer valueSerializer = new XmlSerializer
                ( typeof ( TValue ) );

            writer.WriteStartElement ( "dictionary" );
            foreach ( KeyValuePair < TKey, TValue > pair in this )
            {
                writer.WriteStartElement ( "pair" );

                writer.WriteStartElement ( "key" );
                keySerializer.Serialize
                    (
                     writer,
                     pair.Key );
                writer.WriteEndElement ();

                writer.WriteStartElement ( "value" );
                valueSerializer.Serialize
                    (
                     writer,
                     pair.Value );
                writer.WriteEndElement ();

                writer.WriteEndElement ();
            }
            writer.WriteEndElement ();
        }

        #endregion
    }
}
