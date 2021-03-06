/* Reference.cs -- Generic reference to given object.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM
{
    /// <summary>
    /// Generic reference to given object. Allows to track object changes.
    /// </summary>
    /// <typeparam name="T">Type of object to reference.</typeparam>
    public class Reference < T >
    {
        #region Events

        /// <summary>
        /// Fired when target value changed.
        /// </summary>
        public event EventHandler TargetChanged;

        #endregion

        #region Properties

        private T _target;

        /// <summary>
        /// Gets or sets the target.
        /// </summary>
        /// <value>The target.</value>
        public T Target
        {
            [DebuggerStepThrough]
            get
            {
                return _target;
            }
            [DebuggerStepThrough]
            set
            {
                _target = value;
                OnTargetChanged ();
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference{T}"/> class.
        /// </summary>
        public Reference ( )
        {
            _target = default ( T );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public Reference ( T initialValue )
        {
            _target = initialValue;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Implicit operators the specified reference.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public static implicit operator T ( Reference < T > reference )
        {
            return reference.Target;
        }

        /// <summary>
        /// Implicit operators the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static implicit operator Reference < T > ( T value )
        {
            return new Reference < T > ( value );
        }

        #endregion

        #region Private members

        /// <summary>
        /// Fired when target value changed.
        /// </summary>
        protected virtual void OnTargetChanged ( )
        {
            EventHandler handler = TargetChanged;
            if ( handler != null )
            {
                handler
                    (
                     this,
                     EventArgs.Empty );
            }
        }

        #endregion
    }
}
