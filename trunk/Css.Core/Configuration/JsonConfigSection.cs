using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Css.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Css.Configuration
{
    /// <summary>
    /// A container for settings - key/value pairs where keys are strings, and values are arbitrary objects.
    /// Instances of this class are thread-safe.
    /// </summary>
    public class JsonConfigSection : IConfigSection, INotifyPropertyChanged, ICloneable
    {

        // Properties instances form a tree due to the nested properties containers.
        // All nodes in such a tree share the same syncRoot in order to simplify synchronization.
        // When an existing node is added to a tree, its syncRoot needs to change.
        object syncRoot;
        JsonConfigSection parent;
        Dictionary<string, object> dict = new Dictionary<string, object>();

        public JsonConfigSection()
        {
            this.syncRoot = new object();
        }

        private JsonConfigSection(JsonConfigSection parent)
        {
            this.parent = parent;
            this.syncRoot = parent.syncRoot;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string key)
        {
            Volatile.Read(ref PropertyChanged)?.Invoke(this, new PropertyChangedEventArgs(key));
        }


        bool isDirty;
        /// <summary>
        /// Gets/Sets whether this properties container is dirty.
        /// IsDirty automatically gets set to <c>true</c> when a property in this container (or a nested container)
        /// changes.
        /// </summary>
        public bool IsDirty
        {
            get { return isDirty; }
            set
            {
                lock (syncRoot)
                {
                    if (value)
                        MakeDirty();
                    else
                        CleanDirty();
                }
            }
        }

        void MakeDirty()
        {
            // called within syncroot
            if (!isDirty)
            {
                isDirty = true;
                if (parent != null)
                    parent.MakeDirty();
            }
        }

        void CleanDirty()
        {
            if (isDirty)
            {
                isDirty = false;
                foreach (var section in dict.Values.OfType<JsonConfigSection>())
                {
                    section.CleanDirty();
                }
            }
        }

        /// <summary>
        /// Retrieves a string value from this Properties-container.
        /// Using this indexer is equivalent to calling <c>Get(key, string.Empty)</c>.
        /// </summary>
        public string this[string key]
        {
            get
            {
                lock (syncRoot)
                {
                    object val;
                    dict.TryGetValue(key, out val);
                    return val as string ?? string.Empty;
                }
            }
            set
            {
                Set(key, value);
            }
        }


        /// <summary>
        /// Gets the keys that are in use by this properties container.
        /// </summary>
        public IReadOnlyList<string> Keys
        {
            get
            {
                lock (syncRoot)
                {
                    return dict.Keys.ToArray();
                }
            }
        }

        /// <summary>
        /// Gets whether this properties instance contains any entry (value, list, or nested container)
        /// with the specified key.
        /// </summary>
        public bool Contains(string key)
        {
            lock (syncRoot)
            {
                return dict.ContainsKey(key);
            }
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves a single element from this Properties-container.
        /// </summary>
        /// <param name="key">Key of the item to retrieve</param>
        /// <param name="defaultValue">Default value to be returned if the key is not present.</param>
        public T Get<T>(string key, T defaultValue = default(T))
        {
            lock (syncRoot)
            {
                object val;
                if (dict.TryGetValue(key, out val))
                {
                    try
                    {
                        return (T)Deserialize(val, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        RT.Logger.Warn(ex);
                        return defaultValue;
                    }
                }
                else
                {
                    return defaultValue;
                }
            }
        }

        public IReadOnlyList<T> GetList<T>(string key)
        {
            throw new NotImplementedException();
        }

        public IConfigSection GetSection(string key)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets a single element in this Properties-container.
        /// The element will be serialized using a TypeConverter if possible, or Json serializer otherwise.
        /// </summary>
        /// <remarks>Setting a key to <c>null</c> has the same effect as calling <see cref="Remove"/>.</remarks>
        public void Set<T>(string key, T value)
        {
            object serializedValue = Serialize(value, typeof(T), key);
            SetSerializedValue(key, serializedValue);
        }

        public void SetList<T>(string key, IEnumerable<T> value)
        {
            object serializedValue = Serialize(value, typeof(T), key);
            SetSerializedValue(key, serializedValue);
        }

        public void SetSection(string key, IConfigSection section)
        {
            if (section == null)
            {
                Remove(key);
                return;
            }
            JsonConfigSection xmlSection = section as JsonConfigSection;
            lock (syncRoot)
            {
                for (JsonConfigSection ancestor = this; ancestor != null; ancestor = ancestor.parent)
                {
                    if (ancestor == xmlSection)
                        throw new InvalidOperationException("Cannot add a section container to itself.");
                }

                object oldValue;
                if (dict.TryGetValue(key, out oldValue))
                {
                    if (oldValue == xmlSection)
                        return;
                    HandleOldValue(oldValue);
                }
                lock (xmlSection.syncRoot)
                {
                    if (xmlSection.parent != null)
                        throw new InvalidOperationException("Cannot attach nested section that already have a parent.");
                    MakeDirty();
                    xmlSection.SetSyncRoot(syncRoot);
                    xmlSection.parent = this;
                    dict[key] = xmlSection;
                }
            }
            OnPropertyChanged(key);
        }

        void SetSyncRoot(object newSyncRoot)
        {
            this.syncRoot = newSyncRoot;
            foreach (var section in dict.Values.OfType<JsonConfigSection>())
            {
                section.SetSyncRoot(newSyncRoot);
            }
        }

        object Serialize(object value, Type sourceType, string key)
        {
            if (value == null)
                return null;
            return JToken.FromObject(value);
        }

        object Deserialize(object serializedVal, Type targetType)
        {
            if (serializedVal == null)
                return null;
            var element = serializedVal as JToken;
            if (element != null)
            {
                return element.ConvertTo(targetType);
            }
            else
            {
                string text = serializedVal as string;
                if (text == null)
                    throw new InvalidOperationException("Cannot read a section container as a single value");
                TypeConverter c = TypeDescriptor.GetConverter(targetType);
                return c.ConvertFromInvariantString(text);
            }
        }

        void SetSerializedValue(string key, object serializedValue)
        {
            if (serializedValue == null)
            {
                Remove(key);
                return;
            }
            lock (syncRoot)
            {
                object oldValue;
                if (dict.TryGetValue(key, out oldValue))
                {
                    if (object.Equals(serializedValue, oldValue))
                        return;
                    HandleOldValue(oldValue);
                }
                dict[key] = serializedValue;
            }
            OnPropertyChanged(key);
        }

        void HandleOldValue(object oldValue)
        {
            JsonConfigSection p = oldValue as JsonConfigSection;
            if (p != null)
            {
                Debug.Assert(p.parent == this);
                p.parent = null;
            }
        }

        public static JsonConfigSection Load(FileName fileName)
        {
            try
            {
                JsonConfigSection section = new JsonConfigSection();
                using (var sr = new StreamReader(fileName))
                {
                    var json = sr.ReadToEnd();
                    section.Load(json);
                    return section;
                }
            }
            catch (Exception exc)
            {
                RT.Logger.Warn(exc);
                var section = new JsonConfigSection();
                section.Save(fileName);
                return section;
            }
        }

        public void Load(string json)
        {
            var obj = JObject.Parse(json);
            Load(obj);
        }


        void Load(JObject obj)
        {
            foreach (var p in obj.Properties())
            {
                if (p.Name == "_sections")
                {
                    JsonConfigSection section = new JsonConfigSection();
                    section.Load((JObject)p.Value);
                    dict[p.Name] = section;
                }
                else
                {
                    dict[p.Name] = p.Value;
                }
            }
        }

        public void Save(FileName fileName)
        {
            using (var sw = new StreamWriter(fileName))
            {
                sw.Write(Save());
            }
        }

        public string Save()
        {
            lock (syncRoot)
            {
                var content = SaveContents();
                return content.ToString();
            }
        }

        string GetValue(JToken token)
        {
            if (token.Type == JTokenType.String)
                return "\"{0}\"".FormatArgs(token);
            return token.ToString();
        }

        JObject SaveContents()
        {
            JObject obj = new JObject();
            Dictionary<string, JsonConfigSection> children = new Dictionary<string, JsonConfigSection>();
            foreach (var pair in dict)
            {
                JsonConfigSection child = pair.Value as JsonConfigSection;
                if (child != null)
                {
                    children.Add(pair.Key, child);
                }
                else
                {
                    JToken token = pair.Value as JToken ?? JToken.FromObject(pair.Value);
                    obj.Add(pair.Key, token);
                }
            }
            if (children.Any())
            {
                JObject section = new JObject();
                foreach (var child in children)
                {
                    section.Add(child.Key, child.Value.SaveContents());
                }
                obj.Add("_sections", section);
            }
            return obj;
        }
    }
}
