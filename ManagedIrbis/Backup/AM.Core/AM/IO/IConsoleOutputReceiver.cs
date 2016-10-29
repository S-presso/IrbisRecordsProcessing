/* IConsoleOutputReceiver.cs
   ArsMagna project, https://www.assembla.com/spaces/arsmagna */

namespace AM.IO
{
    /// <summary>
    /// Receives console output.
    /// </summary>
    public interface IConsoleOutputReceiver
    {
        /// <summary>
        /// Receives the console line.
        /// </summary>
        /// <param name="text">The text.</param>
        void ReceiveConsoleOutput ( string text );
    }
}
