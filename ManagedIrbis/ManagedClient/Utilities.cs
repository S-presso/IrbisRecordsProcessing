/* Utilities.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace ManagedClient
{
    /// <summary>
    /// Несколько утилит, упрощающих код.
    /// </summary>
    static class Utilities
    {
        /// <summary>
        /// Выборка элемента из массива.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="occurrence"></param>
        /// <returns></returns>
        public static T GetOccurrence<T>
            (
                this T[] array,
                int occurrence
            )
        {
            occurrence = (occurrence >= 0)
                            ? occurrence
                            : array.Length + occurrence;
            T result = default(T);
            if ((occurrence >= 0) && (occurrence < array.Length))
            {
                result = array[occurrence];
            }
            return result;
        }

        public static T GetOccurrence<T>
            (
                this IList<T> list,
                int occurrence
            )
        {
            occurrence = (occurrence >= 0)
                            ? occurrence
                            : list.Count + occurrence;
            T result = default(T);
            if ((occurrence >= 0) && (occurrence < list.Count))
            {
                result = list[occurrence];
            }
            return result;
        }

        /// <summary>
        /// Отбирает из последовательности только
        /// ненулевые элементы.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerable<T> NonNullItems<T>
            (
                this IEnumerable<T> sequence
            )
            where T: class
        {
            return sequence.Where(value => value != null);
        }

        /// <summary>
        /// Отбирает из последовательности только непустые строки.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static IEnumerable<string> NonEmptyLines
            (
                this IEnumerable<string> sequence
            )
        {
            return sequence.Where(line => !string.IsNullOrEmpty(line));
        }

        /// <summary>
        /// Разбивает строку по указанному разделителю.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[] SplitFirst
            (
                this string line,
                char delimiter
            )
        {
            int index = line.IndexOf(delimiter);
            string[] result = (index < 0)
                                  ? new[] { line }
                                  : new[] { line.Substring(0, index), line.Substring(index + 1) };
            return result;
        }

        /// <summary>
        /// Сравнивает строки с точностью до регистра.
        /// </summary>
        /// <param name="one">Первая строка.</param>
        /// <param name="two">Вторая строка.</param>
        /// <returns>Строки совпадают с точностью до регистра.</returns>
        public static bool SameString
            (
                this string one,
                string two
            )
        {
            return string.Compare(one, two, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Проверяет, является ли искомая строка одной
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомая строка.</param>
        /// <param name="many">Источник проверяемых строк.</param>
        /// <returns>Найдена ли искомая строка.</returns>
        public static bool OneOf
            (
                this string one,
                IEnumerable<string> many
            )
        {
            return many
                .Any(_ => _.SameString(one));
        }

        /// <summary>
        /// Проверяет, является ли искомая строка одной
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомая строка.</param>
        /// <param name="many">Массив проверяемых строк.</param>
        /// <returns>Найдена ли искомая строка.</returns>
        public static bool OneOf
            (
               this string one,
               params string[] many
            )
        {
            return one.OneOf(many.AsEnumerable());
        }

        /// <summary>
        /// Проверяет, является ли искомый символ одним
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомый символ.</param>
        /// <param name="many">Массив проверяемых символов.</param>
        /// <returns>Найден ли искомый символ.</returns>
        public static bool OneOf
            (
                this char one,
                IEnumerable<char> many
            )
        {
            return many
                .Any(_ => _.SameChar(one));
        }

        /// <summary>
        /// Проверяет, является ли искомый символ одним
        /// из перечисленных. Регистр символов не учитывается.
        /// </summary>
        /// <param name="one">Искомый символ.</param>
        /// <param name="many">Массив проверяемых символов.</param>
        /// <returns>Найден ли искомый символ.</returns>
        public static bool OneOf
            (
                this char one,
                params char[] many
            )
        {
            return one.OneOf(many.AsEnumerable());
        }

        /// <summary>
        /// Сравнивает символы с точностью до регистра.
        /// </summary>
        /// <param name="one">Первый символ.</param>
        /// <param name="two">Второй символ.</param>
        /// <returns>Символы совпадают с точностью до регистра.</returns>
        public static bool SameChar
            (
                this char one,
                char two
            )
        {
#if PocketPC
			return (char.ToUpper(one) == char.ToUpper(two));
#else
            return (char.ToUpperInvariant(one) == char.ToUpperInvariant(two));
#endif
        }

        /// <summary>
        /// Представляет ли строка положительное целое число.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsPositiveInteger
            (
                this string text
            )
        {
            return (text.SafeParseInt32(0) > 0);
        }

        /// <summary>
        /// Безопасный парсинг целого числа.
        /// </summary>
        /// <param name="text">Строка, подлежащая парсингу.</param>
        /// <param name="defaultValue">Значение по умолчанию.</param>
        /// <returns>Разобранное целое число или значение по умолчанию.</returns>
        public static int SafeParseInt32
            (
                this string text,
                int defaultValue
            )
        {
            int result = defaultValue;

            try
            {
                result = int.Parse(text);
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            {
                // Do nothing
            }

            //if (!Int32.TryParse(text, out result))
            //{
            //    result = defaultValue;
            //}
            return result;
        }

        /// <summary>
        /// Безопасный парсинг целого числа.
        /// </summary>
        /// <param name="text">Строка, подлежащая парсингу.</param>
        /// <returns>Разобранное целое число или значение по умолчанию.</returns>
        public static int SafeParseInt32
            (
                this string text
            )
        {
            return SafeParseInt32
                (
                    text,
                    0
                 );
        }

        /// <summary>
        /// Преобразование числа в строку по правилам инвариантной 
        /// (не зависящей от региона) культуры.
        /// </summary>
        /// <param name="value">Число для преобразования.</param>
        /// <returns>Строковое представление числа.</returns>
        public static string ToInvariantString
            (
                this int value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToInvariantString
            (
                this char value
            )
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Форматирование диапазона целых чисел.
        /// </summary>
        /// <remarks>Границы диапазона могут совпадать, однако
        /// левая не должна превышать правую.</remarks>
        /// <param name="first">Левая граница диапазона.</param>
        /// <param name="last">Правая граница диапазона.</param>
        /// <returns>Строковое представление диапазона.</returns>
        public static string FormatRange
            (
                int first,
                int last
            )
        {
            if (first == last)
            {
                return first.ToInvariantString();
            }
            if (first == (last - 1))
            {
                return (first.ToInvariantString() + ", " + last.ToInvariantString());
            }
            return (first.ToInvariantString() + "-" + last.ToInvariantString());
        }

        /// <summary>
        /// Преобразование набора целых чисел в строковое представление,
        /// учитывающее возможное наличие цепочек последовательных чисел,
        /// которые форматируются как диапазоны.
        /// </summary>
        /// <param name="n">Источник целых чисел.</param>
        /// <remarks>Источник должен поддерживать многократное считывание.
        /// Числа предполагаются предварительно упорядоченные. Повторения чисел
        /// не допускаются. Пропуски в последовательностях допустимы.
        /// Числа допускаются только неотрицательные.
        /// </remarks>
        /// <returns>Строковое представление набора чисел.</returns>
        public static string CompressRange
            (
                IEnumerable<int> n
            )
        {
            if (n == null)
            {
                return string.Empty;
            }

            // ReSharper disable PossibleMultipleEnumeration
            if (!n.Any())
            {
                return String.Empty;
            }

            var result = new StringBuilder();
            var first = true;
            var prev = n.First();
            var last = prev;
            foreach (var i in n.Skip(1))
            {
                if (i != (last + 1))
                {
                    result.AppendFormat("{0}{1}", (first ? "" : ", "),
                        FormatRange(prev, last));
                    prev = i;
                    first = false;
                }
                last = i;
            }
            result.AppendFormat("{0}{1}", (first ? "" : ", "),
                FormatRange(prev, last));

            return result.ToString();
            // ReSharper restore PossibleMultipleEnumeration
        }

        /// <summary>
        /// Разбивка текста на отдельные строки.
        /// </summary>
        /// <remarks>Пустые строки не удаляются.</remarks>
        /// <param name="text">Текст для разбиения.</param>
        /// <returns>Массив строк.</returns>
        public static string[] SplitLines
            (
                this string text
            )
        {
            text = text.Replace("\r\n", "\r");

            return text.Split
                (
                    '\r'
                );
        }

        /// <summary>
        /// Склейка строк в сплошной текст, разделенный переводами строки.
        /// </summary>
        /// <param name="lines">Строки для склейки.</param>
        /// <returns>Склеенный текст.</returns>
        public static string MergeLines
            (
                this IEnumerable<string> lines
            )
        {
            string result = string.Join
                (
                    Environment.NewLine,
                    lines.ToArray()
                );
            return result;
        }

        /// <summary>
        /// Считывает из потока максимально возможное число байт.
        /// </summary>
        /// <remarks>Полезно для считывания из сети (сервер высылает
        /// ответ, после чего закрывает соединение).</remarks>
        /// <param name="stream">Поток для чтения.</param>
        /// <returns>Массив считанных байт.</returns>
        public static byte[] ReadToEnd
            (
                this Stream stream
            )
        {
            MemoryStream result = new MemoryStream();

            while (true)
            {
                byte[] buffer = new byte[10 * 1024];
                int read = stream.Read(buffer, 0, buffer.Length);
                if (read <= 0)
                {
                    break;
                }
                result.Write(buffer, 0, read);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Шестнадцатиричный дамп массива байт.
        /// </summary>
        /// <param name="writer">Куда писать.</param>
        /// <param name="buffer">Байты.</param>
        /// <param name="offset">Начальное смещение.</param>
        /// <param name="count">Количество байт для дампа.</param>
        public static void DumpBytes
            (
                TextWriter writer,
                byte[] buffer,
                int offset,
                int count
            )
        {
            for (int i = 0; i < count; i++)
            {
                if (i != 0)
                {
                    writer.Write(" ");
                }
                writer.Write("{0:X2}", buffer[offset + i]);
            }
            writer.WriteLine();
        }
    }
}
