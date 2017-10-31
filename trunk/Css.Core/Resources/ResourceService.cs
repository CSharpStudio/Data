using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Css.Resources
{
    public class ResourceService : IResourceService
    {
        /// <summary>
        /// The current instance of IResourceService
        /// </summary>
        public static IResourceService Current = new ResourceService();

        List<IResourceObject> _resources = new List<IResourceObject>();

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler CultureChanged;

        public ResourceService()
        {
            Cultures = new List<NameValue>();
        }

        public void Update()
        {
            OnPropertyChanged("Item[]");
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCultureChanged()
        {
            CultureChanged?.Invoke(this, EventArgs.Empty);
        }

        string _culture;
        public virtual string Culture
        {
            get { return _culture; }
            set
            {
                if (!_culture.CIEquals(value))
                {
                    _culture = value;
                    OnCultureChanged();
                    Update();
                }
            }
        }

        /// <summary>
        /// All support cultures list.
        /// </summary>
        public IList<NameValue> Cultures { get; protected set; }

        /// <summary>
        /// Gets the resources text.
        /// </summary>
        public IList<IResourceObject> Resources
        {
            get { return _resources; }
        }

        public virtual string this[string key]
        {
            get { return GetText(Culture, key); }
        }

        /// <summary>
        /// Splite the text. 
        /// If the string is grouping by the ".", gets the last group. eg: "User.Name" will get "Name".
        /// If the string is Pascal style, splite words with ' '. eg: "User.UserName" will get "User Name".
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string SplitText(string str)
        {
            string text = str;
            int index = text.LastIndexOf('.');
            if (index > -1 && index + 1 < text.Length)
                text = text.Substring(index + 1);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (sb.Length > 0 && char.IsUpper(text[i]) && char.IsLower(text[i - 1]))
                    sb.Append(' ');
                sb.Append(text[i]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the resource test for current culture.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string GetText(string culture, string key)
        {
            if (key.IsNullOrEmpty()) return null;
            var r = Resources.FirstOrDefault(p => culture.CIEquals(p.CultureCode) && key.CIEquals(p.Name));
            if (r == null)
                r = Resources.FirstOrDefault(p => key.CIEquals(p.Name));
            if (r != null && r.Value is string)
                return (string)r.Value;

            return SplitText(key);
        }

        public virtual string GetText(CultureInfo culture, string key)
        {
            Check.NotNull(culture, nameof(culture));
            return GetText(culture.Name, key);
        }

        public virtual string GetText(string key)
        {
            return GetText(System.Threading.Thread.CurrentThread.CurrentUICulture, key);
        }

        public virtual object GetObject(string key)
        {
            if (key.IsNullOrEmpty()) return null;
            return Resources.FirstOrDefault(p => key.CIEquals(p.Name));
        }

        public virtual void Register(IEnumerable<IResourceObject> resources)
        {
            _resources.AddRange(resources);
            Update();
        }

        public virtual void Release()
        {
            _resources.Clear();
        }
    }
}
