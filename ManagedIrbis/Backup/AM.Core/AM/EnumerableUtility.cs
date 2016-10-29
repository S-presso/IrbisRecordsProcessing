/* EnumerableUtility.cs -- some useful routines for IEnumerable
   ArsMagna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;

#endregion

namespace AM
{
    /// <summary>
    /// Useful routines for IEnumerable
    /// </summary>
    public static class EnumerableUtility
    {
        #region Private members

        private static IEnumerable < T > PipeImpl < T >
            (
            this IEnumerable < T > source,
            Action < T > action )
        {
            foreach ( var element in source )
            {
                action ( element );
                yield return element;
            }
        }

        private static IEnumerable < T > TakeLastImpl < T >
            (
            IEnumerable < T > source,
            int count )
        {
            Debug.Assert ( source != null );

            if ( count <= 0 )
            {
                yield break;
            }

            var q = new Queue < T > ( count );

            foreach ( var item in source )
            {
                if ( q.Count == count )
                {
                    q.Dequeue ();
                }
                q.Enqueue ( item );
            }

            foreach ( var item in q )
            {
                yield return item;
            }
        }

        private static string ToDelimitedStringImpl < T >
            (
            IEnumerable < T > source,
            string delimiter,
            Func < StringBuilder, T, StringBuilder > append )
        {
            Debug.Assert ( source != null );
            Debug.Assert ( append != null );

            delimiter = delimiter
                        ?? CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            var sb = new StringBuilder ();
            var i = 0;

            foreach ( var value in source )
            {
                if ( i++ > 0 )
                {
                    sb.Append ( delimiter );
                }
                append
                    (
                     sb,
                     value );
            }

            return sb.ToString ();
        }


        private static IEnumerable < TSource > TraceImpl < TSource >
            (
            IEnumerable < TSource > source,
            Func < TSource, string > formatter )
        {
            Debug.Assert ( source != null );
            Debug.Assert ( formatter != null );

            return source
#if !NO_TRACING
                .Pipe
                ( x => System.Diagnostics.Trace.WriteLine ( formatter ( x ) ) )
#endif
                ;
        }

        #endregion

        #region Public methods

        ///// <summary>
        ///// Returns a sequence consisting of the head element and the given tail elements.
        ///// </summary>
        ///// <typeparam name="T">Type of sequence</typeparam>
        ///// <param name="head">Head element of the new sequence.</param>
        ///// <param name="tail">All elements of the tail. Must not be null.</param>
        ///// <returns>A sequence consisting of the head elements and the given tail elements.</returns>
        ///// <remarks>This operator uses deferred execution and streams its results.</remarks>
        //public static IEnumerable<T> Concat<T>
        //    (
        //        this T head, 
        //        IEnumerable<T> tail
        //    )
        //{
        //    if (tail == null) throw new ArgumentNullException("tail");
        //    return tail.Prepend(head);
        //}

        ///// <summary>
        ///// Returns a sequence consisting of the head elements and the given tail element.
        ///// </summary>
        ///// <typeparam name="T">Type of sequence</typeparam>
        ///// <param name="head">All elements of the head. Must not be null.</param>
        ///// <param name="tail">Tail element of the new sequence.</param>
        ///// <returns>A sequence consisting of the head elements and the given tail element.</returns>
        ///// <remarks>This operator uses deferred execution and streams its results.</remarks>
        //public static IEnumerable<T> Concat<T>
        //    (
        //        this IEnumerable<T> head, 
        //        T tail
        //    )
        //{
        //    if (head == null) throw new ArgumentNullException("head");
        //    return Concat<T>(head, Enumerable.Repeat<T>(tail, 1));
        //}

        /// <summary>
        /// Completely consumes the given sequence. This method uses immediate execution,
        /// and doesn't store any data during execution.
        /// </summary>
        /// <typeparam name="T">Element type of the sequence</typeparam>
        /// <param name="sequence">Source to consume</param>
        /// <remarks>
        /// <para>Borrowed from 
        /// <a href="http://code.google.com/p/morelinq/">MoreLINQ</a>
        /// project.
        /// </para>
        /// </remarks>
        public static void Consume < T > ( this IEnumerable < T > sequence )
        {
            ArgumentUtility.NotNull
                (
                 sequence,
                 "sequence" );

#pragma warning disable 168
            foreach ( T element in sequence )
            {
                // Do nothing
            }
#pragma warning restore 168
        }

        /// <summary>
        /// Immediately executes the given action on each element in the source sequence.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the sequence</typeparam>
        /// <param name="source">The sequence of elements</param>
        /// <param name="action">The action to execute on each element</param>
        /// <remarks>
        /// <para>Borrowed from 
        /// <a href="http://code.google.com/p/morelinq/">MoreLINQ</a>
        /// project.
        /// </para>
        /// </remarks>
        public static void ForEach < T >
            (
            this IEnumerable < T > source,
            Action < T > action )
        {
            if ( source == null )
            {
                throw new ArgumentNullException ( "source" );
            }
            if ( action == null )
            {
                throw new ArgumentNullException ( "action" );
            }
            foreach ( var element in source )
            {
                action ( element );
            }
        }


        /// <summary>
        /// Executes the given action on each element in the source sequence
        /// and yields it.
        /// </summary>
        /// <param name="sequence">The sequence of elements</param>
        /// <param name="action">The action to execute on each element</param>    
        /// <remarks>
        /// The returned sequence is essentially a duplicate of
        /// the original, but with the extra action being executed while the
        /// sequence is evaluated. The action is always taken before the element
        /// is yielded, so any changes made by the action will be visible in the
        /// returned sequence. This operator uses deferred execution and streams it results.
        /// <typeparam name="T">The type of the elements in the sequence</typeparam>
        /// <para>Borrowed from
        /// <a href="http://code.google.com/p/morelinq/">MoreLINQ</a>
        /// project.
        /// </para>
        /// </remarks>
        public static IEnumerable < T > Pipe < T >
            (
            this IEnumerable < T > sequence,
            Action < T > action )
        {
            ArgumentUtility.NotNull
                (
                 sequence,
                 "sequence" );
            ArgumentUtility.NotNull
                (
                 action,
                 "action" );

            return PipeImpl
                (
                 sequence,
                 action );
        }

        ///// <summary>
        ///// Prepends a single value to a sequence.
        ///// </summary>
        ///// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        ///// <param name="source">The sequence to prepend to.</param>
        ///// <param name="value">The value to prepend.</param>
        ///// <returns>
        ///// Returns a sequence where a value is prepended to it.
        ///// </returns>
        ///// <remarks>
        ///// This operator uses deferred execution and streams its results.
        ///// </remarks>
        ///// <code>
        ///// int[] numbers = { 1, 2, 3 };
        ///// IEnumerable&lt;int&gt; result = numbers.Prepend(0);
        ///// </code>
        ///// The <c>result</c> variable, when iterated over, will yield 
        ///// 0, 1, 2 and 3, in turn.

        //public static IEnumerable<TSource> Prepend<TSource>
        //    (
        //        this IEnumerable<TSource> source, 
        //        TSource value
        //    )
        //{
        //    if (source == null) throw new ArgumentNullException("source");
        //    return Concat<TSource>(Enumerable.Repeat<TSource>(value, 1), source);
        //}

        /// <summary>
        /// Returns a specified number of contiguous elements from the end of 
        /// a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to return the last element of.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> that contains the specified number of 
        /// elements from the end of the input sequence.
        /// </returns>
        /// <remarks>
        /// This operator uses deferred execution and streams its results.
        /// <para>Borrowed from 
        /// <a href="http://code.google.com/p/morelinq/">MoreLINQ</a>
        /// project.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code>
        /// int[] numbers = { 12, 34, 56, 78 };
        /// IEnumerable&lt;int&gt; result = numbers.TakeLast(2);
        /// </code>
        /// The <c>result</c> variable, when iterated over, will yield 
        /// 34, 56 and 78 in turn.
        /// </example>
        public static IEnumerable < TSource > TakeLast < TSource >
            (
            this IEnumerable < TSource > source,
            int count )
        {
            if ( source == null )
            {
                throw new ArgumentNullException ( "source" );
            }
            return TakeLastImpl
                (
                 source,
                 count );
        }

        /// <summary>
        /// Creates a delimited string from a sequence of values. The 
        /// delimiter used depends on the current culture of the executing thread.
        /// </summary>
        /// <remarks>
        /// This operator uses immediate execution and effectively buffers the sequence.
        /// </remarks>
        /// <typeparam name="TSource">Type of element in the source sequence</typeparam>
        /// <param name="source">The sequence of items to delimit. Each is converted to a string using the
        /// simple ToString() conversion.</param>
        /// <remarks>
        /// <para>Borrowed from 
        /// <a href="http://code.google.com/p/morelinq/">MoreLINQ</a>
        /// project.
        /// </para>
        /// </remarks>
        public static string ToDelimitedString < TSource >
            ( this IEnumerable < TSource > source )
        {
            return ToDelimitedString
                (
                 source,
                 null );
        }

        /// <summary>
        /// Creates a delimited string from a sequence of values and
        /// a given delimiter.
        /// </summary>
        /// <remarks>
        /// This operator uses immediate execution and effectively buffers the sequence.
        /// </remarks>
        /// <typeparam name="TSource">Type of element in the source sequence</typeparam>
        /// <param name="source">The sequence of items to delimit. Each is converted to a string using the
        /// simple ToString() conversion.</param>
        /// <param name="delimiter">The delimiter to inject between elements. May be null, in which case
        /// the executing thread's current culture's list separator is used.</param>
        /// <remarks>
        /// <para>Borrowed from 
        /// <a href="http://code.google.com/p/morelinq/">MoreLINQ</a>
        /// project.
        /// </para>
        /// </remarks>
        public static string ToDelimitedString < TSource >
            (
            this IEnumerable < TSource > source,
            string delimiter )
        {
            if ( source == null )
            {
                throw new ArgumentNullException ( "source" );
            }
            return ToDelimitedStringImpl
                (
                 source,
                 delimiter,
                 ( sb,
                   e ) => sb.Append ( e ) );
        }

        /// <summary>
        /// Traces the elements of a source sequence for diagnostics.
        /// </summary>
        /// <typeparam name="TSource">Type of element in the source sequence</typeparam>
        /// <param name="source">Source sequence whose elements to trace.</param>
        /// <returns>
        /// Return the source sequence unmodified.
        /// </returns>
        /// <remarks>
        /// This a pass-through operator that uses deferred execution and 
        /// streams the results.
        /// </remarks>
        public static IEnumerable < TSource > Trace < TSource >
            ( this IEnumerable < TSource > source )
        {
            return Trace
                (
                 source,
                 (string) null );
        }

        /// <summary>
        /// Traces the elements of a source sequence for diagnostics using
        /// custom formatting.
        /// </summary>
        /// <typeparam name="TSource">Type of element in the source sequence</typeparam>
        /// <param name="source">Source sequence whose elements to trace.</param>
        /// <param name="format">
        /// String to use to format the trace message. If null then the
        /// element value becomes the traced message.
        /// </param>
        /// <returns>
        /// Return the source sequence unmodified.
        /// </returns>
        /// <remarks>
        /// This a pass-through operator that uses deferred execution and 
        /// streams the results.
        /// </remarks>
        public static IEnumerable < TSource > Trace < TSource >
            (
            this IEnumerable < TSource > source,
            string format )
        {
            if ( source == null )
            {
                throw new ArgumentNullException ( "source" );
            }

            return TraceImpl
                (
                 source,
                 string.IsNullOrEmpty ( format )
                     ? (Func < TSource, string >) ( x => x == null
                                                             ? string.Empty
                                                             : x.ToString () )
                     : ( x => string.Format
                                  (
                                   format,
                                   x ) ) );
        }

        /// <summary>
        /// Traces the elements of a source sequence for diagnostics using
        /// a custom formatter.
        /// </summary>
        /// <typeparam name="TSource">Type of element in the source sequence</typeparam>
        /// <param name="source">Source sequence whose elements to trace.</param>
        /// <param name="formatter">Function used to format each source element into a string.</param>
        /// <returns>
        /// Return the source sequence unmodified.
        /// </returns>
        /// <remarks>
        /// This a pass-through operator that uses deferred execution and 
        /// streams the results.
        /// </remarks>
        public static IEnumerable < TSource > Trace < TSource >
            (
            this IEnumerable < TSource > source,
            Func < TSource, string > formatter )
        {
            if ( source == null )
            {
                throw new ArgumentNullException ( "source" );
            }
            if ( formatter == null )
            {
                throw new ArgumentNullException ( "formatter" );
            }
            return TraceImpl
                (
                 source,
                 formatter );
        }

        #endregion
    }
}
