using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Css.Configuration
{
    class XmlConfig : IConfig
    {
        object syncRoot;
        XmlConfig parent;
        // Objects in the dictionary are one of:
        // - string: value stored using TypeConverter
        // - XElement: serialized object
        // - object[]: a stored list (array elements are null, string or XElement)
        // - Properties: nested section container
        Dictionary<string, object> dict = new Dictionary<string, object>();

        public IConfigSection Section => throw new NotImplementedException();

        #region Constructor
        public XmlConfig()
        {
            this.syncRoot = new object();
        }

        private XmlConfig(XmlConfig parent)
        {
            this.parent = parent;
            this.syncRoot = parent.syncRoot;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string key)
        {
            Volatile.Read(ref PropertyChanged)?.Invoke(this, new PropertyChangedEventArgs(key));
        }

        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key, T defaultValue)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<T> GetList<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IConfigSection GetSection(string key)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }

        public void SetList<T>(string key, IEnumerable<T> value)
        {
            throw new NotImplementedException();
        }

        public void SetSection(string key, IConfigSection section)
        {
            throw new NotImplementedException();
        }
    }
}
