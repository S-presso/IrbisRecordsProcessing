/* PftCache.cs
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace ManagedClient.Pft
{
    [Serializable]
    public sealed class PftCache
    {
        #region Properties

        public ManagedClient64 Client { get; set; }

        #endregion

        #region Construction

        public PftCache()
        {
            _dictionary = new Dictionary<string, PftProgram>
                (
                    StringComparer.InvariantCultureIgnoreCase
                );
        }

        public PftCache
            (
                ManagedClient64 client
            )
            : this ()
        {
            Client = client;
        }

        #endregion

        #region Private members

        private readonly Dictionary<string, PftProgram> _dictionary;

        #endregion

        #region Public methods

        public void Clear()
        {
            _dictionary.Clear();
        }

        public static string ComposeKey
            (
                string database,
                IrbisPath path,
                string filename
            )
        {
            return string.Format
                (
                    "{0}.{1}.{2}",
                    database,
                    path,
                    filename
                );
        }

        public PftProgram GetProgram
            (
                string database,
                IrbisPath path,
                string filename
            )
        {
            Preload(database, path, filename);
            string key = ComposeKey(database, path, filename);
            return _dictionary[key];
        }

        public static PftCache Load
            (
                string filename
            )
        {
            return new PftCache();
        }

        public PftCache Preload
            (
                string database,
                IrbisPath path,
                string filename
            )
        {
            string key = ComposeKey(database, path, filename);
            if (!_dictionary.ContainsKey(key))
            {
                // TODO Load and compile script
            }
            return this;
        }

        public void Save
            (
                string filename
            )
        {
            
        }

        #endregion
    }
}
