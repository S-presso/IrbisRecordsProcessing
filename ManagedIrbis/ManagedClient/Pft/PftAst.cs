/* PftAst.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime.Tree;

#endregion

namespace ManagedClient.Pft
{
    /// <summary>
    /// Абстрактный элемент синтаксического дерева.
    /// </summary>
    [Serializable]
    public abstract class PftAst
    {
        #region Properties

        public string Text { get; set; }

        public List<PftAst> Children { get { return _children; } }

        #endregion

        #region Construction

        protected PftAst()
        {
            _children = new List<PftAst>();
        }

        protected PftAst
            (
                IParseTree node
            )
            : this()
        {
            Text = node.GetText();
        }

        #endregion

        #region Private members

        protected List<PftAst> _children;

        #endregion

        #region Public methods

        /// <summary>
        /// Собственно форматирование
        /// </summary>
        /// <param name="context"></param>
        public virtual void Execute
            (
                PftContext context
            )
        {
            foreach (PftAst child in Children)
            {
                child.Execute(context);
            }
        }

        public virtual PftAst Optimize()
        {
            if (_children.Count == 1)
            {
                return _children[0].Optimize();
            }
            return this;
        }

        public List<T> GetDescendants<T>()
            where T : PftAst
        {
            List<T> result = new List<T>();

            foreach (PftAst child in Children)
            {
                if (child is T)
                {
                    result.Add((T)child);
                }
                result.AddRange(child.GetDescendants<T>());
            }

            return result;
        }

        /// <summary>
        /// Семантическая валидация поддерева.
        /// </summary>
        /// <param name="throwOnError"></param>
        /// <returns></returns>
        public virtual bool Validate
            (
                bool throwOnError
            )
        {
            bool result = Children
                .All
                (
                    child => child.Validate(throwOnError)
                );
            if (!result && throwOnError)
            {
                throw new ArgumentException();
            }
            return result;
        }

        public void PrintDebug
            (
                TextWriter writer,
                int level
            )
        {
            for (int i = 0; i < level; i++)
            {
                writer.Write("| ");
            }
            writer.WriteLine
                (
                    "{0}: {1}",
                    GetType().Name,
                    Text
                );
            foreach (PftAst child in Children)
            {
                child.PrintDebug(writer, level + 1);
            }
        }

        #endregion

        #region Object members

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            foreach (PftAst child in Children)
            {
                result.Append(child);
            }
            return result.ToString();
        }

        #endregion
    }
}
