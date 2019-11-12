using System.Net;
using System;
using System.IO;

namespace NetworkSniffer
{
    /// <summary>
    /// This class encapsulates all the UDP header fields and provides a mechanism
    /// to set and get the details of them through a parameterized contructor
    /// and public properties respectively.
    /// </summary>
    public class UdpHeader
    {
        // Sixteen bits for the source port number.
        private ushort UsSourcePort;
        // Sixteen bits for the destination port number.
        private ushort UsDestinationPort;
        // Length of the UDP header.
        private ushort UsLength;
        // Sixteen bits for the checksum (checksum can be negative so taken as short).
        private short SChecksum;
        // Data carried by the UDP packet.
        private byte[] ByUDPData = new byte[4096];

        public UdpHeader(byte [] byBuffer, int nReceived)
        {
            // Create MemoryStream out of the received bytes.
            MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
            // Next we create a BinaryReader out of the MemoryStream.
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            // The first sixteen bits contain the source port.
            UsSourcePort = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
            // The next sixteen bits contain the destination port.
            UsDestinationPort = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
            // The next sixteen bits contain the length of the UDP packet.
            UsLength = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
            // The next sixteen bits contain the checksum.
            SChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
            // Copy the data carried by the UDP packet into the data buffer.
            Array.Copy(byBuffer, 8, ByUDPData, 0, nReceived - 8);
        }

        public string SourcePort
        {
            get
            {
                return UsSourcePort.ToString();
            }
        }

        public string DestinationPort
        {
            get
            {
                return UsDestinationPort.ToString();
            }
        }

        public string Length
        {
            get
            {
                return UsLength.ToString ();
            }
        }

        public string Checksum
        {
            get
            {
                // Return the checksum in hexadecimal format.
                return string.Format("0x{0:x2}", SChecksum);
            }
        }

        public byte[] Data
        {
            get
            {
                return ByUDPData;
            }
        }
    }
}