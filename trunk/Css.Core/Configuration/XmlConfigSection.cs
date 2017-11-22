using Css.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace Css.Configuration
{
    /// <summary>
    /// A container for settings - key/value pairs where keys are strings, and values are arbitrary objects.
    /// Instances of this class are thread-safe.
    /// </summary>
    public class XmlConfigSection : IConfigSection, INotifyPropertyChanged, ICloneable
    {
        // Properties instances form a tree due to the nested properties containers.
        // All nodes in such a tree share the same syncRoot in order to simplify synchronization.
        // When an existing node is added to a tree, its syncRoot needs to change.
        object syncRoot;
        XmlConfigSection parent;
        // Objects in the dictionary are one of:
        // - string: value stored using TypeConverter
        // - XElement: serialized object
        // - object[]: a stored list (array elements are null, string or XElement)
        // - Sections: nested properties container
        Dictionary<string, object> dict = new Dictionary<string, object>();

        public XmlConfigSection()
        {
            this.syncRoot = new object();
        }

        private XmlConfigSection(XmlConfigSection parent)
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
                foreach (var section in dict.Values.OfType<XmlConfigSection>())
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
                    catch (SerializationException ex)
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
        /// <summary>
        /// Retrieves the list of items stored with the specified key.
        /// If no entry with the specified key exists, this method returns an empty list.
        /// </summary>
        /// <remarks>
        /// This method returns a copy of the list used internally; you need to call
        /// <see cref="SetList"/> if you want to store the changed list.
        /// </remarks>
        public IReadOnlyList<T> GetList<T>(string key)
        {
            lock (syncRoot)
            {
                object val;
                if (dict.TryGetValue(key, out val))
                {
                    object[] serializedArray = val as object[];
                    if (serializedArray != null)
                    {
                        try
                        {
                            T[] array = new T[serializedArray.Length];
                            for (int i = 0; i < array.Length; i++)
                            {
                                array[i] = (T)Deserialize(serializedArray[i], typeof(T));
                            }
                            return array;
                        }
                        catch (NotSupportedException ex)
                        {
                            RT.Logger.Warn(ex);
                        }
                    }
                    else
                    {
                        RT.Logger.Warn("XmlConfigSection.GetList(" + key + ") - this entry is not a list");
                    }
                }
                return new T[0];
            }
        }

        /// <summary>
        /// Sets a list of elements in this Properties-container.
        /// The elements will be serialized using a TypeConverter if possible, or XAML serializer otherwise.
        /// </summary>
        /// <remarks>Passing <c>null</c> or an empty list as value has the same effect as calling <see cref="Remove"/>.</remarks>
        public void SetList<T>(string key, IEnumerable<T> value)
        {
            if (value == null)
            {
                Remove(key);
                return;
            }
            T[] array = value.ToArray();
            if (array.Length == 0)
            {
                Remove(key);
                return;
            }
            object[] serializedArray = new object[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                serializedArray[i] = Serialize(array[i], typeof(T), null);
            }
            SetSerializedValue(key, serializedArray);
        }


        /// <summary>
        /// Retrieves a nested property container; creating a new one on demand.
        /// Multiple calls to this method will return the same instance (unless the entry at this key
        /// is overwritten by one of the Set-methods).
        /// Changes performed on the nested container will be persisted together with the parent container.
        /// </summary>
        public IConfigSection GetSection(string key)
        {
            bool isNewContainer = false;
            IConfigSection result;
            lock (syncRoot)
            {
                object oldValue;
                dict.TryGetValue(key, out oldValue);
                result = oldValue as IConfigSection;
                if (result == null)
                {
                    result = new XmlConfigSection(this);
                    dict[key] = result;
                    isNewContainer = true;
                }
            }
            if (isNewContainer)
                OnPropertyChanged(key);
            return result;
        }

        void HandleOldValue(object oldValue)
        {
            XmlConfigSection p = oldValue as XmlConfigSection;
            if (p != null)
            {
                Debug.Assert(p.parent == this);
                p.parent = null;
            }
        }

        /// <summary>
        /// Attaches the specified properties container as nested properties.
        /// 
        /// This method is intended to be used in conjunction with the <see cref="IMementoCapable"/> pattern
        /// where a new unattached properties container is created and then later attached to a parent container.
        /// </summary>
        public void SetSection(string key, IConfigSection section)
        {
            if (section == null)
            {
                Remove(key);
                return;
            }
            XmlConfigSection xmlSection = section as XmlConfigSection;
            lock (syncRoot)
            {
                for (XmlConfigSection ancestor = this; ancestor != null; ancestor = ancestor.parent)
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
            foreach (var properties in dict.Values.OfType<XmlConfigSection>())
            {
                properties.SetSyncRoot(newSyncRoot);
            }
        }

        /// <summary>
        /// Removes the entry (value, list, or nested container) with the specified key.
        /// </summary>
        public bool Remove(string key)
        {
            bool removed = false;
            lock (syncRoot)
            {
                object oldValue;
                if (dict.TryGetValue(key, out oldValue))
                {
                    removed = true;
                    HandleOldValue(oldValue);
                    MakeDirty();
                    dict.Remove(key);
                }
            }
            if (removed)
                OnPropertyChanged(key);
            return removed;
        }

        /// <summary>
        /// Creates a deep clone of this Properties container.
        /// </summary>
        public XmlConfigSection Clone()
        {
            lock (syncRoot)
            {
                return CloneWithParent(null);
            }
        }

        XmlConfigSection CloneWithParent(XmlConfigSection parent)
        {
            XmlConfigSection copy = parent != null ? new XmlConfigSection(parent) : new XmlConfigSection();
            foreach (var pair in dict)
            {
                XmlConfigSection child = pair.Value as XmlConfigSection;
                if (child != null)
                    copy.dict.Add(pair.Key, child.CloneWithParent(copy));
                else
                    copy.dict.Add(pair.Key, pair.Value);
            }
            return copy;
        }

        object ICloneable.Clone()
        {
            return Clone();
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

        object Serialize(object value, Type sourceType, string key)
        {
            if (value == null)
                return null;
            TypeConverter c = TypeDescriptor.GetConverter(sourceType);
            if (c != null && c.CanConvertTo(typeof(string)) && c.CanConvertFrom(typeof(string)))
            {
                return c.ConvertToInvariantString(value);
            }

            var element = new XElement("Object");
            if (key != null)
            {
                element.Add(new XAttribute("key", key));
            }
            element.Value = JsonConvert.SerializeObject(value);
            return element;
        }

        object Deserialize(object serializedVal, Type targetType)
        {
            if (serializedVal == null)
                return null;
            XElement element = serializedVal as XElement;
            if (element != null)
            {
                return JsonConvert.DeserializeObject(element.Value, targetType);
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

        public static XmlConfigSection Load(FileName fileName)
        {
            try
            {
                return Load(XDocument.Load(fileName).Root);
            }
            catch(Exception exc)
            {
                RT.Logger.Warn(exc);
                var section = new XmlConfigSection();
                section.Save(fileName);
                return section;
            }
        }

        public static XmlConfigSection Load(XElement element)
        {
            XmlConfigSection section = new XmlConfigSection();
            section.LoadContents(element.Elements());
            return section;
        }

        void LoadContents(IEnumerable<XElement> elements)
        {
            foreach (var element in elements)
            {
                string key = (string)element.Attribute("key");
                if (key == null)
                    continue;
                switch (element.Name.LocalName)
                {
                    case "Section":
                        dict[key] = element.Value;
                        break;
                    case "Array":
                        dict[key] = LoadArray(element.Elements());
                        break;
                    case "Object":
                        dict[key] = new XElement(element);
                        break;
                    case "Sections":
                        XmlConfigSection child = new XmlConfigSection(this);
                        child.LoadContents(element.Elements());
                        dict[key] = child;
                        break;
                }
            }
        }

        static object[] LoadArray(IEnumerable<XElement> elements)
        {
            List<object> result = new List<object>();
            foreach (var element in elements)
            {
                switch (element.Name.LocalName)
                {
                    case "Null":
                        result.Add(null);
                        break;
                    case "Element":
                        result.Add(element.Value);
                        break;
                    case "Object":
                        result.Add(new XElement(element));
                        break;
                }
            }
            return result.ToArray();
        }

        public void Save(FileName fileName)
        {
            new XDocument(Save()).Save(fileName);
        }

        public XElement Save()
        {
            lock (syncRoot)
            {
                return new XElement("Sections", SaveContents());
            }
        }

        IReadOnlyList<XElement> SaveContents()
        {
            List<XElement> result = new List<XElement>();
            foreach (var pair in dict)
            {
                XAttribute key = new XAttribute("key", pair.Key);
                XmlConfigSection child = pair.Value as XmlConfigSection;
                if (child != null)
                {
                    var contents = child.SaveContents();
                    if (contents.Count > 0)
                        result.Add(new XElement("Sections", key, contents));
                }
                else if (pair.Value is object[])
                {
                    object[] array = (object[])pair.Value;
                    XElement[] elements = new XElement[array.Length];
                    for (int i = 0; i < array.Length; i++)
                    {
                        XElement obj = array[i] as XElement;
                        if (obj != null)
                        {
                            elements[i] = new XElement(obj);
                        }
                        else if (array[i] == null)
                        {
                            elements[i] = new XElement("Null");
                        }
                        else
                        {
                            elements[i] = new XElement("Element", (string)array[i]);
                        }
                    }
                    result.Add(new XElement("Array", key, elements));
                }
                else if (pair.Value is XElement)
                {
                    result.Add(new XElement((XElement)pair.Value));
                }
                else
                {
                    result.Add(new XElement("Section", key, (string)pair.Value));
                }
            }
            return result;
        }
    }
}
