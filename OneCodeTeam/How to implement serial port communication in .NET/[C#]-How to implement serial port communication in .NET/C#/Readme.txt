How to implement serial port communication in .NET 
Overview: This code sample written for demonstrating serial port communication. This program display the data from serial port.
Code details:
C.Net framework has came up implementation of serial port communication with System.IO.Ports name space.
Under this name space we have SerialPort class supports functionalities to get available COM ports and open and closing the ports. And lao this exposes to read bytes, character and data from the port. 
Below is the code base for more clarity:

  string[] ports = SerialPort.GetPortNames();
  SerialPort myserialPort;
  myserialPort = new SerialPort(str, 9600);
  myserialPort.Open(); //open th eserial port
  byte b = (byte)myserialPort.ReadByte(); ///read a byte
  char c = (char)myserialPort.ReadChar(); // read a char
  string line = (string)myserialPort.ReadLine(); //read a whole line
  string all = (string)myserialPort.ReadExisting(); //read everythin in the buffer
  myserialPort.Close();

Test:
To test this program, build the program successful without error.
It will display the data if the port the opens. 

