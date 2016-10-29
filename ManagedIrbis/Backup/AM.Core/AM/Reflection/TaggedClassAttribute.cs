/* TaggedClassAttribute.cs -- marks class with given tag
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// One can tag the class (e.g. for XML serialization).
    /// </summary>
    [AttributeUsage ( AttributeTargets.Class, AllowMultiple = true )]
    public sealed class TaggedClassAttribute : Attribute
    {
        #region Properties

        private string _tag;

        /// <summary>
        /// Tag.
        /// </summary>
        public string Tag
        {
            [DebuggerStepThrough]
            get
            {
                return _tag;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tag">Tag.</param>
        public TaggedClassAttribute ( string tag )
        {
            _tag = tag;
        }

        #endregion
    }
}
