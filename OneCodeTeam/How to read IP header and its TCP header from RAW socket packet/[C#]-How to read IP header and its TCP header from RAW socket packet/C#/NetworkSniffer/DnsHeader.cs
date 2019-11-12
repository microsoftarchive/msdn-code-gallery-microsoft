using System.Net;
using System;
using System.IO;

namespace NetworkSniffer
{
    /// <summary>
    /// This class encapsulates all the DNS header fields and provides a mechanism
    /// to set and get the details of them through a parameterized contructor
    /// and public properties respectively.
    /// </summary>
    public class DnsHeader
    {
        // Sixteen bits for identification.
        private ushort UsIdentification;
        // Sixteen bits for DNS flags.
        private ushort UsFlags;
        // Sixteen bits indicating the number of entries in the questions list.
        private ushort UsTotalQuestions;
        // Sixteen bits indicating the number of entries in the answer 
        // resource record list.
        private ushort UsTotalAnswerRRs;
        // Sixteen bits indicating the number of entries in the authority
        // resource record list.
        private ushort UsTotalAuthorityRRs;
        // Sixteen bits indicating the number of entries in the additional
        // resource record list.
        private ushort UsTotalAdditionalRRs;        

        public DnsHeader(byte []byBuffer, int nReceived)
        {
            // Create MemoryStream out of the received bytes.
            MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
            // Next we create a BinaryReader out of the MemoryStream.
            BinaryReader binaryReader = new BinaryReader(memoryStream);   
            // First sixteen bits are for identification.
            UsIdentification = (ushort)IPAddress.NetworkToHostOrder(
                                binaryReader.ReadInt16());
            // Next sixteen contain the flags.
            UsFlags = (ushort)IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            // Read the total numbers of questions in the quesion list.
            UsTotalQuestions = (ushort)IPAddress.NetworkToHostOrder(
                                binaryReader.ReadInt16());
            // Read the total number of answers in the answer list.
            UsTotalAnswerRRs = (ushort)IPAddress.NetworkToHostOrder(
                                binaryReader.ReadInt16());
            // Read the total number of entries in the authority list.
            UsTotalAuthorityRRs = (ushort)IPAddress.NetworkToHostOrder(
                                binaryReader.ReadInt16());
            // Total number of entries in the additional resource record list.
            UsTotalAdditionalRRs = (ushort)IPAddress.NetworkToHostOrder(
                                binaryReader.ReadInt16());
        }

        public string Identification
        {
            get
            {
                return string.Format("0x{0:x2}", UsIdentification);
            }
        }

        public string Flags
        {
            get
            {
                return string.Format("0x{0:x2}", UsFlags);
            }
        }

        public string TotalQuestions
        {
            get
            {
                return UsTotalQuestions.ToString();
            }
        }

        public string TotalAnswerRRs
        {
            get
            {
                return UsTotalAnswerRRs.ToString();
            }
        }

        public string TotalAuthorityRRs
        {
            get
            {
                return UsTotalAuthorityRRs.ToString();
            }
        }

        public string TotalAdditionalRRs
        {
            get
            {
                return UsTotalAdditionalRRs.ToString();
            }
        }
	}
}
