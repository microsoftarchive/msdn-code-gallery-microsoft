
//****************************** Module Header ******************************\
//Module Name:    Program.cs
//Project:        Serialport_communication
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
using System.IO;
using System.IO.Ports;

namespace Serialport_communication
{
    class Program
    {
        static void Main(string[] args)
        {


            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                   OpenAndCommunicateport(port);
            }

     
        }

        public static void OpenAndCommunicateport(string str)
        {
            SerialPort myserialPort;
            myserialPort = new SerialPort(str, 9600);
            try
            { 
                myserialPort.Open(); //open th eserial port
                byte b = (byte)myserialPort.ReadByte(); ///read a byte
                char c = (char)myserialPort.ReadChar(); // read a char
                string line = (string)myserialPort.ReadLine(); //read a whole line
                string all = (string)myserialPort.ReadExisting(); //read everythin in the buffer

                ///print all the results
                Console.WriteLine("Byte: " + b);
                Console.WriteLine("Char: " + c);
                Console.WriteLine("Line: " + line);
                Console.WriteLine("All: " + all);


                myserialPort.Close();
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex);
            }
}
    }
}
