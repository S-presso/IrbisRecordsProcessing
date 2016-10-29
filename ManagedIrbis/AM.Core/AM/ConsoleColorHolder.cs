/* ConsoleColorHolder.cs
   ArsMagna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace AM
{
    [Done]
    public sealed class ConsoleColorHolder : IDisposable
    {
        #region Properties

        public ConsoleColor InitialBackground
        {
            get
            {
                return _savedBackground.Last ();
            }
        }

        public ConsoleColor InitialForeground
        {
            get
            {
                return _savedForeground.Last ();
            }
        }

        #endregion

        #region Construction

        public ConsoleColorHolder ( )
        {
            _savedBackground.Push ( Console.BackgroundColor );
            _savedForeground.Push ( Console.ForegroundColor );
        }

        public ConsoleColorHolder ( ConsoleColor newForeground )
            : this ()
        {
            Console.ForegroundColor = newForeground;
        }

        public ConsoleColorHolder
            (
            ConsoleColor newForeground,
            ConsoleColor newBackground )
            : this ( newForeground )
        {
            Console.BackgroundColor = newBackground;
        }

        #endregion

        #region Private members

        private readonly Stack < ConsoleColor > _savedForeground =
            new Stack < ConsoleColor > ();

        private readonly Stack < ConsoleColor > _savedBackground =
            new Stack < ConsoleColor > ();

        #endregion

        #region Public methods

        public ConsoleColorHolder PopBackground ( )
        {
            Console.BackgroundColor = ( _savedBackground.Count == 1 )
                                          ? _savedBackground.Peek ()
                                          : _savedBackground.Pop ();

            return this;
        }

        public ConsoleColorHolder PopForeground ( )
        {
            Console.ForegroundColor = ( _savedForeground.Count == 1 )
                                          ? _savedForeground.Peek ()
                                          : _savedForeground.Pop ();

            return this;
        }

        public ConsoleColorHolder PushBackground ( ConsoleColor newColor )
        {
            _savedForeground.Push ( Console.BackgroundColor );
            Console.BackgroundColor = newColor;
            return this;
        }

        public ConsoleColorHolder PushForeground ( ConsoleColor newColor )
        {
            _savedForeground.Push ( Console.ForegroundColor );
            Console.ForegroundColor = newColor;
            return this;
        }

        #endregion

        #region IDisposable members

        public void Dispose ( )
        {
            Console.BackgroundColor = InitialBackground;
            Console.ForegroundColor = InitialForeground;
        }

        #endregion
    }
}
