using Css.Modules;
using Css.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Css
{
    public partial class AppRuntime
    {
        static IList<ModuleAssembly> _modules;
        static object _moduleLock = new object();

        /// <summary>
        /// 找到当前程序所有可运行的领域实体插件。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ModuleAssembly> Modules
        {
            get
            {
                if (_modules == null)
                {
                    lock (_moduleLock)
                    {
                        if (_modules == null)
                        {
                            var assemblies = Directory.GetFiles(RT.Environment.DllRootDirectory, "*.dll", SearchOption.AllDirectories)
                                .Select(p => Assembly.LoadFrom(p));
                            _modules = LoadSortedModules(assemblies);
                        }
                    }
                }
                return _modules;
            }
        }

        /// <summary>
        /// 启动所有的 模块插件
        /// </summary>
        internal static void StartupModules()
        {
            foreach (var moduleAssembly in Modules)
            {
                if (moduleAssembly.Module != null) moduleAssembly.Module.Initialize(App);
            }
        }

        static List<ModuleAssembly> LoadSortedModules(IEnumerable<Assembly> assemblies)
        {
            var list = new List<ModuleAssembly>();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var attribute = assembly.GetCustomAttribute<ModuleAttribute>();
                    IModule moduleInstance = null;
                    if (attribute != null)
                    {
                        var t = attribute.ModuleType;
                        if (t != null && typeof(IModule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract && t != typeof(EmptyModule))
                            moduleInstance = Activator.CreateInstance(attribute.ModuleType) as IModule;
                        else
                            moduleInstance = new EmptyModule(assembly);
                        list.Add(new ModuleAssembly(assembly, moduleInstance));
                    }
                }
                catch (ReflectionTypeLoadException exc)
                {
                    string message = assembly.FullName;
                    foreach (var e in exc.LoaderExceptions)
                    {
                        message += e.Message + "\r\n";
                    }
                    throw new SystemException(message, exc);
                }
                catch (Exception exc)
                {
                    throw new SystemException(assembly.FullName, exc);
                }
            }

            //将 list 中集合中的元素，先按照 SetupLevel 排序；
            //然后同一个启动级别中的插件，再按照引用关系来排序。
            var sorted = new List<ModuleAssembly>(list.Count);
            var index = 0;
            var sortedItems = SortByReference(list);
            foreach (var item in sortedItems)
            {
                item.SetupIndex = index++;
                sorted.Add(item);
            }

            return sorted;
        }

        static List<ModuleAssembly> SortByReference(IEnumerable<ModuleAssembly> list)
        {
            //items 表示待处理列表。
            var items = list.ToList();
            var sorted = new List<ModuleAssembly>(items.Count);

            while (items.Count > 0)
            {
                for (int i = 0, c = items.Count; i < c; i++)
                {
                    var item = items[i];
                    bool referencesOther = false;
                    var refItems = item.Assembly.GetReferencedAssemblies();
                    for (int j = 0, c2 = items.Count; j < c2; j++)
                    {
                        if (i != j)
                        {
                            if (refItems.Any(ri => ri.FullName == items[j].Assembly.FullName))
                            {
                                referencesOther = true;
                                break;
                            }
                        }
                    }
                    //没有被任何一个程序集引用，则把这个加入到结果列表中，并从待处理列表中删除。
                    if (!referencesOther)
                    {
                        sorted.Add(item);
                        items.RemoveAt(i);

                        //跳出循环，从新开始。
                        break;
                    }
                }
            }

            return sorted;
        }
    }
}
