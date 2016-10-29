/* InteropUtility.cs -- set of interop helper routines. 
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

namespace AM.Runtime
{
    /// <summary>
    /// Set of interop helper routines.
    /// </summary>
    public static class InteropUtility
    {
        /// <summary>
        /// Copies buffer to unmanaged memory and returns
        /// pointer to it.
        /// </summary>
        /// <param name="buffer">The buffer to copy.</param>
        /// <returns>Pointer to copy in unmanaged memory.
        /// This pointer must be released by 
        /// <see cref="Marshal.FreeHGlobal"/>.
        /// </returns>
        public static IntPtr BufferToPtr ( byte[] buffer )
        {
            ArgumentUtility.NotNull
                (
                 buffer,
                 "buffer" );

            IntPtr result = Marshal.AllocHGlobal ( buffer.Length );
            Marshal.Copy
                (
                 buffer,
                 0,
                 result,
                 buffer.Length );

            return result;
        }

        /// <summary>
        /// Превращает структуру в массив байтов.
        /// </summary>
        /// <param name="structure"></param>
        /// <returns></returns>
        /// <remarks>Годится только для простых структур.</remarks>
        public static byte[] StructToPtr ( object structure )
        {
            int size = Marshal.SizeOf ( structure );
            IntPtr ptr = Marshal.AllocHGlobal ( size );
            Marshal.StructureToPtr
                (
                 structure,
                 ptr,
                 false );
            byte[] result = new byte[size];
            Marshal.Copy
                (
                 ptr,
                 result,
                 0,
                 size );
            Marshal.FreeHGlobal ( ptr );
            return result;
        }

        /// <summary>
        /// Превращает массив байтов в структуру.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="offset"></param>
        /// <param name="structure"></param>
        /// <remarks>Годится только для простых структур.</remarks>
        public static void PtrToStruct
            (
            byte[] block,
            int offset,
            object structure )
        {
            int size = Marshal.SizeOf ( structure );
            // Debug.Assert ( size == block.Length ); // ???
            IntPtr ptr = Marshal.AllocHGlobal ( size );
            Marshal.Copy
                (
                 block,
                 offset,
                 ptr,
                 size );
            Marshal.PtrToStructure
                (
                 ptr,
                 structure );
            Marshal.FreeHGlobal ( ptr );
        }
    }
}
