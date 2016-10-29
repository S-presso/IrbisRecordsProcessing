/* BinaryTree.cs -- Simple binary tree.
   Ars Magna project, https://www.assembla.com/spaces/arsmagna */

#region Using directives

using System;
using System.Diagnostics;

#endregion

namespace AM.Collections
{
    /// <summary>
    /// Simple binary tree.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    [CLSCompliant ( false )]
    public class BinaryTree < T >
    {
        #region Nested classes

        /// <summary>
        /// Node handler.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public delegate bool NodeHandler ( Node node );

        /// <summary>
        /// Node value handler.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate bool ValueHandler ( T value );

        /// <summary>
        /// Node values comparer.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public delegate int ValueComparer ( T left,
                                            T right );

        /// <summary>
        /// Tree node.
        /// </summary>
        [Serializable]
        //[DebuggerTypeProxy ( typeof ( Node.Proxy ) )]
        public class Node
        {
            #region Properties

            /// <summary>
            /// Node value.
            /// </summary>
            public T Value;

            private Node _leftChild;

            /// <summary>
            /// Left child node.
            /// </summary>
            /// <value></value>
            public Node LeftChild
            {
                [DebuggerStepThrough]
                get
                {
                    return _leftChild;
                }
                [DebuggerStepThrough]
                set
                {
                    _leftChild = value;
                    if ( value != null )
                    {
                        value._parent = this;
                    }
                }
            }

            private Node _rightChild;

            /// <summary>
            /// Right child node.
            /// </summary>
            /// <value></value>
            public Node RightChild
            {
                [DebuggerStepThrough]
                get
                {
                    return _rightChild;
                }
                [DebuggerStepThrough]
                set
                {
                    _rightChild = value;
                    if ( value != null )
                    {
                        value._parent = this;
                    }
                }
            }

            private Node _parent;

            /// <summary>
            /// Parent node.
            /// </summary>
            /// <value></value>
            public Node Parent
            {
                [DebuggerStepThrough]
                get
                {
                    return _parent;
                }
            }

            #endregion

            #region Construction

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Node"/> class.
            /// </summary>
            /// <param name="value">The value.</param>
            public Node ( T value )
            {
                Value = value;
            }

            #endregion

            #region Private members

            private static bool _ForEach
                (
                NodeHandler handler,
                Node node )
            {
                bool result = true;
                if ( node != null )
                {
                    result = _ForEach
                        (
                         handler,
                         node.LeftChild );
                    if ( result )
                    {
                        result = handler ( node );
                    }
                    if ( result )
                    {
                        result = _ForEach
                            (
                             handler,
                             node.RightChild );
                    }
                }
                return result;
            }

            private static bool _ForEach
                (
                ValueHandler handler,
                Node node )
            {
                bool result = true;
                if ( node != null )
                {
                    result = _ForEach
                        (
                         handler,
                         node.LeftChild );
                    if ( result )
                    {
                        result = handler ( node.Value );
                    }
                    if ( result )
                    {
                        result = _ForEach
                            (
                             handler,
                             node.RightChild );
                    }
                }
                return result;
            }

            #endregion

            #region Public methods

            /// <summary>
            /// Executes delegate for this node and each child node.
            /// </summary>
            /// <param name="handler"></param>
            /// <returns></returns>
            public bool ForEach ( NodeHandler handler )
            {
                return _ForEach
                    (
                     handler,
                     this );
            }

            /// <summary>
            /// Executes delegate for this node and each child node.
            /// </summary>
            /// <param name="handler"></param>
            /// <returns></returns>
            public bool ForEach ( ValueHandler handler )
            {
                return _ForEach
                    (
                     handler,
                     this );
            }

            /// <summary>
            /// Add left child node.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public Node AddLeftChild ( T value )
            {
                LeftChild = new Node ( value );
                return LeftChild;
            }

            /// <summary>
            /// Add right child node.
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public Node AddRightChild ( T value )
            {
                RightChild = new Node ( value );
                return RightChild;
            }

            #endregion

            #region Debugger proxy class

            private class Proxy
            {
                private Node _node;

                public T Value
                {
                    get
                    {
                        return _node.Value;
                    }
                }

                public Node LeftChild
                {
                    get
                    {
                        return _node.LeftChild;
                    }
                }

                public Node RightChild
                {
                    get
                    {
                        return _node.RightChild;
                    }
                }

                public Node Parent
                {
                    get
                    {
                        return _node.Parent;
                    }
                }

                public Proxy ( Node node )
                {
                    _node = node;
                }
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// Root node.
        /// </summary>
        public Node Root;

        #endregion

        #region Construction

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BinaryTree ( )
        {
        }

        /// <summary>
        /// Constructs tree with given root node.
        /// </summary>
        /// <param name="node"></param>
        public BinaryTree ( Node node )
        {
            Root = node;
        }

        /// <summary>
        /// Constructs tree with root node with given value.
        /// </summary>
        /// <param name="value"></param>
        public BinaryTree ( T value )
        {
            Root = new Node ( value );
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Executes delegate for each node in the tree.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool ForEach ( NodeHandler handler )
        {
            return ( Root == null )
                       ? true
                       : Root.ForEach ( handler );
        }

        /// <summary>
        /// Executes delegate for each node in the tree.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public bool ForEach ( ValueHandler handler )
        {
            return ( Root == null )
                       ? true
                       : Root.ForEach ( handler );
        }

        #endregion
    }
}

#if false

    // Ïðèìåð èñïîëüçîâàíèÿ

using System;
using AM.Collections;

class Class1
{
		static void Main ()
		{
			BinaryTree<int> tree = new BinaryTree<int> ( 0 );

			tree.Root.AddLeftChild ( 1 ).AddLeftChild ( 3 );
			tree.Root.AddRightChild ( 2 );
			tree.ForEach ( MyValueHandler );
		}

		static bool MyValueHandler ( int value )
		{
			Console.WriteLine ( value );
			return true;
		}
}

// Íàïå÷àòàåò:

3
1
0
2

#endif
