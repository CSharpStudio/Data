using Css.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Css.Resources
{
    /// <summary>
    /// 资源服务，这个服务不会注册到IoC容器里，可通过<see cref="AppRuntime.ResourceService"/>访问
    /// </summary>
    public interface IResourceService : INotifyPropertyChanged
    {
        event EventHandler CultureChanged;
        /// <summary>
        /// A specific culture.
        /// </summary>
        string Culture { get; set; }
        /// <summary>
        /// Support cultures.
        /// </summary>
        IList<NameValue> Cultures { get; }
        /// <summary>
        /// Get the culture text with the key.
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <param name="culture">The culture name</param>
        /// <returns></returns>
        string GetText(string culture, string key);
        /// <summary>
        /// Get the culture text with the key.
        /// </summary>
        /// <param name="key">The resource key</param>
        /// <param name="culture">The culture info</param>
        /// <returns></returns>
        string GetText(CultureInfo culture, string key);
        /// <summary>
        /// Get current thread culture resource text with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetText(string key);
        /// <summary>
        /// Get resource object with the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetObject(string key);
        /// <summary>
        /// Registers resources in the resource service.
        /// </summary>
        /// <param name="resource"></param>
        void Register(IEnumerable<IResourceObject> resources);
        /// <summary>
        /// Release all resources.
        /// </summary>
        void Release();
    }
}
