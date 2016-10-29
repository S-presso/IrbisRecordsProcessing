/* PropertyIndexer.cs -- 
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyIndexer
    {
        #region Properties

        private object _target;

        /// <summary>
        /// Gets the target.
        /// </summary>
        /// <value>The target.</value>
        public object Target
        {
            [DebuggerStepThrough]
            get
            {
                return _target;
            }
        }

        /// <summary>
        /// Gets or sets the property value for the specified property name.
        /// </summary>
        /// <value></value>
        public object this [ string propertyName ]
        {
            get
            {
                return GetProperty ( propertyName )
                    .GetValue
                    (
                     Target,
                     null );
            }
            set
            {
                GetProperty ( propertyName )
                    .SetValue
                    (
                     Target,
                     value,
                     null );
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyIndexer"/> class.
        /// </summary>
        /// <param name="target">The target.</param>
        public PropertyIndexer ( object target )
        {
            SetTarget ( target );
        }

        #endregion

        #region Private members

        #endregion

        #region Public methods

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public PropertyInfo GetProperty ( string propertyName )
        {
            ArgumentUtility.NotNullOrEmpty
                (
                 propertyName,
                 "propertyName" );
            PropertyInfo result = Target.GetType ()
                                        .GetProperty
                (
                 propertyName,
                 BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Instance );
            if ( result == null )
            {
                throw new KeyNotFoundException ( propertyName );
            }
            return result;
        }

        /// <summary>
        /// Sets the target.
        /// </summary>
        /// <param name="target">The target.</param>
        public void SetTarget ( object target )
        {
            ArgumentUtility.NotNull
                (
                 target,
                 "target" );
            _target = target;
        }

        #endregion
    }
}
