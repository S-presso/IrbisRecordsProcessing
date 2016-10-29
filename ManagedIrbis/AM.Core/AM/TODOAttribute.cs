/* TODOAttribute.cs -- marks incomplete classes and class members.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM
{
    /// <summary>
    /// Marks incomplete classes and class members.
    /// </summary>
    /// <example>
    /// Here is example of <see cref="TodoAttribute"/> usage.
    /// <code>
    /// [TODO ("Optimize locate algoritm")]
    /// public Employee LocateEmployee ( int empId )
    /// {
    /// 	...
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="DoneAttribute"/>
    /// <seealso cref="NotImplementedAttribute"/>
    [Done]
    [AttributeUsage ( AttributeTargets.All, AllowMultiple = false,
        Inherited = false )]
    public sealed class TodoAttribute : Attribute
    {
        #region Properties

        private readonly string _message;

        /// <summary>
        /// Gets the note.
        /// </summary>
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
        /// <see cref="TodoAttribute"/> class
        /// without message.
        /// </summary>
        public TodoAttribute ( )
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="TodoAttribute"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TodoAttribute ( string message )
        {
            _message = message;
        }

        #endregion
    }
}
