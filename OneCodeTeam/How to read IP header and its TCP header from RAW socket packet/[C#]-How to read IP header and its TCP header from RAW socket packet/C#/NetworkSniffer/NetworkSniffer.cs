using System;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace NetworkSniffer
{
    // Types of communication protocol.
    public enum Protocol
    {
        TCP = 6,
        UDP = 17,
        Unknown = -1
    };

    /// <summary>
    /// This class consists of functions that captures incoming packets,
    /// analyze and parses them to display its contents.
    /// </summary>
    public partial class NetworkSnifferForm : Form
    {
        // The socket which captures all incoming packets.
        private Socket MainSocket;
        private byte[] ByteData = new byte[4096];
        //A flag to check if packets are to be captured or not.
        private bool ContinueCapturing = false;
        private delegate void AddTreeNode(TreeNode node);

        public NetworkSnifferForm()
        {
            InitializeComponent();
        }

        private void NetworkSnifferForm_Load(object sender, EventArgs e)
        {
            // Resolves a host name or IP address to an System.Net.IPHostEntry instance.
            IPHostEntry iPHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            if (iPHostEntry.AddressList.Length > 0)
            {
                // Getting a list of IP addresses that are associated with a host.
                cbIpAddressList.DataSource = iPHostEntry.AddressList.Where(
                    ipa => ipa.AddressFamily == AddressFamily.InterNetwork).
                    Select(ip => ip.ToString()).ToList();
            }
        }

        private void btnStartCapture_Click(object sender, EventArgs e)
        {
            if (cbIpAddressList.Text == "")
            {
                MessageBox.Show("Select an interface to capture the packets.",
                    "Network Sniffer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                if (!ContinueCapturing)
                {
                    // Start capturing the packets.
                    btnStartCapture.Text = "&Stop";
                    ContinueCapturing = true;
                    // For sniffing the socket to capture the packet it has to be a raw
                    // socket, with the address family being of type internetwork
                    // and protocol being IP.
                    MainSocket = new Socket(AddressFamily.InterNetwork,
                                            SocketType.Raw, ProtocolType.IP);
                    //Bind the socket to the selected IP address.
                    MainSocket.Bind(new IPEndPoint(IPAddress.Parse
                                    (cbIpAddressList.Text), 0));
                    //Set the socket options.
                    MainSocket.SetSocketOption(SocketOptionLevel.IP,
                                               SocketOptionName.HeaderIncluded, true);
                    byte[] byTrue = new byte[4] { 1, 0, 0, 0 };
                    // Capture outgoing packets.
                    byte[] byOut = new byte[4] { 1, 0, 0, 0 };
                    // Socket.IOControl is analogous to the WSAIoctl method of Winsock 2.
                    MainSocket.IOControl(IOControlCode.ReceiveAll, byTrue, byOut);
                    // Start receiving the packets asynchronously.
                    MainSocket.BeginReceive(ByteData, 0, ByteData.Length,
                                            SocketFlags.None,
                                            new AsyncCallback(OnReceive), null);
                }
                else
                {
                    btnStartCapture.Text = "&Start";
                    ContinueCapturing = false;
                    // To stop capturing the packets close the socket.
                    if (MainSocket != null)
                    {
                        MainSocket.Dispose();
                    }
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Network Sniffer",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This function is being called asynchronously to invoke 'ParseData' method
        /// which analyzes the incoming bytes received.
        /// </summary>
        /// <param name="asyncResult">
        /// Represents the status of an asynchronous operation.
        /// </param>
        private void OnReceive(IAsyncResult asyncResult)
        {
            try
            {
                int nReceived = MainSocket.EndReceive(asyncResult);
                // Analyze the bytes received.
                ParseData(ByteData, nReceived);
                if (ContinueCapturing)
                {
                    ByteData = new byte[4096];
                    // Making another call to BeginReceive so that we continue to receive
                    // the incoming packets.
                    MainSocket.BeginReceive(ByteData, 0, ByteData.Length,
                                            SocketFlags.None,
                                            new AsyncCallback(OnReceive), null);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Network Sniffer",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// This function parses the incoming packets and extracts the data based upon
        /// the protocol being carried by the IP datagram.
        /// </summary>
        /// <param name="byteData">Incoming bytes</param>
        /// <param name="nReceived">The number of bytes received</param>
        private void ParseData(byte[] byteData, int nReceived)
        {
            TreeNode rootNode = new TreeNode();
            // Since all protocol packets are encapsulated in the IP datagram
            // so we start by parsing the IP header and see what protocol data
            // is being carried by it.
            IpHeader ipHeader = new IpHeader(byteData, nReceived);
            TreeNode ipNode = MakeIPTreeNode(ipHeader);
            rootNode.Nodes.Add(ipNode);

            // Now according to the protocol being carried by the IP datagram we parse
            // the data field of the datagram.
            switch (ipHeader.ProtocolType)
            {
                case Protocol.TCP: TcpHeader tcpHeader = new TcpHeader(ipHeader.Data,
                                                           ipHeader.MessageLength);
                    TreeNode tcpNode = MakeTCPTreeNode(tcpHeader);
                    rootNode.Nodes.Add(tcpNode);
                    // If the port is equal to 53 then the underlying protocol is DNS.
                    // Note: DNS can use either TCP or UDP hence checking is done twice.
                    if (tcpHeader.DestinationPort == "53" || 
                        tcpHeader.SourcePort == "53")
                    {
                        TreeNode dnsNode = MakeDNSTreeNode(tcpHeader.Data,
                                            (int)tcpHeader.MessageLength);
                        rootNode.Nodes.Add(dnsNode);
                    }
                    break;
                case Protocol.UDP: UdpHeader udpHeader = new UdpHeader(ipHeader.Data,
                                                           (int)ipHeader.MessageLength);
                    TreeNode udpNode = MakeUDPTreeNode(udpHeader);
                    rootNode.Nodes.Add(udpNode);
                    // If the port is equal to 53 then the underlying protocol is DNS.
                    // Note: DNS can use either TCP or UDP, thats the reason
                    // why the checking has been done twice.
                    if (udpHeader.DestinationPort == "53" || 
                        udpHeader.SourcePort == "53")
                    {
                        TreeNode dnsNode = MakeDNSTreeNode(udpHeader.Data,
                                            Convert.ToInt32(udpHeader.Length) - 8);
                        rootNode.Nodes.Add(dnsNode);
                    }
                    break;
                case Protocol.Unknown:
                    break;
            }

            AddTreeNode addTreeNode = new AddTreeNode(OnAddTreeNode);
            rootNode.Text = ipHeader.SourceAddress.ToString() + "-" +
            ipHeader.DestinationAddress.ToString();
            // Thread safe adding of the nodes.
            treeView.Invoke(addTreeNode, new object[] { rootNode });
        }

        /// <summary>
        /// Helper function which returns the information contained in the IP header
        /// as a tree node.
        /// </summary>
        /// <param name="ipHeader">Object containing all the IP header fields</param>
        /// <returns>TreeNode object returning IP header details</returns>
        private TreeNode MakeIPTreeNode(IpHeader ipHeader)
        {
            TreeNode ipNode = new TreeNode();
            ipNode.Text = "IP";
            ipNode.Nodes.Add("Ver: " + ipHeader.Version);
            ipNode.Nodes.Add("Header Length: " + ipHeader.HeaderLength);
            ipNode.Nodes.Add("Differentiated Services: " +
                            ipHeader.DifferentiatedServices);
            ipNode.Nodes.Add("Total Length: " + ipHeader.TotalLength);
            ipNode.Nodes.Add("Identification: " + ipHeader.Identification);
            ipNode.Nodes.Add("Flags: " + ipHeader.Flags);
            ipNode.Nodes.Add("Fragmentation Offset: " + ipHeader.FragmentationOffset);
            ipNode.Nodes.Add("Time to live: " + ipHeader.TTL);
            switch (ipHeader.ProtocolType)
            {
                case Protocol.TCP:
                    ipNode.Nodes.Add("Protocol: " + "TCP");
                    break;
                case Protocol.UDP:
                    ipNode.Nodes.Add("Protocol: " + "UDP");
                    break;
                case Protocol.Unknown:
                    ipNode.Nodes.Add("Protocol: " + "Unknown");
                    break;
            }
            ipNode.Nodes.Add("Checksum: " + ipHeader.Checksum);
            ipNode.Nodes.Add("Source: " + ipHeader.SourceAddress.ToString());
            ipNode.Nodes.Add("Destination: " + ipHeader.DestinationAddress.ToString());
            return ipNode;
        }

        /// <summary>
        /// Helper function which returns the information contained in the TCP
        /// header as a tree node.
        /// </summary>
        /// <param name="tcpHeader">Object containing all the TCP header fields</param>
        /// <returns>TreeNode object returning TCP header details</returns>
        private TreeNode MakeTCPTreeNode(TcpHeader tcpHeader)
        {
            TreeNode tcpNode = new TreeNode();
            tcpNode.Text = "TCP";
            tcpNode.Nodes.Add("Source Port: " + tcpHeader.SourcePort);
            tcpNode.Nodes.Add("Destination Port: " + tcpHeader.DestinationPort);
            tcpNode.Nodes.Add("Sequence Number: " + tcpHeader.SequenceNumber);
            if (tcpHeader.AcknowledgementNumber != "")
            {
                tcpNode.Nodes.Add("Acknowledgement Number: " +
                                tcpHeader.AcknowledgementNumber);
            }
            tcpNode.Nodes.Add("Header Length: " + tcpHeader.HeaderLength);
            tcpNode.Nodes.Add("Flags: " + tcpHeader.Flags);
            tcpNode.Nodes.Add("Window Size: " + tcpHeader.WindowSize);
            tcpNode.Nodes.Add("Checksum: " + tcpHeader.Checksum);
            if (tcpHeader.UrgentPointer != "")
            {
                tcpNode.Nodes.Add("Urgent Pointer: " + tcpHeader.UrgentPointer);
            }
            return tcpNode;
        }

        /// <summary>
        /// Helper function which returns the information contained in the UDP
        /// header as a tree node.
        /// </summary>
        /// <param name="udpHeader">Object containing all the UDP header fields</param>
        /// <returns>TreeNode object returning UDP header details</returns>
        private TreeNode MakeUDPTreeNode(UdpHeader udpHeader)
        {
            TreeNode udpNode = new TreeNode();
            udpNode.Text = "UDP";
            udpNode.Nodes.Add("Source Port: " + udpHeader.SourcePort);
            udpNode.Nodes.Add("Destination Port: " + udpHeader.DestinationPort);
            udpNode.Nodes.Add("Length: " + udpHeader.Length);
            udpNode.Nodes.Add("Checksum: " + udpHeader.Checksum);
            return udpNode;
        }

        /// <summary>
        /// Helper function which returns the information contained in the DNS
        /// header as a tree node.
        /// </summary>
        /// <param name="byteData">Incoming bytes</param>
        /// <param name="nLength">Number of bytes received</param>
        /// <returns></returns>
        private TreeNode MakeDNSTreeNode(byte[] byteData, int nLength)
        {
            DnsHeader dnsHeader = new DnsHeader(byteData, nLength);
            TreeNode dnsNode = new TreeNode();
            dnsNode.Text = "DNS";
            dnsNode.Nodes.Add("Identification: " + dnsHeader.Identification);
            dnsNode.Nodes.Add("Flags: " + dnsHeader.Flags);
            dnsNode.Nodes.Add("Questions: " + dnsHeader.TotalQuestions);
            dnsNode.Nodes.Add("Answer RRs: " + dnsHeader.TotalAnswerRRs);
            dnsNode.Nodes.Add("Authority RRs: " + dnsHeader.TotalAuthorityRRs);
            dnsNode.Nodes.Add("Additional RRs: " + dnsHeader.TotalAdditionalRRs);
            return dnsNode;
        }

        private void OnAddTreeNode(TreeNode node)
        {
            treeView.Nodes.Add(node);
        }

        private void NetworkSnifferForm_FormClosing(object sender, 
                                                    FormClosingEventArgs e)
        {
            if (ContinueCapturing && MainSocket != null)
            {
                MainSocket.Close();
            }
        }
    }
}