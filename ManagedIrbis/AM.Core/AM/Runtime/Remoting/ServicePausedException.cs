/* ServicePausedException.cs -- 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace AM.Runtime.Remoting
{
    /// <summary>
    /// Thrown when service was paused.
    /// </summary>
    public class ServicePausedException
        : RemotingException,
          ISerializable
    {
        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ServicePausedException ( )
        {
            // Nothing to do here
        }

        /// <summary>
        /// Constructor for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public ServicePausedException
            (
            SerializationInfo info,
            StreamingContext context )
        {
            // Nothing to do here
        }

        #endregion

        #region ISerializable members

        void ISerializable.GetObjectData
            (
            SerializationInfo info,
            StreamingContext context )
        {
            // Nothing to do here
        }

        #endregion
    }
}
