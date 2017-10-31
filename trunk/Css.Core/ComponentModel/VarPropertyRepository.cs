using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// Repostitory for var property. 
    /// </summary>
    public class VarPropertyRepository
    {
        object syncLock = new object();
        static object propertyLock = new object();
        static int _globalIndex = 0;

        public List<VarProperty> Properties { get; } = new List<VarProperty>();

        /// <summary>
        /// Gets the type of the properties's owner
        /// </summary>
        public Type OwnerType { get; }

        internal VarPropertyRepository(Type ownerType)
        {
            OwnerType = ownerType;
        }

        public static void RegisterProperty(VarProperty property)
        {
            if (property == null) throw new ArgumentNullException("property");
            if (property.GlobalIndex >= 0) { throw new InvalidOperationException("同一个属性只能注册一次。"); }

            lock (propertyLock)
                property.GlobalIndex = _globalIndex++;

            var repo = VarTypeRepository.Instance.GetOrCreateVarPropertyRepository(property.OwnerType);
            repo.Properties.Add(property);
        }

        public void RegisterProperty(string propertyName, Type propertyType, bool serializable = true)
        {
            if (propertyName.IsNullOrEmpty()) throw new ArgumentNullException(nameof(propertyName));
            if (propertyType == null) throw new ArgumentNullException(nameof(propertyType));

            var property = new VarProperty(OwnerType, propertyName, propertyType, serializable);

            lock (propertyLock)
                property.GlobalIndex = _globalIndex++;

            Properties.Add(property);
        }

        /// <summary>
        /// 
        /// </summary>
        public VarPropertyRepository BaseRepository { get; internal set; }

        public VarPropertyContainer Container { get; internal set; }

        public bool IsCompiled { get; private set; }

        public void Compile()
        {
            lock (syncLock)
            {
                if (!IsCompiled)
                {
                    Properties.Sort((x, y) => x.Name.CompareTo(y.Name));
                    var container = new VarPropertyContainer(this);
                    container.CompileProperties();
                    Container = container;
                    IsCompiled = true;
                }
            }
        }
    }
}
