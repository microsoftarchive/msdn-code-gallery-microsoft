using System.Net;
using System;
using System.IO;
using System.Windows.Forms;

namespace NetworkSniffer
{
    /// <summary>
    /// This class encapsulates all the TCP header fields and provides a mechanism
    /// to set and get the details of them through a parameterized contructor
    /// and public properties respectively.
    /// </summary>
    public class TcpHeader
    {
        // Sixteen bits for the source port number.
        private ushort UsSourcePort;
        // Sixteen bits for the destination port number.
        private ushort UsDestinationPort;
        // Thirty two bits for the sequence number.
        private uint UiSequenceNumber = 555;
        // Thirty two bits for the acknowledgement number.
        private uint UiAcknowledgementNumber = 555;
        // Sixteen bits for flags and data offset.
        private ushort UsDataOffsetAndFlags = 555;
        // Sixteen bits for the window size.
        private ushort UsWindow = 555;
        // Sixteen bits for the checksum, (checksum can be negative so taken as short).
        private short SChecksum = 555;
        // Sixteen bits for the urgent pointer.  
        private ushort UsUrgentPointer;
        // Header length.
        private byte ByHeaderLength;
        // Length of the data being carried.
        private ushort UsMessageLength;
        // Data carried by the TCP packet.
        private byte[] ByTCPData = new byte[4096];

        public TcpHeader(byte[] byBuffer, int nReceived)
        {
            try
            {
                // Create MemoryStream out of the received bytes.
                MemoryStream memoryStream = new MemoryStream(byBuffer, 0, nReceived);
                // Next we create a BinaryReader out of the MemoryStream.
                BinaryReader binaryReader = new BinaryReader(memoryStream);
                // The first sixteen bits contain the source port.
                UsSourcePort = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
                // The next sixteen contain the destiination port.
                UsDestinationPort = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
                // Next thirty two have the sequence number.
                UiSequenceNumber = (uint)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt32());
                // Next thirty two have the acknowledgement number.
                UiAcknowledgementNumber = (uint)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt32());
                // The next sixteen bits hold the flags and the data offset.
                UsDataOffsetAndFlags = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
                // The next sixteen contain the window size.
                UsWindow = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
                // In the next sixteen we have the checksum.
                SChecksum = (short)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
                // The following sixteen contain the urgent pointer.
                UsUrgentPointer = (ushort)IPAddress.NetworkToHostOrder(
                                        binaryReader.ReadInt16());
                // The data offset indicates where the data begins, so using it we
                // calculate the header length.
                ByHeaderLength = (byte)(UsDataOffsetAndFlags >> 12);
                ByHeaderLength *= 4;
                // Message length = Total length of the TCP packet - Header length.
                UsMessageLength = (ushort)(nReceived - ByHeaderLength);
                // Copy the TCP data into the data buffer.
                Array.Copy(byBuffer, ByHeaderLength, ByTCPData, 0,
                                        nReceived - ByHeaderLength);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Network Sniffer" + (nReceived), 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        public string SequenceNumber
        {
            get
            {
                return UiSequenceNumber.ToString();
            }
        }

        public string AcknowledgementNumber
        {
            get
            {
                // If the ACK flag is set then only we have a valid value in the
                // acknowlegement field, so check for it beore returning anything.
                if ((UsDataOffsetAndFlags & 0x10) != 0)
                {
                    return UiAcknowledgementNumber.ToString();
                }
                else
                {
                    return "";
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

        public string WindowSize
        {
            get
            {
                return UsWindow.ToString();
            }
        }

        public string UrgentPointer
        {
            get
            {
                // If the URG flag is set then only we have a valid value in the urgent
                // pointer field, so check for it beore returning anything.
                if ((UsDataOffsetAndFlags & 0x20) != 0)
                {
                    return UsUrgentPointer.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        public string Flags
        {
            get
            {
                // The last six bits of data offset and flags contain the control bits.
                // First we extract the flags.
                int nFlags = UsDataOffsetAndFlags & 0x3F;
                string strFlags = string.Format("0x{0:x2} (", nFlags);
                // Now we start looking whether individual bits are set or not.
                if ((nFlags & 0x01) != 0)
                {
                    strFlags += "FIN, ";
                }
                if ((nFlags & 0x02) != 0)
                {
                    strFlags += "SYN, ";
                }
                if ((nFlags & 0x04) != 0)
                {
                    strFlags += "RST, ";
                }
                if ((nFlags & 0x08) != 0)
                {
                    strFlags += "PSH, ";
                }
                if ((nFlags & 0x10) != 0)
                {
                    strFlags += "ACK, ";
                }
                if ((nFlags & 0x20) != 0)
                {
                    strFlags += "URG";
                }
                strFlags += ")";

                if (strFlags.Contains("()"))
                {
                    strFlags = strFlags.Remove(strFlags.Length - 3);
                }
                else if (strFlags.Contains(", )"))
                {
                    strFlags = strFlags.Remove(strFlags.Length - 3, 2);
                }
                return strFlags;
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
                return ByTCPData;
            }
        }

        public ushort MessageLength
        {
            get
            {
                return UsMessageLength;
            }
        }
    }
}