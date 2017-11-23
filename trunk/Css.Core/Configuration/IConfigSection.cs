using Css.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Css.Configuration
{
    /// <summary>
    /// A container for settings - key/value pairs where keys are strings, and values are arbitrary objects.
    /// Instances of this class are thread-safe.
    /// </summary>
    public interface IConfigSection
    {
        /// <summary>
        /// 属性变更通知
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
		/// Retrieves a child section container; creating a new one on demand.
		/// Multiple calls to this method will return the same instance (unless the entry at this key
		/// is overwritten by one of the Set-methods).
		/// Changes performed on the nested container will be persisted together with the parent container.
		/// </summary>
        IConfigSection GetSection(string key);

        /// <summary>
		/// Attaches the specified section container as nested section.
		/// 
		/// This method is intended to be used in conjunction with the <see cref="IMementoCapable"/> pattern
		/// where a new unattached section container is created and then later attached to a parent container.
		/// </summary>
        void SetSection(string key, IConfigSection section);

        IReadOnlyList<string> Keys { get; }

        bool Contains(string key);

        string this[string key] { get; }

        T Get<T>(string key, T defaultValue = default(T));

        void Set<T>(string key, T value);

        IReadOnlyList<T> GetList<T>(string key);

        void SetList<T>(string key, IEnumerable<T> value);

        bool Remove(string key);

        void Save(FileName file);
    }
}
