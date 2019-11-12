using System.Net;
using System;
using System.IO;
using System.Windows.Forms;

namespace NetworkSniffer
{
    /// <summary>
    /// This class encapsulates all the IP header fields and provides a mechanism
    /// to set and get the details of them through a parameterized contructor
    /// and public properties respectively.
    /// </summary>
    public class IpHeader
    {
        // Eight bits for version and header length.
        private byte ByVersionAndHeaderLength;
        // Eight bits for differentiated services (TOS).
        private byte ByDifferentiatedServices;
        // Sixteen bits for total length of the datagram (header + message).
        private ushort UsTotalLength;
        // Sixteen bits for identification.
        private ushort UsIdentification;
        // Eight bits for flags and fragmentation offset.
        private ushort usFlagsAndOffset;
        // Eight bits for TTL (Time To Live).
        private byte ByTTL;
        // Eight bits for the underlying protocol.
        private byte ByProtocol;
        // Sixteen bits containing the checksum of the header
        // (checksum can be negative so taken as short).
        private short SChecksum;
        // Thirty two bit source IP Address.
        private uint UiSourceIPAddress;
        // Thirty two bit destination IP Address.
        private uint UiDestinationIPAddress;
        // Header length.
        private byte ByHeaderLength;
        // Data carried by the datagram.
        private byte[] ByIPData = new byte[4096];

        public IpHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                // Create MemoryStream out of the received bytes.
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                // Next we create a BinaryReader out of the MemoryStream.
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                // The first eight bits of the IP header contain the version and
                // header length so we read them.
                ByVersionAndHeaderLength = binaryReader.ReadByte();
                // The next eight bits contain the Differentiated services.
                ByDifferentiatedServices = binaryReader.ReadByte();
                // Next eight bits hold the total length of the datagram.
                UsTotalLength = (ushort)IPAddress.NetworkToHostOrder(
                                            binaryReader.ReadInt16());
                // Next sixteen have the identification bytes.
                UsIdentification = (ushort)IPAddress.NetworkToHostOrder(
                                            binaryReader.ReadInt16());
                // Next sixteen bits contain the flags and fragmentation offset.
                usFlagsAndOffset = (ushort)IPAddress.NetworkToHostOrder(
                                            binaryReader.ReadInt16());
                // Next eight bits have the TTL value.
                ByTTL = binaryReader.ReadByte();
                // Next eight represnts the protocol encapsulated in the datagram.
                ByProtocol = binaryReader.ReadByte();
                // Next sixteen bits contain the checksum of the header.
                SChecksum = IPAddress.NetworkToHostOrder(binaryReader.ReadInt16());
                // Next thirty two bits have the source IP address.
                UiSourceIPAddress = (uint)(binaryReader.ReadInt32());
                // Next thirty two hold the destination IP address.
                UiDestinationIPAddress = (uint)(binaryReader.ReadInt32());
                // Now we calculate the header length.
                ByHeaderLength = ByVersionAndHeaderLength;
                // The last four bits of the version and header length field contain the
                // header length, we perform some simple binary airthmatic operations to
                // extract them.
                ByHeaderLength <<= 4;
                ByHeaderLength >>= 4;
                // Multiply by four to get the exact header length.
                ByHeaderLength *= 4;
                // Copy the data carried by the data gram into another array so that
                // according to the protocol being carried in the IP datagram
                Array.Copy(byBuffer, ByHeaderLength, ByIPData, 0,
                            UsTotalLength - ByHeaderLength);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Network Sniffer", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        public string Version
        {
            get
            {
                // Calculate the IP version. The four bits of the IP header
                // contain the IP version.
                if ((ByVersionAndHeaderLength >> 4) == 4)
                {
                    return "IP v4";
                }
                else if ((ByVersionAndHeaderLength >> 4) == 6)
                {
                    return "IP v6";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string HeaderLength
        {
            get
            {
                return ByHeaderLength.ToString();
            }
        }

        public ushort MessageLength
        {
            get
            {
                // MessageLength = Total length of the datagram - Header length.
                return (ushort)(UsTotalLength - ByHeaderLength);
            }
        }

        public string DifferentiatedServices
        {
            get
            {
                // Returns the differentiated services in hexadecimal format.
                return string.Format("0x{0:x2} ({1})", ByDifferentiatedServices,
                                     ByDifferentiatedServices);
            }
        }

        public string Flags
        {
            get
            {
                // The first three bits of the flags and fragmentation field 
                // represent the flags (which indicate whether the data is 
                // fragmented or not).
                int nFlags = usFlagsAndOffset >> 13;
                if (nFlags == 2)
                {
                    return "Don't fragment";
                }
                else if (nFlags == 1)
                {
                    return "More fragments to come";
                }
                else
                {
                    return nFlags.ToString();
                }
            }
        }

        public string FragmentationOffset
        {
            get
            {
                // The last thirteen bits of the flags and fragmentation field 
                // contain the fragmentation offset.
                int nOffset = usFlagsAndOffset << 3;
                nOffset >>= 3;
                return nOffset.ToString();
            }
        }

        public string TTL
        {
            get
            {
                return ByTTL.ToString();
            }
        }

        public Protocol ProtocolType
        {
            get
            {
                // The protocol field represents the protocol in the data portion
                // of the datagram.
                if (ByProtocol == 6) // A value of six represents the TCP protocol.
                {
                    return Protocol.TCP;
                }
                else if (ByProtocol == 17)  // Seventeen for UDP.
                {
                    return Protocol.UDP;
                }
                else
                {
                    return Protocol.Unknown;
                }
            }
        }

        public string Checksum
        {
            get
            {
                // Returns the checksum in hexadecimal format.
                return string.Format("0x{0:x2}", SChecksum);
            }
        }

        public IPAddress SourceAddress
        {
            get
            {
                return new IPAddress(UiSourceIPAddress);
            }
        }

        public IPAddress DestinationAddress
        {
            get
            {
                return new IPAddress(UiDestinationIPAddress);
            }
        }

        public string TotalLength
        {
            get
            {
                return UsTotalLength.ToString();
            }
        }

        public string Identification
        {
            get
            {
                return UsIdentification.ToString();
            }
        }

        public byte[] Data
        {
            get
            {
                return ByIPData;
            }
        }
    }
}
