
//****************************** Module Header ******************************\
//Module Name:    Program.cs
//Project:        WakeUpOnLan
//Copyright (c) Microsoft Corporation

// The project illustrates how to check whether a file is in use or not.

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace WakeUpOnLan
{
     // *************************************************************************
     /// <summary>
     /// A class to demonstrate Wake up on LAN. For this program to function, your
     /// machine must be setup to accept wake-up on LAN requests. Usually this
     /// option can set to "Enabled" state in boot(BIOS) options. You can see the
     /// set value by Rebooting your PC/Laptop and press F2 just after you power
     /// on your PC/Laptop. (you might have to keep pressing F2 until it comes up
     /// with boot options)
     /// </summary>
     /// <remarks>
     /// <para>
     /// For more information see http://support.microsoft.com/kb/257277 and
     /// http://en.wikipedia.org/wiki/Wake-on-LAN.
     /// </para>
     /// <para>For an in depth details please visit :
     /// http://en.wikipedia.org/wiki/Data_link_layer and 
     /// http://en.wikipedia.org/wiki/Network_layer </para>
     /// </remarks>
    public sealed class WakeUpOnLan
    {
        static void Main(string[] args)
        {

             bool wakeUp = true;
                while (wakeUp)
                {

                ///4437E694B11E 
                Console.WriteLine("Enter the MAC address of the host to wake up " +
                         " on LAN (no space or hyphen(-). e.g. 0021702BA7A5." +
                         "Type Exit to end the program):");
                     string macAddress = Console.ReadLine();

                
                     StringComparer cp = StringComparer.OrdinalIgnoreCase;
                      if (cp.Compare(macAddress, "Exit") == 0) break;

                
                      //remove all non 0-9, A-F, a-f characters 

                    macAddress = Regex.Replace(macAddress, @"[^0-9A-Fa-f]", "");
                          //check if mac adress length is valid

                    if (macAddress.Length != 12)
                    {
                        Console.WriteLine("Invalid MAC address. Try again!");
                     }
                     else
                     {
                          Wakeup(macAddress);
                      }
               }
        }


        // *********************************************************************
        /// <summary>
        /// Wakes up the machine with the given <paramref name="macAddress"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <note>
        /// <list type="number">
        /// <item> The motherboard must support Wake On LAN.</item>
        /// <item> The NIC must support Wake On LAN.</item>
        /// <item> There must be a wire connecting the motherboard's WOL port to
        /// the NIC's WOL port. Usually there always is a connection on most of
        /// the PCs.</item>
        /// <item> The Wake On LAN feature must be enabled in the motherboard's
        /// BIOS. Usually this is also enabled by default, but you might like to
        /// check again.</item>
        /// <item> The "Good Connection" light on the back of the NIC must be lit
        /// when the machine is off. (By default always good if you are not
        /// facing any network issues)</item>
        /// <item> Port 12287 (0x2FFF) must be open. (By default it should be
        /// open unless some antivirus or any other such program has changed 
        /// settings.)</item>
        /// <item> Packets cannot be broadcast across the Internet.  That's why
        /// it's called Wake On Lan, not Wake On Internet.</item>
        /// <item> To find your MAC address, run the MSINFO32.EXE tool that is a
        /// part of Windows and navigate to Components > Network > Adapteror
        /// or simply type nbtstat -a &lt;your hostname &lt at command prompt.
        /// e.g. nbtstat -a mymachinename or nbtstat -A 10.2.100.213.
        /// <param name="macAddress">The MAC address of the host which has to be
        /// woken up.</param>
        /// 
        private static void Wakeup(string macAddress)
        {
            WOLUdpClient client = new WOLUdpClient();
            client.Connect(
            new IPAddress(0xffffffff),    //255.255.255.255  i.e broadcast
            0x2fff); // port = 12287
            if (client.IsClientInBrodcastMode())
            {
                int byteCount = 0;
                byte[] bytes = new byte[102];
                for (int trailer = 0; trailer < 6; trailer++)
                {
                    bytes[byteCount++] = 0xFF;
                }
                for (int macPackets = 0; macPackets < 16; macPackets++)
                {
                    int i = 0;
                    for (int macBytes = 0; macBytes < 6; macBytes++)
                    {
                        bytes[byteCount++] =
                        byte.Parse(macAddress.Substring(i, 2),
                        NumberStyles.HexNumber);
                        i += 2;
                    }
                }

                int returnValue = client.Send(bytes, byteCount);
                Console.WriteLine(returnValue + " bytes sent to " + macAddress +
                Environment.NewLine + "Check if it's woken up. If not, try again!" +
                Environment.NewLine);
            }
            else
            {
                Console.WriteLine("Remote client could not be set in broadcast mode!");
            }
        }


        public class WOLUdpClient : UdpClient
        {
            // *********************************************************************
            /// <summary>
            /// Initializes a new instance of <see cref="WOLUdpClient"/>.
            /// </summary>
            public WOLUdpClient() : base()
            {
            }

            // *********************************************************************
            /// <summary>
            /// Sets up the UDP client to broadcast packets.
            /// </summary>
            /// <returns><see langword="true"/> if the UDP client is set in
            /// broadcast mode.</returns>
            public bool IsClientInBrodcastMode()
            {
                bool broadcast = false;
                if (this.Active)
                {
                    try
                    {
                        this.Client.SetSocketOption(SocketOptionLevel.Socket,
                             SocketOptionName.Broadcast, 0);
                        broadcast = true;
                    }
                    catch
                    {
                        broadcast = false;
                    }
                }
                return broadcast;
            }
        }

        }
}
