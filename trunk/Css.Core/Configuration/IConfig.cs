﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Css.Configuration
{
    /// <summary>
    /// 配置接口，键值对存储
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// 
        /// </summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the main section container for this config service.
        /// </summary>
        IConfigSection Section { get; }

        /// <inheritdoc cref="IConfigSection.Get{T}(string, T)"/>
        T Get<T>(string key, T defaultValue = default(T));

        /// <inheritdoc cref="IConfigSection.GetSection"/>
        IConfigSection GetSection(string key);

        /// <inheritdoc cref="IConfigSection.SetSection"/>
        void SetSection(string key, IConfigSection section);

        /// <inheritdoc cref="IConfigSection.Contains"/>
        bool Contains(string key);

        /// <inheritdoc cref="IConfigSection.Set{T}(string, T)"/>
        void Set<T>(string key, T value);

        /// <inheritdoc cref="IConfigSection.GetList"/>
        IReadOnlyList<T> GetList<T>(string key);

        /// <inheritdoc cref="IConfigSection.SetList"/>
        void SetList<T>(string key, IEnumerable<T> value);

        /// <inheritdoc cref="IConfigSection.Remove"/>
        void Remove(string key);

        /// <summary>
        /// Saves the main section to disk.
        /// </summary>
        void Save();
    }
}
