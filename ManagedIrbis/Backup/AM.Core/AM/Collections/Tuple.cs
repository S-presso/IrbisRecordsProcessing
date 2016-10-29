/* Tuple.cs -- simple tuple implementation
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

using R = AM.Core.Properties.Resources;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Simple tuple implementation.
    /// </summary>
    [Serializable]
    public sealed class Tuple
        : IEnumerable,
          IEquatable < Tuple >,
          ICloneable,
          IXmlSerializable
    {
        #region Properties

        private object[] _items;

        /// <summary>
        /// Gets the <see cref="System.Object"/> at the specified index.
        /// </summary>
        public object this [ int index ]
        {
            [DebuggerStepThrough]
            get
            {
                return _items [ index ];
            }
        }

        #endregion

        #region Construction

        private Tuple ( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public Tuple ( params object[] items )
        {
            ArgumentUtility.NotNull
                (
                 items,
                 "items" );
            _items = items;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple"/> class.
        /// </summary>
        /// <param name="items">The items.</param>
        public Tuple ( IEnumerable items )
        {
            ArgumentUtility.NotNull
                (
                 items,
                 "items" );

            ArrayList list = new ArrayList ();
            foreach ( object item in items )
            {
                list.Add ( item );
            }
            _items = list.ToArray ();
        }

        #endregion

        #region IEnumerable members

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/>
        /// object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator ( )
        {
            foreach ( object item in _items )
            {
                yield return item;
            }
        }

        #endregion

        #region ICloneable members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone ( )
        {
            Tuple result = new Tuple ();
            result._items = _items;
            return result;
        }

        ///<summary>
        /// Indicates whether the current object is equal 
        /// to another object of the same type.
        ///</summary>
        /// <returns>
        /// true if the current object is equal to the other parameter;
        /// otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.
        /// </param>
        public bool Equals ( Tuple other )
        {
            ArgumentUtility.NotNull
                (
                 other,
                 "other" );
            return EnumerableUtility.Equals
                (
                 this,
                 other );
        }

        #endregion

        #region IEquatable<Tuple> members

        #endregion

        #region IXmlSerializable members

        ///<summary>
        /// This property is reserved, apply the
        /// <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/>
        /// to the class instead. 
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/>
        /// that describes the XML representation of the object that 
        /// is produced by the 
        /// <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/>
        /// method and consumed by the 
        /// <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/>
        /// method.
        ///</returns>
        public XmlSchema GetSchema ( )
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The 
        /// <see cref="T:System.Xml.XmlReader"/>
        /// stream from which the object is deserialized.</param>
        public void ReadXml ( XmlReader reader )
        {
            ArrayList list = new ArrayList ();
            reader.Read ();
            while ( reader.NodeType != XmlNodeType.EndElement )
            {
                reader.MoveToFirstAttribute ();

                if ( reader.Name == "null" )
                {
                    list.Add ( null );
                    reader.Read ();
                }
                else
                {
                    if ( reader.Name != "type" )
                    {
                        throw new XmlException ( R.MissingTypeAttribute );
                    }
                    Type itemType = Type.GetType ( reader.Value );
                    XmlSerializer serializer = new XmlSerializer ( itemType );
                    reader.Read ();
                    object item = serializer.Deserialize ( reader );
                    list.Add ( item );
                    reader.ReadEndElement ();
                }
                reader.MoveToContent ();
            }
            _items = list.ToArray ();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The 
        /// <see cref="T:System.Xml.XmlWriter"/>
        /// stream to which the object is serialized.</param>
        public void WriteXml ( XmlWriter writer )
        {
            //writer.WriteStartElement ( "tuple" );
            foreach ( object item in this )
            {
                writer.WriteStartElement ( "item" );
                if ( item == null )
                {
                    writer.WriteAttributeString
                        (
                         "null",
                         "true" );
                }
                else
                {
                    Type itemType = item.GetType ();
                    writer.WriteAttributeString
                        (
                         "type",
                         itemType.FullName );
                    XmlSerializer serializer = new XmlSerializer ( itemType );
                    serializer.Serialize
                        (
                         writer,
                         item );
                }
                writer.WriteEndElement ();
            }
            //writer.WriteEndElement ();
        }

        #endregion

        #region Object members

        /// <summary>
        /// Returns a <see cref="T:System.String"/>
        /// that represents the current <see cref="T:System.Object"/>.
        ///</summary>
        /// <returns>
        /// A <see cref="T:System.String"/>
        /// that represents the current <see cref="T:System.Object"/>.
        ///</returns>
        public override string ToString ( )
        {
            StringBuilder result = new StringBuilder ();
            result.Append ( "[" );
            bool first = true;
            foreach ( object item in this )
            {
                if ( !first )
                {
                    result.Append ( "," );
                }
                first = false;
                result.Append ( item );
            }
            result.Append ( "]" );
            return result.ToString ();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/>
        /// is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/>
        /// to compare with the current <see cref="T:System.Object"/>.
        /// </param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/>
        /// is equal to the current <see cref="T:System.Object"/>;
        /// otherwise, false.
        /// </returns>
        public override bool Equals ( object obj )
        {
            if ( ReferenceEquals
                (
                 this,
                 obj ) )
            {
                return true;
            }
            Tuple otherTuple = obj as Tuple;
            if ( otherTuple == null )
            {
                return false;
            }
            return EnumerableUtility.Equals
                (
                 this,
                 otherTuple );
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// <see cref="M:System.Object.GetHashCode"/>
        /// is suitable for use in hashing algorithms and data 
        /// structures like a hash table.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode ( )
        {
            return _items.GetHashCode ();
        }

        #endregion
    }
}
