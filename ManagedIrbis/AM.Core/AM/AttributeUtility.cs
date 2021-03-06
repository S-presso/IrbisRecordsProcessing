/* AttributeUtility.cs -- Helper class to simplify runtime attribute retrieving.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Linq;
using System.Reflection;

#endregion

namespace AM
{
    /// <summary>
    /// Helper class to simplify runtime attribute retrieving
    /// </summary>
    public static class AttributeUtility
    {
        /// <summary>
        /// Retrives custom <see cref="Attribute"/> from
        /// <see cref="MemberInfo"/> <paramref name="member"/>.
        /// </summary>
        /// <typeparam name="T">Type of the attribute to retrieve.</typeparam>
        /// <param name="member">Member to examine to.</param>
        /// <returns>Attribute or <c>null</c>.</returns>
        public static T GetAttribute < T > ( this MemberInfo member )
            where T : Attribute
        {
            ArgumentUtility.NotNull
                (
                 member,
                 "member" );

            return member.GetCustomAttributes
                (
                 typeof ( T ),
                 true )
                         .Cast < T > ()
                         .FirstOrDefault ();
        }

        /// <summary>
        /// Gets the attribute member.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="member">The member.</param>
        /// <param name="retriever">The retriever.</param>
        /// <returns></returns>
        public static T2 GetAttributeMember < T1, T2 >
            (
            this MemberInfo member,
            Func < T1, T2 > retriever ) where T1 : Attribute
        {
            ArgumentUtility.NotNull
                (
                 member,
                 "member" );
            ArgumentUtility.NotNull
                (
                 retriever,
                 "retriever" );

            return retriever ( GetAttribute < T1 > ( member ) );
        }

        /// <summary>
        /// Gets the attribute member.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <typeparam name="T2">The type of the 2.</typeparam>
        /// <param name="member">The member.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="retriever">The retriever.</param>
        /// <returns></returns>
        public static T2 GetAttributeMember < T1, T2 >
            (
            this MemberInfo member,
            T2 defaultValue,
            Func < T1, T2 > retriever ) where T1 : Attribute
        {
            ArgumentUtility.NotNull
                (
                 member,
                 "member" );
            ArgumentUtility.NotNull
                (
                 retriever,
                 "retriever" );

            T1 attribute = GetAttribute < T1 > ( member );
            return ( attribute == null )
                       ? defaultValue
                       : retriever ( attribute );
        }
    }
}
