using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    [DebuggerDisplay("Count = {_properties.Count}")]
    public class VarPropertyContainer
    {
        VarPropertyRepository Repository { get; }
        Dictionary<string, VarProperty> _properties = new Dictionary<string, VarProperty>();
        List<VarProperty> _readWriteProperties = new List<VarProperty>();

        public Type OwnerType { get { return Repository.OwnerType; } }

        /// <summary>
        /// Gets all properties.
        /// </summary>
        public IEnumerable<VarProperty> Properties { get { return _properties.Values; } }

        /// <summary>
        /// Gets read write properties.
        /// </summary>
        public IEnumerable<VarProperty> ReadWriteProperties { get { return _readWriteProperties; } }

        internal VarPropertyContainer(VarPropertyRepository repo)
        {
            Repository = repo;
        }

        public VarProperty FindProperty(string name)
        {
            VarProperty result = null;
            _properties.TryGetValue(name, out result);
            return result;
        }

        internal void CompileProperties()
        {
            var hierarchy = GetHierarchyRepositories();

            // walk from top to bottom to build consolidated list
            for (int index = hierarchy.Count - 1; index >= 0; index--)
            {
                hierarchy[index].Properties.ForEach(p => _properties[p.Name] = p);
            }

            _readWriteProperties.AddRange(Properties.Where(p => !p.IsReadOnly));
            if (_readWriteProperties.Any() && _readWriteProperties.Last().TypeCompiledIndex == -1)
            {
                for (int i = 0; i < _readWriteProperties.Count; i++)
                    _readWriteProperties[i].TypeCompiledIndex = i;
            }
        }

        void EnumerateHierarchyContainers(Action<VarPropertyRepository> action)
        {
            // get inheritance hierarchy
            var hierarchy = GetHierarchyRepositories();

            // walk from top to bottom to build consolidated list
            for (int index = hierarchy.Count - 1; index >= 0; index--)
            {
                action(hierarchy[index]);
            }
        }

        List<VarPropertyRepository> GetHierarchyRepositories()
        {
            VarPropertyRepository current = Repository;
            var result = new List<VarPropertyRepository>();
            do
            {
                result.Add(current);
                current = current.BaseRepository;
            } while (current != null);
            return result;
        }

        PropertyDescriptorCollection _propertyDescriptorCollection;
        object _lock = new object();

        internal PropertyDescriptorCollection GetPropertyDescriptorCollection()
        {
            if (_propertyDescriptorCollection == null)
            {
                lock (_lock)
                {
                    if (_propertyDescriptorCollection == null)
                    {
                        var pdList = Properties.Select(p => new VarPropertyDescriptor(p));

                        _propertyDescriptorCollection = new PropertyDescriptorCollection(pdList.ToArray());

                        //加入 CLRProperty
                        //为了兼容一些直接使用 CLR 属性而没有使用托管属性编写的视图属性。
                        var clrProperties = OwnerType.GetProperties();
                        foreach (var clrProperty in clrProperties)
                        {
                            if (Properties.All(mp => mp.Name != clrProperty.Name))
                            {
                                _propertyDescriptorCollection.Add(new ClrPropertyDescriptor(clrProperty));
                            }
                        }
                    }
                }
            }
            return _propertyDescriptorCollection;
        }
    }
}
