using Css.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Css.Reflection
{
    public static class VarTypeEmit
    {
        static MethodInfo _getValueMethod;
        static MethodInfo _setValueMethod;
        static Dictionary<string, ModuleBuilder> _moduleBilders = new Dictionary<string, ModuleBuilder>();

        static VarTypeEmit()
        {
            var o = new VarObject();
            Func<string, string> getValue = o.Get<string>;
            _getValueMethod = getValue.Method.GetGenericMethodDefinition();
            Action<string, string> setValue = o.Set<string>;
            _setValueMethod = setValue.Method.GetGenericMethodDefinition();
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            ModuleBuilder result;
            _moduleBilders.TryGetValue(args.Name, out result);
            if (result != null)
                return result.Assembly;
            return null;
        }

        static ModuleBuilder FindOrCreateBuilder(string assemblyName)
        {
            ModuleBuilder result;
            if (!_moduleBilders.TryGetValue(assemblyName, out result))
            {
                var assembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.Run);
                result = assembly.DefineDynamicModule("DynamicModule");
                _moduleBilders.Add(assemblyName, result);
            }
            return result;
        }

        public static Type CreateType(VarType type)
        {
            var module = FindOrCreateBuilder(type.AssemblyName);
            TypeBuilder typeBuilder = module.DefineType(type.TypeName, TypeAttributes.Public | TypeAttributes.Class, type.BaseType);
            var repo = VarTypeRepository.Instance.GetOrCreateVarPropertyRepository(type);
            foreach (var p in repo.Container.Properties)
            {
                if (p.PropertyType is VarType)
                    continue;
                var typeCode = Type.GetTypeCode(typeof(Nullable<DateTime>).IgnoreNullable());
                if (typeCode == TypeCode.Empty || typeCode == TypeCode.DBNull || typeCode == TypeCode.Object)
                    continue;
                var propBldr = typeBuilder.DefineProperty(p.Name, PropertyAttributes.HasDefault, p.PropertyType, null);

                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + p.Name, getSetAttr, p.PropertyType, Type.EmptyTypes);

                ILGenerator getIL = getPropMthdBldr.GetILGenerator();
                Label il_000F = getIL.DefineLabel();
                LocalBuilder loc = getIL.DeclareLocal(p.PropertyType);
                getIL.Emit(OpCodes.Nop);
                getIL.Emit(OpCodes.Ldarg_0);
                getIL.Emit(OpCodes.Ldstr, p.Name);
                getIL.Emit(OpCodes.Call, _getValueMethod.MakeGenericMethod(p.PropertyType));
                getIL.Emit(OpCodes.Stloc_0);
                getIL.Emit(OpCodes.Br_S, il_000F);
                getIL.MarkLabel(il_000F);
                getIL.Emit(OpCodes.Ldloc_0);
                getIL.Emit(OpCodes.Ret);

                MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + p.Name, getSetAttr, null, new Type[] { p.PropertyType });

                ILGenerator setIL = setPropMthdBldr.GetILGenerator();
                setIL.Emit(OpCodes.Nop);
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Ldstr, p.Name);
                setIL.Emit(OpCodes.Call, _setValueMethod.MakeGenericMethod(p.PropertyType));
                setIL.Emit(OpCodes.Nop);
                setIL.Emit(OpCodes.Ret);

                propBldr.SetGetMethod(getPropMthdBldr);
                propBldr.SetSetMethod(setPropMthdBldr);
            }
            var result = typeBuilder.CreateTypeInfo();
            return result;
        }
    }
}

