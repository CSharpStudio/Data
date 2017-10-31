using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    public class VarTypeRepository
    {
        public static VarTypeRepository Instance = new VarTypeRepository();

        VarTypeRepository() { }
        object syncLock = new object();
        Dictionary<string, VarPropertyRepository> _types = new Dictionary<string, VarPropertyRepository>();

        public static event EventHandler<ComplieEventArgs> Compiling;

        void OnCompile(VarPropertyRepository repo)
        {
            Compiling?.Invoke(this, new ComplieEventArgs(repo));
        }

        public VarPropertyContainer GetVarPropertyContainer(Type type)
        {
            var repo = GetOrCreateVarPropertyRepository(type);
            if (repo != null)
            {
                if (!repo.IsCompiled)
                {
                    RunPropertyResigtry(type);
                    OnCompile(repo);
                    repo.Compile();
                }

                return repo.Container;
            }

            return null;
        }

        public static bool RunPropertyResigtry(Type type)
        {
            //泛型类在绑定具体类型前，是无法初始化它的静态字段的，所以这里直接退出，而留待子类来进行初始化。
            if (type.ContainsGenericParameters)
            {
                if (!type.IsAbstract)
                {
                    throw new InvalidOperationException("声明可变对象属性的泛型类型 {0}，必须声明为 abstract，否则无法正常使用可变对象属性！".FormatArgs(type.FullName));
                }
                return false;
            }

            //同时运行基类及它本身的所有静态构造函数
            var types = type.GetHierarchy(typeof(VarObject), typeof(object)).ToArray();
            for (int i = types.Length - 1; i >= 0; i--)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(types[i].TypeHandle);
            }

            return true;
        }

        public VarPropertyRepository GetOrCreateVarPropertyRepository(Type ownerType)
        {
            VarPropertyRepository repo = null;
            var ownerTypeName = ownerType.GetQualifiedName();
            if (!_types.TryGetValue(ownerTypeName, out repo))
            {
                lock (syncLock)
                {
                    if (!_types.TryGetValue(ownerTypeName, out repo))
                    {
                        repo = new VarPropertyRepository(ownerType);

                        var baseType = ownerType.BaseType;
                        if (ownerType != typeof(VarObject) && baseType != typeof(VarObject))
                        {
                            if (baseType == typeof(object))
                            {
                                throw new InvalidProgramException(string.Format("属性类型 {0} 必须继承自 VarObject 类。", ownerType));
                            }
                            repo.BaseRepository = GetOrCreateVarPropertyRepository(baseType);
                        }

                        _types.Add(ownerTypeName, repo);
                    }
                }
            }
            return repo;
        }
    }

    public class ComplieEventArgs : EventArgs
    {
        public ComplieEventArgs(VarPropertyRepository repo)
        {
            PropertyRepository = repo;
        }
        public VarPropertyRepository PropertyRepository { get; }
    }
}
