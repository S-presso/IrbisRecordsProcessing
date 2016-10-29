/* PropertyAccessor.cs -- get access to private properties via reflection.
   Ars Magna project, http://library.istu.edu/am */

#region Using directives

using System;
using System.Diagnostics;
using System.Reflection;

#endregion

namespace AM.Reflection
{
    /// <summary>
    /// Some hacking: get access to private properties (via reflection).
    /// </summary>
    /// <typeparam name="T">Main object type.</typeparam>
    /// <typeparam name="V">Property type.</typeparam>
    /// <example>
    /// <code>
    /// using System;
    /// using AM.Reflection;
    ///
    ///	class Canary
    ///	{
    ///		public int myProp;
    ///		private int MyProp
    ///		{
    ///			get
    ///			{
    ///				return myProp;
    ///			}
    ///			set
    ///			{
    ///				myProp = value;
    ///			}
    ///		}
    ///	}
    /// 
    ///	class Program
    ///	{
    ///		static void Main ( string [] args )
    ///		{
    ///			Canary canary = new Canary ();
    ///			PropertyAccessor&lt;Canary, int&gt; pa
    ///	           = new PropertyAccessor&lt;Canary, int&gt; ( canary, "MyProp" );
    ///			pa.Value = 2;
    ///			Console.WriteLine ( canary.myProp );
    ///		}
    ///	}
    /// </code>
    /// </example>
    public class PropertyAccessor < T, V >
        where T : class
    {
        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taget"></param>
        /// <param name="value"></param>
        public delegate void AccessHandler ( T taget,
                                             V value );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="previousTarget"></param>
        public delegate void TargetHandler ( PropertyAccessor < T, V > accessor,
                                             T previousTarget );

        /// <summary>
        /// Fired when getting value.
        /// </summary>
        public event AccessHandler GettingValue;

        /// <summary>
        /// Fired when setting value.
        /// </summary>
        public event AccessHandler SettingValue;

        /// <summary>
        /// Fired when target changed.
        /// </summary>
        public event TargetHandler TargetChanged;

        #endregion

        #region Properties

        private T _target;

        ///<summary>
        /// Target.
        ///</summary>
        public virtual T Target
        {
            [DebuggerStepThrough]
            get
            {
                return _target;
            }
            [DebuggerStepThrough]
            set
            {
                T previousTarget = _target;
                _target = value;
                OnTargetChanged ( previousTarget );
            }
        }

        private readonly PropertyInfo _info;

        ///<summary>
        /// Property info.
        ///</summary>
        public PropertyInfo Info
        {
            [DebuggerStepThrough]
            get
            {
                return _info;
            }
        }

        /// <summary>
        /// Property value.
        /// </summary>
        public virtual V Value
        {
            get
            {
                if ( _getter == null )
                {
                    throw new NotSupportedException ();
                }
                return OnGettingValue ( _getter () );
            }
            set
            {
                if ( _setter == null )
                {
                    throw new NotSupportedException ();
                }
                _setter ( OnSettingValue ( value ) );
            }
        }

        #endregion

        #region Construction

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="propertyName"></param>
        public PropertyAccessor
            (
            T target,
            string propertyName )
        {
            _target = target;
            if ( string.IsNullOrEmpty ( propertyName ) )
            {
                throw new ArgumentNullException ( "propertyName" );
            }
            _info = typeof ( T ).GetProperty
                (
                 propertyName,
                 BindingFlags.Public | BindingFlags.NonPublic
                 | BindingFlags.Instance | BindingFlags.Static );
            if ( _info == null )
            {
                throw new ArgumentException ( "propertyName" );
            }
            _CreateDelegates ();
        }

        #endregion

        #region Private members

        private delegate V _Getter ( );

        private _Getter _getter;

        private delegate void _Setter ( V value );

        private _Setter _setter;

        private void _CreateDelegates ( )
        {
            if ( _info.CanRead )
            {
                MethodInfo minfo = _info.GetGetMethod ( true );
                _getter =
                    (_Getter) Delegate.CreateDelegate
                                  (
                                   typeof ( _Getter ),
                                   Target,
                                   minfo );
            }
            if ( _info.CanWrite )
            {
                MethodInfo minfo = _info.GetSetMethod ( true );
                _setter =
                    (_Setter) Delegate.CreateDelegate
                                  (
                                   typeof ( _Setter ),
                                   Target,
                                   minfo );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected virtual V OnGettingValue ( V value )
        {
            if ( GettingValue != null )
            {
                GettingValue
                    (
                     Target,
                     value );
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        protected virtual V OnSettingValue ( V value )
        {
            if ( SettingValue != null )
            {
                SettingValue
                    (
                     Target,
                     value );
            }
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="previousTarget"></param>
        protected virtual void OnTargetChanged ( T previousTarget )
        {
            _CreateDelegates ();
            if ( TargetChanged != null )
            {
                TargetChanged
                    (
                     this,
                     previousTarget );
            }
        }

        #endregion
    }
}
