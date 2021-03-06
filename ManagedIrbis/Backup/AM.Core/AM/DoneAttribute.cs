/* DoneAttribute.cs -- marks complete classes and class members.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM
{
    /// <summary>
    /// Marks complete classes and class members.
    /// </summary>
    /// <example>
    /// Here is example of <see cref="DoneAttribute"/> usage.
    /// <code>
    /// [Done ("Locate algoritm optimized")]
    /// public Employee LocateEmployee ( int empId )
    /// {
    /// 	...
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="TodoAttribute"/>
    /// <seealso cref="NotImplementedAttribute"/>
    [Done]
    [AttributeUsage ( AttributeTargets.All, AllowMultiple = false,
        Inherited = false )]
    public sealed class DoneAttribute : Attribute
    {
        #region Properties

        private readonly string _message;

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            [DebuggerStepThrough]
            get
            {
                return _message;
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DoneAttribute"/> class
        /// without any message.
        /// </summary>
        public DoneAttribute ( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="DoneAttribute"/> class
        /// with given message.
        /// </summary>
        /// <param name="message">The message.</param>
        public DoneAttribute ( string message )
        {
            _message = message;
        }

        #endregion
    }
}
