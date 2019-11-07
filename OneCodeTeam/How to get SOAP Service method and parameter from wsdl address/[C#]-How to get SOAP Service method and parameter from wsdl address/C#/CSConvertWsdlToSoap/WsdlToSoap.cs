using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CSConvertWsdlToSoap
{
    /// <summary>
    /// This class can convert WSDL format xml to SOAP format
    /// </summary>
    public class WsdlToSoap
    {
        private XmlDocument _wsdlXDoc = new XmlDocument();
        private Dictionary<string, string> _soapMethodDic = new Dictionary<string, string>();
        private XmlNamespaceManager _wsdlNSM = null;

        public WsdlToSoap(string wsdlContent)
        {
            this._wsdlXDoc.LoadXml(wsdlContent);

            // set name space
            this._wsdlNSM = new XmlNamespaceManager(this._wsdlXDoc.NameTable);
            this._wsdlNSM.AddNamespace("wsdl", "http://schemas.xmlsoap.org/wsdl/");
            this._wsdlNSM.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
        }
                public Dictionary<string, string> SoapMethodDictionary
        {
            get
            {
                this.AllMethods.ForEach((methodName) => {
                    var soapContent = this.BuildSkeleton();
                    soapContent = this.BuildSoapMethodElements(soapContent, methodName);
                    this._soapMethodDic.Add(methodName, soapContent);
                });
                return this._soapMethodDic;
            }
        }

        #region common const
        private const string NSPrefix = "ns1";
        private const string NSPlaceHolder = "#ns";
        #endregion common const

        private static string _soapSkeleton
        {
            get
            {
                return $@"<?xml version='1.0' encoding='UTF-8'?>
                            <SOAP-ENV:Envelope xmlns:SOAP-ENV='http://schemas.xmlsoap.org/soap/envelope/' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:{WsdlToSoap.NSPrefix}='{WsdlToSoap.NSPlaceHolder}'>
                                <SOAP-ENV:Body>
                            	</SOAP-ENV:Body>
                            </SOAP-ENV:Envelope>";
            }
        }
                
        /// <summary>
        /// return one of "0 true Array ?"
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        private string _defaultValue(string typeName)
        {
            if (!typeName.Contains(":")) {
                throw new Exception($"Invalid typeName: {typeName}");
            }

            if (!typeName.StartsWith("xs:")) {
                if (this._isArray(typeName)) {
                    return "Array";
                } else {
                    return string.Empty;
                }
            }

            typeName = typeName.Remove(0, 3); // remove prefix xs:

            switch (typeName) {
                case "boolean":
                    return "true";
                case "string":
                    return "?";
                case "decimal":
                case "double":
                    return "0.0";
                default:
                    return "0";
            }
        }

        private bool _isArray(string typeName)
        {
            if (typeName.Contains(":")) {
                typeName = typeName.Remove(0, typeName.IndexOf(':') + 1);
            } else {
                throw new Exception($"Invalid typeName: {typeName}");
            }

            var simpleTypeNode = this._wsdlXDoc.SelectSingleNode($"./wsdl:definitions/wsdl:types//xs:simpleType[@name='{typeName}']", _wsdlNSM);
            if (simpleTypeNode != null) {
                if (simpleTypeNode.SelectSingleNode(".//xs:enumeration", _wsdlNSM) != null) {
                    return true;
                }
            }
            return false;
        }

        private string _nameSpace
        {
            get
            {
                return this._wsdlXDoc.LastChild.Attributes["targetNamespace"].Value;
            }
        }

        private XmlNode _getSoapTypeNodeFromWsdlXml(XmlDocument soapXmlDoc, XmlNode wsdlTypeNode, string typeName, string paramName)
        {
            if (!typeName.Contains(":")) {
                throw new Exception($"Invalid typeName: {typeName}");
            }

            var typeNameWithoutPrefix = typeName.Remove(0, typeName.IndexOf(':') + 1);
            var typeNode = soapXmlDoc.CreateElement(WsdlToSoap.NSPrefix, paramName, this._nameSpace);

            // primitive type and simple type
            var defaultValue = this._defaultValue(typeName);
            if (!string.IsNullOrWhiteSpace(defaultValue)) {
                typeNode.AppendChild(soapXmlDoc.CreateTextNode(defaultValue));
                return typeNode;
            }

            // if has base type, get it
            var baseTypeNodes = this._getBaseTypeFromWsdlXml(soapXmlDoc, wsdlTypeNode, typeName, paramName);
            baseTypeNodes.ForEach((subTypeNode) => {
                typeNode.AppendChild(subTypeNode);
            });

            // complex type, execute recursive
            var compositeTypeNodes = wsdlTypeNode.SelectNodes($".//xs:complexType[@name='{typeNameWithoutPrefix}']//xs:sequence/xs:element[@name][@type]", _wsdlNSM);
            if (0 == compositeTypeNodes.Count) {
                return null;
            }

            for (var i = 0; i < compositeTypeNodes.Count; ++i) {
                var compositeParamName = compositeTypeNodes.Item(i).Attributes["name"].Value;
                var compositeTypeName = compositeTypeNodes.Item(i).Attributes["type"].Value;
                var compsiteTypeNode = this._getSoapTypeNodeFromWsdlXml(soapXmlDoc, wsdlTypeNode, compositeTypeName, compositeParamName);
                if (null != compsiteTypeNode) {
                    typeNode.AppendChild(compsiteTypeNode);
                }
            }

            return typeNode;
        }

        private List<XmlNode> _getBaseTypeFromWsdlXml(XmlDocument soapXmlDoc, XmlNode wsdlTypeNode, string typeName, string paramName)
        {
            List<XmlNode> baseNodes = new List<XmlNode>();

            if (!typeName.Contains(":")) {
                throw new Exception($"Invalid typeName: {typeName}");
            }

            var typeNameWithoutPrefix = typeName.Remove(0, typeName.IndexOf(':') + 1);
            var typeNodeDef = wsdlTypeNode.SelectSingleNode($".//xs:complexType[@name='{typeNameWithoutPrefix}']", _wsdlNSM);
            if (null == typeNodeDef) {
                return baseNodes;
            }

            var baseNode = typeNodeDef.SelectSingleNode(".//xs:extension[@base]", _wsdlNSM);
            if (null == baseNode) {
                return baseNodes;
            }

            var baseTypeName = baseNode.Attributes["base"].Value;
            var baseTypeNameWithoutPrefix = baseTypeName.Contains(":") ? baseTypeName.Remove(0, baseTypeName.IndexOf(':') + 1) : baseTypeName;

            var baseTypeCompositeTypes = wsdlTypeNode.SelectNodes($".//xs:complexType[@name='{baseTypeNameWithoutPrefix}']//xs:sequence/xs:element[@name][@type]", _wsdlNSM);
            for (var i = 0; i < baseTypeCompositeTypes.Count; ++i) {
                var baseTypeCompositeTypeNode = baseTypeCompositeTypes.Item(i);
                var baseTypeCompositeTypeName = baseTypeCompositeTypeNode.Attributes["type"].Value;
                var baseTypeCompositeParamName = baseTypeCompositeTypeNode.Attributes["name"].Value;

                var baseTypeCompositeTypeSoapNode = this._getSoapTypeNodeFromWsdlXml(soapXmlDoc, wsdlTypeNode, baseTypeCompositeTypeName, baseTypeCompositeParamName);
                if (null != baseTypeCompositeTypeSoapNode) {
                    baseNodes.Add(baseTypeCompositeTypeSoapNode);
                }
            }

            return baseNodes;
        }

        #region operation
        private List<string> AllMethods
        {
            get
            {
                var methodList = new List<string>();
                var nodeList = this._wsdlXDoc.SelectNodes("./wsdl:definitions/wsdl:portType/wsdl:operation[@name]", this._wsdlNSM);
                for (int i = 0; i < nodeList.Count; ++i) {
                    methodList.Add(nodeList.Item(i).Attributes["name"].Value);
                }
                return methodList;
            }
        }

        private string BuildSkeleton()
        {
            return WsdlToSoap._soapSkeleton.Replace(WsdlToSoap.NSPlaceHolder, this._nameSpace);
        }

        private string BuildSoapMethodElements(string soapContent, string methodName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(soapContent);
            var targetNode = doc.LastChild.FirstChild; //"SOAP-ENV:Body" node
            var sourceNode = this._wsdlXDoc.SelectSingleNode($"./wsdl:definitions/wsdl:types//xs:element[@name='{methodName}']", _wsdlNSM);
            if (null == sourceNode) {
                return doc.OuterXml;
            }

            var wsdlTypeNode = this._wsdlXDoc.SelectSingleNode("./wsdl:definitions/wsdl:types", _wsdlNSM);

            // insert method name
            var methodNode = doc.CreateElement(WsdlToSoap.NSPrefix, methodName, this._nameSpace);
            targetNode = targetNode.AppendChild(methodNode);

            // insert parameter
            var paramNodes = sourceNode.SelectNodes(".//xs:sequence/xs:element[@name][@type]", _wsdlNSM);
            for (var i = 0; i < paramNodes.Count; ++i) {
                var paramNode = paramNodes.Item(i);
                var paramType = paramNode.Attributes["type"].Value;
                var paramName = paramNode.Attributes["name"].Value;
                var paramSoapNode = this._getSoapTypeNodeFromWsdlXml(doc, wsdlTypeNode, paramType, paramName);
                if (paramSoapNode != null) {
                    targetNode.AppendChild(paramSoapNode);
                }
            }

            return doc.OuterXml;
        }
        #endregion operation
    }
}
