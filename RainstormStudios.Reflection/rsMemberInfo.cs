using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace RainstormStudios.Reflection
{
    [Author("Unfried, Michael")]
    public sealed class rsMemberInfo
    {
        #region Declarations
        //***************************************************************************
        // Private Fields
        // 
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        private rsMemberInfo()
        {
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        //***************************************************************************
        // Static Methods
        // 
        public static string GetScope(MemberInfo mi)
        {
            string scope = string.Empty;
            switch (mi.MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo miFld = (FieldInfo)mi;
                    scope = ((miFld.IsPublic) ? "public " : (miFld.IsFamily) ? "protected " : "private ");
                    if (miFld.IsAssembly) scope += "internal ";
                    if (miFld.IsStatic) scope += "static ";
                    if (miFld.IsLiteral) scope += "const ";
                    break;
                case MemberTypes.Constructor:
                    ConstructorInfo miCtor = (ConstructorInfo)mi;
                    scope = ((miCtor.IsPublic) ? "public " : (miCtor.IsFamily) ? "protected " : "private ");
                    if (miCtor.IsVirtual) scope += "virtual ";
                    if (miCtor.IsAbstract) scope += "abstract ";
                    if (miCtor.IsStatic) scope += "static ";
                    if (miCtor.IsFinal) scope += "final ";
                    break;
                case MemberTypes.Method:
                    MethodInfo miMeth = (MethodInfo)mi;
                    scope = ((miMeth.IsPublic) ? "public " : (miMeth.IsFamily) ? "protected " : "private ");
                    if (miMeth.IsVirtual) scope += "virtual ";
                    if (miMeth.IsAbstract) scope += "abstract ";
                    if (miMeth.IsStatic) scope += "static ";
                    if (miMeth.IsFinal) scope += "final ";
                    break;
                case MemberTypes.Property:
                    scope = "public ";
                    break;
                case MemberTypes.Event:
                    scope = "public event ";
                    break;
                case MemberTypes.TypeInfo:
                    Type miType = (Type)mi;
                    scope = ((miType.IsPublic) ? "public " : "private ");
                    if (miType.IsAbstract)
                        scope += "abstract ";
                    if (miType.IsEnum)
                        scope += "enum ";
                    else if (miType.BaseType != null && miType.BaseType.FullName.ToLower().EndsWith("delegate"))
                        scope += "delegate ";
                    else if (miType.IsClass)
                        scope += "class ";
                    break;
                case MemberTypes.NestedType:
                    Type miNestType = (Type)mi;
                    scope = ((miNestType.IsPublic) ? "public " : "private ");
                    if (miNestType.IsAbstract)
                        scope += "abstract ";
                    if (miNestType.IsEnum)
                        scope += "enum ";
                    else if (miNestType.BaseType != null && miNestType.BaseType.FullName.ToLower().EndsWith("delegate"))
                        scope += "delegate ";
                    else if (miNestType.IsClass)
                        scope += "class ";
                    break;
            }
            return scope;
        }
        public static string GetSignature(MemberInfo mi)
        {
            string sig = "";
            switch (mi.MemberType)
            {
                case MemberTypes.Constructor:
                    ConstructorInfo miCtor = (ConstructorInfo)mi;
                    foreach (ParameterInfo pi in miCtor.GetParameters())
                        sig += rsMemberInfo.GetParameterScope(pi) + ", ";
                    sig = "(" + ((sig.Length > 0) ? sig.Substring(0, sig.Length - 2) : " ") + ")";
                    break;
                case MemberTypes.Method:
                    MethodInfo miMeth = (MethodInfo)mi;
                    foreach (ParameterInfo pi in miMeth.GetParameters())
                        sig += rsMemberInfo.GetParameterScope(pi) + ", ";
                    sig = "(" + ((sig.Length > 0) ? sig.Substring(0, sig.Length - 2) : " ") + ")";
                    break;
                case MemberTypes.Property:
                    PropertyInfo miProp = (PropertyInfo)mi;
                    foreach (ParameterInfo pi in miProp.GetIndexParameters())
                        sig += "," + rsMemberInfo.GetParameterScope(pi);
                    sig = miProp.Name + ((sig.Length > 0) ? " [ " + sig.Substring(1) + " ]" : "");
                    break;
            }
            return sig;
        }
        public static string GetUniqueName(MemberInfo mi)
        {
            return mi.DeclaringType.FullName + "." + mi.Name + GetSignature(mi);
        }
        public static string GetParameterScope(ParameterInfo pi)
        {
            string pScope = "";
            if (pi.IsIn)
                pScope += "in ";
            if (pi.IsOut)
                pScope += "out ";
            if (pi.ParameterType.IsByRef)
                pScope += "ref ";

            // Parameters which are 'ByRef' end up with an ampersand (&) at
            //   the end of their type names, so we have to trim them.
            return pScope + pi.ParameterType.Name.Trim('&');
        }
        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        } 
        #endregion
    }
}
