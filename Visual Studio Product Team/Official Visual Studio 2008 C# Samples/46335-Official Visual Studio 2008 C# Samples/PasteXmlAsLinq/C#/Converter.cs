//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace PasteXmlAsLinq
{
    class Converter
    {
        XElement root;
        List<XNamespace> namespaces;
        List<string> prefixes;
        StringBuilder sb;
        CodeDomProvider cp;
        int pos;
        int indent;

        public static bool CanConvert(string xml) {
            bool result = false;
            try {
                XElement.Parse(xml);
                result = true;
            }
            catch {
            }
            return result;
        }

        public static string Convert(string xml) {
            return new Converter().GetSourceCode(xml);
        }

        string GetSourceCode(string xml) {
            root = XElement.Parse(xml);
            namespaces = new List<XNamespace>();
            prefixes = new List<string>();
            sb = new StringBuilder();
            cp = new CSharpCodeProvider();
            FindNamespaces();
            WriteXNamespaces();
            WriteXElement();
            return sb.ToString();
        }

        void AddNamespace(XNamespace ns) {
            if (ns != XNamespace.None &&
                ns != XNamespace.Xml &&
                ns != XNamespace.Xmlns &&
                !namespaces.Contains(ns)) {
                namespaces.Add(ns);
                prefixes.Add(null);
            }
        }

        void FindNamespaces() {
            foreach (XElement e in root.DescendantsAndSelf()) {
                AddNamespace(e.Name.Namespace);
                foreach (XAttribute a in e.Attributes()) {
                    AddNamespace(a.Name.Namespace);
                }
            }
            foreach (XAttribute a in root.DescendantsAndSelf().Attributes()) {
                if (a.IsNamespaceDeclaration && a.Name.LocalName != "xmlns") {
                    int i = namespaces.IndexOf(a.Value);
                    if (i >= 0 && prefixes[i] == null) {
                        string s = a.Name.LocalName.Replace('.', '_').Replace('-', '_');
                        if (!cp.IsValidIdentifier(s)) {
                            s = '_' + s;
                            if (!cp.IsValidIdentifier(s)) s = null;
                        }
                        prefixes[i] = s;
                    }
                }
            }
            for (int i = 0; i < prefixes.Count; i++)
                if (prefixes[i] == null) prefixes[i] = "ns";
            for (int i = 0; i < prefixes.Count - 1; i++) {
                string s = prefixes[i];
                for (int j = i + 1; j < prefixes.Count; j++) {
                    if (s == prefixes[j]) {
                        int n = 1;
                        for (int k = i; k < prefixes.Count; k++) {
                            if (s == prefixes[k]) prefixes[k] = s + n++;
                        }
                        break;
                    }
                }
            }
        }

        static bool IsSingleLineText(XElement e) {
            if (e.HasAttributes) return false;
            foreach (XNode node in e.Nodes())
                if (!(node is XText) || ((XText)node).Value.Contains("\n")) return false;
            return true;
        }

        void Write(char ch) {
            if (pos < indent) {
                sb.Append(' ', indent - pos);
                pos = indent;
            }
            sb.Append(ch);
            pos++;
        }

        void Write(string s) {
            if (pos < indent) {
                sb.Append(' ', indent - pos);
                pos = indent;
            }
            sb.Append(s);
            pos += s.Length;
        }

        void WriteName(XName name) {
            if (name.Namespace != XNamespace.None) {
                if (name.Namespace == XNamespace.Xmlns) {
                    if (name.LocalName != "xmlns")
                        Write("XNamespace.Xmlns + ");
                }
                else if (name.Namespace == XNamespace.Xml) {
                    Write("XNamespace.Xml + ");
                }
                else {
                    Write(prefixes[namespaces.IndexOf(name.Namespace)]);
                    Write(" + ");
                }
            }
            WriteStringLiteral(name.LocalName);
        }

        void WriteNewElement(XElement e) {
            Write("new XElement(");
            WriteName(e.Name);
            if (!e.IsEmpty || e.HasAttributes) {
                if (IsSingleLineText(e)) {
                    Write(", ");
                    WriteStringLiteral(e.Value);
                }
                else {
                    indent += 4;
                    foreach (XAttribute a in e.Attributes()) {
                        Write(",");
                        WriteNewLine();
                        Write("new XAttribute(");
                        WriteName(a.Name);
                        Write(", ");
                        int i = a.IsNamespaceDeclaration ? namespaces.IndexOf(a.Value) : -1;
                        if (i >= 0) {
                            Write(prefixes[i]);
                        }
                        else {
                            WriteStringLiteral(a.Value);
                        }
                        Write(")");
                    }
                    foreach (XNode node in e.Nodes()) {
                        Write(",");
                        WriteNewLine();
                        if (node is XText) {
                            WriteStringLiteral(((XText)node).Value);
                        }
                        else if (node is XElement) {
                            WriteNewElement((XElement)node);
                        }
                        else if (node is XComment) {
                            WriteNewObject("XComment", null, ((XComment)node).Value);
                        }
                        else if (node is XProcessingInstruction) {
                            XProcessingInstruction pi = (XProcessingInstruction)node;
                            WriteNewObject("XProcessingInstruction", pi.Target, pi.Data);
                        }
                    }
                    WriteNewLine();
                    indent -= 4;
                }
            }
            Write(")");
        }

        void WriteNewLine() {
            sb.Append("\r\n");
            pos = 0;
        }

        void WriteNewObject(string type, string name, string value) {
            Write("new ");
            Write(type);
            Write("(");
            if (name != null) WriteStringLiteral(name);
            if (!value.Contains("\n")) {
                if (name != null) Write(", ");
                WriteStringLiteral(value);
            }
            else {
                if (name != null) Write(",");
                WriteNewLine();
                indent += 4;
                WriteStringLiteral(value);
                WriteNewLine();
                indent -= 4;
            }
            Write(")");
        }

        void WriteStringLiteral(string s) {
            Write('"');
            for (int i = 0; i < s.Length; i++) {
                char ch = s[i];
                if (ch >= ' ') {
                    if (ch == '\\' || ch == '"') Write('\\');
                    Write(ch);
                }
                else if (ch == '\t') {
                    Write("\\t");
                }
                else if (ch == '\r') {
                    Write("\\r");
                }
                else if (ch == '\n') {
                    Write("\\n");
                    if (s.Length - i > 1) {
                        Write("\" +");
                        WriteNewLine();
                        Write('"');
                    }
                }
            }
            Write('"');
        }

        void WriteXElement() {
            Write("XElement xml = ");
            WriteNewElement(root);
            Write(";");
            WriteNewLine();
        }

        void WriteXNamespaces() {
            for (int i = 0; i < namespaces.Count; i++) {
                Write("XNamespace ");
                Write(prefixes[i]);
                Write(" = ");
                WriteStringLiteral(namespaces[i].NamespaceName);
                Write(";");
                WriteNewLine();
            }
        }
    }
}
