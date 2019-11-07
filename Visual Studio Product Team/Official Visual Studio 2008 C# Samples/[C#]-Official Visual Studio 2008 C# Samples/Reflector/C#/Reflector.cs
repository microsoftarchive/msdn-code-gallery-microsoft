//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;

// The Reflector component generates a HTML document outlining the public API
// for a given assembly. The task is achieved in two phases. First, a XML 
// document is emitted in memory (see Emit*()). The document captures the 
// relevant aspects of the assembly's metadata. In the second phase, a HTML
// document is extracted from the XML document (see Extract*()). Note that the 
// two phases handle similar information with different representations using 
// the same query constructs.

namespace Samples
{   
    public class Reflector
    {
        public XDocument document;
        
        public Reflector() 
        {
        }
        
        public void Reflect(string assemblyFile)
        {
            Assembly assembly = Assembly.LoadFrom(assemblyFile);
            document = new XDocument(EmitAssembly(assembly));
        }

        public void Transform(XmlWriter writer) 
        {
            if (document == null) return;
            XElement assembly = document.Element("assembly");
            XDocument transform = new XDocument(ExtractAssembly(assembly));
            transform.Save(writer);
        }

        XElement EmitAssembly(Assembly assembly) 
        {
            return new XElement("assembly",
                        new XAttribute("name", assembly.ManifestModule.Name),
                        from type in assembly.GetTypes()
                        where GetVisible(type)
                        group type by GetNamespace(type) into g
                        orderby g.Key
                        select EmitNamespace(g.Key, g));
        }
        
        XElement EmitNamespace(string ns, IEnumerable<Type> types)
        {
            return new XElement("namespace",
                        new XAttribute("name", ns),
                        from type in types
                        orderby type.Name
                        select EmitType(type));
        }
        
        XElement EmitType(Type type)
        {
            return new XElement(type.IsEnum ? "enum" : 
                                type.IsValueType ? "struct" :
                                type.IsInterface ? "interface" :
                                "class",
                        new XAttribute("name", type.Name),
                        !type.IsGenericTypeDefinition ? null : 
                        EmitGenericArguments(type.GetGenericArguments()),
                        EmitModifiers(type),
                        EmitExtends(type.BaseType),
                        EmitImplements(type.GetInterfaces()),
                        EmitDeclaringType(type.DeclaringType),
                        EmitNestedTypes(type.GetNestedTypes()),
                        EmitMethods(type.GetConstructors()),
                        EmitProperties(type.GetProperties()),
                        EmitMethods(type.GetMethods()));
        }

        IEnumerable<XElement> EmitGenericArguments(IEnumerable<Type> args)
        {
            return from arg in args
                   select new XElement("genericArgument", EmitReference(arg));
        }
        
        static XElement EmitModifiers(Type type)
        {
            StringBuilder builder = new StringBuilder();
            if (type.IsPublic) builder.Append("public");
            else if (type.IsNestedPublic) builder.Append("public");
            else if (type.IsNestedFamily) builder.Append("protected");
            else if (type.IsNestedFamANDAssem) builder.Append("protected internal");
            if (type.IsSealed) builder.Append(" sealed");
            if (type.IsAbstract) builder.Append(" abstract");
            return new XElement("modifiers", builder.ToString());
        }
        
        XElement EmitExtends(Type baseType) 
        {
            if (baseType == null || baseType == typeof(Object) || baseType == typeof(ValueType) || baseType == typeof(Enum)) return null;
            return new XElement("extends", EmitReference(baseType));
        }
        
        IEnumerable<XElement> EmitImplements(IEnumerable<Type> ifaces)
        {
            return from iface in ifaces
                   select new XElement("implements", EmitReference(iface));
        }
        
        XElement EmitDeclaringType(Type declaringType)
        {
            if (declaringType == null) return null;
            return new XElement("declaringType", EmitReference(declaringType));
        }
        
        IEnumerable<XElement> EmitNestedTypes(IEnumerable<Type> ntypes)
        {
            return from ntype in ntypes
                   where GetVisible(ntype)
                   select EmitType(ntype);
        }
        
        IEnumerable<XElement> EmitMethods(IEnumerable<MethodBase> metds)
        {
            return from metd in metds
                   where GetVisible(metd)
                   select new XElement("method",                                
                                new XAttribute("name", metd.Name),
                                !metd.IsGenericMethodDefinition ? null :
                                EmitGenericArguments(metd.GetGenericArguments()),
                                EmitModifiers(metd),
                                EmitReturnType(metd),
                                !metd.IsDefined(typeof(ExtensionAttribute), true) ? null :
                                EmitExtension(metd),
                                EmitParameters(metd.GetParameters()));                                
        }
        
        IEnumerable<XElement> EmitProperties(IEnumerable<PropertyInfo> props)
        {
            return from prop in props
                   where GetVisible(prop.GetGetMethod()) ||
                         GetVisible(prop.GetSetMethod())
                   select new XElement("property",                            
                                new XAttribute("name", prop.Name),
                                new XElement("propertyType", EmitReference(prop.PropertyType)));
        }

        IEnumerable<object> EmitReference(Type type)
        {
            if (!type.IsGenericType) 
            {
                return new object[] { new XAttribute("name", type.Name),
                                      new XAttribute("namespace", GetNamespace(type)) };            
            }
            else
            {
                return new object[] { new XAttribute("name", type.Name),
                                      new XAttribute("namespace", GetNamespace(type)),
                                      EmitGenericArguments(type.GetGenericArguments()) };
            }
        }

        static XElement EmitModifiers(MethodBase metd)
        {
            StringBuilder builder = new StringBuilder();            
            if (metd.IsPublic) builder.Append("public");
            else if (metd.IsFamily) builder.Append("protected");
            else if (metd.IsFamilyAndAssembly) builder.Append("protected internal");
            if (metd.IsAbstract) builder.Append(" abstract");
            if (metd.IsStatic) builder.Append(" static");
            if (metd.IsVirtual) builder.Append(" virtual");
            return new XElement("modifiers", builder.ToString());
        }

        XElement EmitReturnType(MethodBase metd)
        {  
            MethodInfo metdInfo = metd as MethodInfo;
            if (metdInfo == null) return null;
            return new XElement("returnType", EmitReference(metdInfo.ReturnType));
        }

        static XElement EmitExtension(MethodBase metd)
        {
            return new XElement("extension");
        }
        
        IEnumerable<XElement> EmitParameters(IEnumerable<ParameterInfo> parms)
        {
            return from parm in parms
                   select new XElement("parameter",
                                new XAttribute("name", parm.Name),
                                new XElement("parameterType", EmitReference(parm.ParameterType)));
        }                                                
        
        static string GetNamespace(Type type)
        {
            string ns = type.Namespace;
            return ns != null ? ns : string.Empty;
        }       

        static bool GetVisible(Type type)
        {
            return type.IsPublic || type.IsNestedPublic || type.IsNestedFamily || type.IsNestedFamANDAssem;
        }
       
        static bool GetVisible(MethodBase metd)
        {
            return metd != null && (metd.IsPublic || metd.IsFamily || metd.IsFamilyAndAssembly);
        }
        
        XElement ExtractAssembly(XElement assembly)
        {
            return new XElement("html",
                        new XElement("head",
                            new XElement("title", ExtractName(assembly))),
                        new XElement("body",
                            new XElement("div", 
                                new XElement("h1", "Assembly: ",ExtractName(assembly)),
                                from ns in assembly.Elements("namespace") 
                                select ExtractNamespace(ns))));                                    
        }

        XElement ExtractNamespace(XElement ns)
        {
            return new XElement("div",
                        new XElement("h2", "Namespace: ", ExtractName(ns)),
                        from name in new string[] { "class", "interface", "struct", "enum" }
                        where ns.Elements(name).Any()
                        select from type in ns.Elements(name)
                               where !type.Elements("declaringType").Any()
                               select ExtractType(type));
        }

        XElement ExtractType(XElement type) 
        {
            return new XElement("div",
                        new XElement("h3",
                            ExtractModifiers(type) + " ",
                            type.Name + " ",
                            ExtractReference(type),                   
                            ExtractInherits(type)),
                        ExtractConstructors(type),
                        ExtractProperties(type),
                        ExtractOperators(type),
                        ExtractMethods(type));
        }

        static string ExtractModifiers(XElement element)
        {
            return element.Element("modifiers").Value;
        }

        static string ExtractName(XElement element)
        {
            string name = element.Attribute("name").Value;
            int i = name.LastIndexOf("`", StringComparison.Ordinal);
            if (i > 0) name = name.Substring(0, i); // fix generic name
            return name;
        }                        

        string ExtractGenericArguments(XElement element)
        {
            if (!element.Elements("genericArgument").Any()) return string.Empty;
            StringBuilder builder = new StringBuilder("<");
            foreach (XElement genericArgument in element.Elements("genericArgument"))
            {
                if (builder.Length != 1) builder.Append(", ");
                builder.Append(ExtractReference(genericArgument));
            }
            builder.Append(">");
            return builder.ToString();
        }

        string ExtractReference(XElement element)
        {
            return ExtractName(element) + ExtractGenericArguments(element);
        }
                
        string ExtractInherits(XElement type)
        {
            if (!type.Elements("extends").Concat(type.Elements("implements")).Any()) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (XElement inherits in type.Elements("extends").Concat(type.Elements("implements")))
            {
                if (builder.Length == 0) builder.Append(" : ");
                else builder.Append(", ");
                builder.Append(ExtractReference(inherits));
            }
            return builder.ToString();
        }

        XElement ExtractConstructors(XElement type)
        {
            var ctors = from ctor in type.Elements("method")
                        where ExtractName(ctor) == ".ctor"
                        select new XElement("li",
                                    ExtractModifiers(ctor) + " ",
                                    ExtractName(type),
                                    ExtractParameters(ctor));
            if (!ctors.Any()) return null;                                    
            return new XElement("div", 
                        new XElement("h4", "Constructors: "),
                        new XElement("ul", ctors));
        }

        XElement ExtractProperties(XElement type)
        {
            var props = from prop in type.Elements("property")
                        let propName = ExtractName(prop)
                        let getter = "get_" + propName
                        let setter = "set_" + propName
                        select new XElement("ul",
                                    propName,
                                    from metd in type.Elements("method")
                                    let metdName = ExtractName(metd)
                                    where metdName == getter ||
                                          metdName == setter
                                    select ExtractMethod(metd));
            if (!props.Any()) return null;                                    
            return new XElement("div", 
                        new XElement("h4", "Properties: "), 
                        props);
        }

        XElement ExtractOperators(XElement type)
        {
            var ops = from op in type.Elements("method")
                      let name = ExtractName(op)
                      where name.StartsWith("op_", StringComparison.Ordinal)
                      select new XElement("ul",
                                    name.Substring("op_".Length),
                                    ExtractMethod(op));
            if (!ops.Any()) return null;                                    
            return new XElement("div", 
                        new XElement("h4", "Operators: "),
                        ops);                      
        }
                
        XElement ExtractMethods(XElement type)
        {
            var metds = from metd in type.Elements("method")
                        let name = ExtractName(metd)
                        where name != ".ctor" &&
                              !type.Elements("property").
                               Where(prop => name == "get_" + ExtractName(prop) ||
                                             name == "set_" + ExtractName(prop)).
                               Any() &&
                              !name.StartsWith("op_") 
                        select ExtractMethod(metd);
            if (!metds.Any()) return null;
            return new XElement("div", 
                        new XElement("h4", "Methods: "), 
                        new XElement("ul", metds));
        }

        XElement ExtractMethod(XElement metd)
        {
            return new XElement("li",
                        ExtractModifiers(metd) + " ",
                        ExtractReference(metd.Element("returnType")) + " ",
                        ExtractReference(metd),
                        ExtractParameters(metd));                         
        }
        
        string ExtractParameters(XElement metd)
        {
            StringBuilder builder = new StringBuilder("(");
            foreach (XElement parm in metd.Elements("parameter"))
            {
                if (builder.Length == 1)
                {
                    if (metd.Element("extension") != null) builder.Append("this ");
                }
                else
                {
                    builder.Append(", ");
                }
                builder.Append(ExtractReference(parm.Element("parameterType")));
                builder.Append(" ");
                builder.Append(ExtractName(parm));
            }
            builder.Append(")");
            return builder.ToString();
        }
    }
}
