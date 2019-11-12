using System;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Microsoft.CSharp;
using System.Collections;
using System.Linq.Expressions;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;
using System.Linq;
[assembly: CLSCompliant(true)]

namespace ExpressionVisualizer {
    static class ExpressionTreeExtention {
        static string ExtractName(string name)
        {
            int i = name.LastIndexOf("`", StringComparison.Ordinal);
            if (i > 0)
                name = name.Substring(0, i);
            return name;
        }

        static string ExtractGenericArguments(Type[] names)
        {
            StringBuilder builder = new StringBuilder("<");
            foreach (Type genericArgument in names) {
                if (builder.Length != 1) builder.Append(", ");
                builder.Append(ObtainOriginalName(genericArgument));
            }
            builder.Append(">");
            return builder.ToString();
        }

        public static string ObtainOriginalName(this Type type)
        {
            if (!type.IsGenericType) {
                return type.Name;
            }
            else {
                return ExtractName(type.Name) + ExtractGenericArguments(type.GetGenericArguments());
            }
        }

        public static string ObtainOriginalMethodName(this MethodInfo method)
        {
            if (!method.IsGenericMethod) {
                return method.Name;
            }
            else {
                return ExtractName(method.Name) + ExtractGenericArguments(method.GetGenericArguments());
            }
        }
    }

    [Serializable]
    public class ExpressionTreeNode : TreeNode {
        protected ExpressionTreeNode(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected override void Serialize(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext context)
        {
            base.Serialize(si, context);
        }

        public ExpressionTreeNode(Object value)
        {
            Type type = value.GetType();
            Text = type.ObtainOriginalName();

            if (value is Expression) {
                ImageIndex = 2;
                SelectedImageIndex = 2;

                PropertyInfo[] propertyInfos = null;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Expression<>)) {
                    propertyInfos = type.BaseType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                }
                else {
                    propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                }

                foreach (PropertyInfo propertyInfo in propertyInfos) {
                    if ((propertyInfo.Name != "nodeType")) {
                        Nodes.Add(new AttributeNode(value, propertyInfo));
                    }
                }
            }
            else {
                ImageIndex = 4;
                SelectedImageIndex = 4;
                Text = "\"" + value.ToString() + "\"";
            }
        }
    }

    [Serializable]
    public class AttributeNode : TreeNode {
        protected AttributeNode(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public AttributeNode(Object attribute, PropertyInfo propertyInfo)
        {
            Text = propertyInfo.Name + " : " + propertyInfo.PropertyType.ObtainOriginalName();
            ImageIndex = 3;
            SelectedImageIndex = 3;

            Object value = propertyInfo.GetValue(attribute, null);
            if (value != null) {
                if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>)) {
                    if ((int)value.GetType().InvokeMember("get_Count", BindingFlags.InvokeMethod, null, value, null, CultureInfo.InvariantCulture) == 0) {
                        Text += " : Empty";
                    }
                    else {
                        foreach (object tree in (IEnumerable)value) {
                            if (tree is Expression) {
                                Nodes.Add(new ExpressionTreeNode(tree));
                            }
                            else if (tree is MemberAssignment) {
                                Nodes.Add(new ExpressionTreeNode(((MemberAssignment)tree).Expression));
                            }
                        }
                    }
                }
                else if (value is Expression) {
                    Text += ((Expression)value).NodeType;
                    Nodes.Add(new ExpressionTreeNode(value));
                }
                else if (value is MethodInfo) {
                    MethodInfo minfo = value as MethodInfo;
                    Text += " : \"" + minfo.ObtainOriginalMethodName() + "\"";
                }
                else if (value is Type) {
                    Type minfo = value as Type;
                    Text += " : \"" + minfo.ObtainOriginalName() + "\"";
                }
                else {
                    Text += " : \"" + value.ToString() + "\"";
                }
            }
            else {
                Text += " : null";
            }
        }
        protected override void Serialize(System.Runtime.Serialization.SerializationInfo si, System.Runtime.Serialization.StreamingContext context)
        {
            base.Serialize(si, context);
        }

    }
}