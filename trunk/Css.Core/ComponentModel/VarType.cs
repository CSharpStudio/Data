using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Css.ComponentModel
{
    /// <summary>
    /// The type of a var object.
    /// </summary>
    public class VarType : Type
    {
        Type _type;
        string _assemblyName;
        string _typeName;
        Type _baseType;

        public VarType(string assemblyName, string typeName, Type baseType = null)
        {
            _assemblyName = assemblyName;
            _typeName = typeName;
            _baseType = baseType;
        }

        public Type Type
        {
            get
            {
                if (_type == null)
                {
                    Assembly a = Assembly.Load(_assemblyName);
                    _type = a.GetType(_typeName);
                }

                return _type;
            }
        }

        public override IEnumerable<CustomAttributeData> CustomAttributes
        {
            get { return Type.CustomAttributes; }
        }

        public override GenericParameterAttributes GenericParameterAttributes
        {
            get { return Type.GenericParameterAttributes; }
        }

        public override Type[] GenericTypeArguments
        {
            get { return Type.GenericTypeArguments; }
        }

        public override bool ContainsGenericParameters
        {
            get { return Type.ContainsGenericParameters; }
        }

        public override MethodBase DeclaringMethod
        {
            get { return Type.DeclaringMethod; }
        }

        public override Type DeclaringType
        {
            get { return Type.DeclaringType; }
        }

        public override int GenericParameterPosition
        {
            get { return Type.GenericParameterPosition; }
        }

        public override bool IsGenericParameter
        {
            get { return Type.IsGenericParameter; }
        }

        public override bool IsConstructedGenericType
        {
            get { return Type.IsConstructedGenericType; }
        }

        public override bool IsEnum
        {
            get { return Type.IsEnum; }
        }

        public override bool IsSecurityCritical
        {
            get { return Type.IsSecurityCritical; }
        }

        public override bool IsGenericType
        {
            get { return Type.IsGenericType; }
        }

        public override bool IsGenericTypeDefinition
        {
            get { return Type.IsGenericTypeDefinition; }
        }

        public override bool IsSecuritySafeCritical
        {
            get { return Type.IsSecuritySafeCritical; }
        }

        public override bool IsSecurityTransparent
        {
            get { return Type.IsSecurityTransparent; }
        }

        public override bool IsSerializable
        {
            get { return Type.IsSerializable; }
        }

        public override MemberTypes MemberType
        {
            get { return Type.MemberType; }
        }

        public override int MetadataToken
        {
            get { return Type.MetadataToken; }
        }

        public override StructLayoutAttribute StructLayoutAttribute
        {
            get { return Type.StructLayoutAttribute; }
        }

        public override Type ReflectedType
        {
            get { return Type.ReflectedType; }
        }

        public override RuntimeTypeHandle TypeHandle
        {
            get { return Type.TypeHandle; }
        }

        public string AssemblyName { get { return _assemblyName; } }

        public string TypeName { get { return _typeName; } }

        public override Assembly Assembly { get { return Type.Assembly; } }

        public override string AssemblyQualifiedName { get { return Type.AssemblyQualifiedName; } }

        public override Type BaseType
        {
            get
            {
                if (_type == null && _baseType != null)
                    return _baseType;
                return Type.BaseType;
            }
        }

        public override string FullName { get { return Type.FullName; } }

        public override Guid GUID { get { return Type.GUID; } }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return Type.Attributes;
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return Type.GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return Type.GetConstructors(bindingAttr);
        }

        public override Type GetElementType()
        {
            return Type.GetElementType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return Type.GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return Type.GetEvents(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return Type.GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return Type.GetFields(bindingAttr);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return Type.GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return Type.GetInterfaces();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return Type.GetMembers(bindingAttr);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return Type.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return Type.GetMethods(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return Type.GetNestedType(name, bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return Type.GetNestedTypes(bindingAttr);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return Type.GetProperties(bindingAttr);
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return Type.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        protected override bool HasElementTypeImpl()
        {
            return Type.HasElementType;
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters)
        {
            return Type.InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        protected override bool IsArrayImpl()
        {
            return Type.IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return Type.IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return Type.IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return Type.IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return Type.IsPrimitive;
        }

        public override Module Module
        {
            get { return Type.Module; }
        }

        public override string Namespace { get { return Type.Namespace; } }

        public override Type UnderlyingSystemType { get { return Type.UnderlyingSystemType; } }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Type.GetCustomAttributes(attributeType, inherit);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return Type.GetCustomAttributes(inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return Type.IsDefined(attributeType, inherit);
        }

        public override string Name { get { return Type.Name; } }

        public override bool IsAssignableFrom(Type c)
        {
            return Type.IsAssignableFrom(c);
        }

        public override Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
        {
            return Type.FindInterfaces(filter, filterCriteria);
        }

        public override MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
        {
            return Type.FindMembers(memberType, bindingAttr, filter, filterCriteria);
        }

        public override Type MakePointerType()
        {
            return Type.MakePointerType();
        }

        public override Type MakeGenericType(params Type[] typeArguments)
        {
            return Type.MakeGenericType(typeArguments);
        }

        public override Type MakeByRefType()
        {
            return Type.MakeByRefType();
        }

        public override int GetArrayRank()
        {
            return Type.GetArrayRank();
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return Type.GetCustomAttributesData();
        }

        public override MemberInfo[] GetDefaultMembers()
        {
            return Type.GetDefaultMembers();
        }

        public override string GetEnumName(object value)
        {
            return Type.GetEnumName(value);
        }

        public override string[] GetEnumNames()
        {
            return Type.GetEnumNames();
        }

        public override Type GetEnumUnderlyingType()
        {
            return Type.GetEnumUnderlyingType();
        }

        public override Array GetEnumValues()
        {
            return Type.GetEnumValues();
        }

        public override Type[] GetGenericArguments()
        {
            return Type.GetGenericArguments();
        }

        public override EventInfo[] GetEvents()
        {
            return Type.GetEvents();
        }

        public override Type[] GetGenericParameterConstraints()
        {
            return Type.GetGenericParameterConstraints();
        }

        public override Type GetGenericTypeDefinition()
        {
            return Type.GetGenericTypeDefinition();
        }

        public override InterfaceMapping GetInterfaceMap(Type interfaceType)
        {
            return Type.GetInterfaceMap(interfaceType);
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        public override MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
        {
            return Type.GetMember(name, bindingAttr);
        }

        public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
        {
            return Type.GetMember(name, type, bindingAttr);
        }

        public override bool IsEnumDefined(object value)
        {
            return Type.IsEnumDefined(value);
        }

        public override bool IsEquivalentTo(Type other)
        {
            return Type.IsEquivalentTo(other);
        }

        public override bool IsInstanceOfType(object o)
        {
            return Type.IsInstanceOfType(o);
        }

        public override bool IsSubclassOf(Type c)
        {
            return Type.IsSubclassOf(c);
        }

        public override Type MakeArrayType()
        {
            return Type.MakeArrayType();
        }

        public override Type MakeArrayType(int rank)
        {
            return Type.MakeArrayType(rank);
        }

        public override bool Equals(Type o)
        {
            return Type.Equals(o);
        }

        public override bool Equals(object o)
        {
            return Type.Equals(o);
        }
    }
}