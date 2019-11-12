/****************************** Module Header ******************************\
* Module Name:  SimpleObject.cs
* Project:      CSDllCOMServer
* Copyright (c) Microsoft Corporation.
* 
* This sample focuses on exposing .NET Framework components to COM, which 
* allows us to write a .NET type and consuming that type from unmanaged code 
* with distinct activities for COM developers. The code file defines a COM 
* component whose class interface is explicitly defined:
* 
* SimpleObject - [Explicitly Define a Class Interface]
* 
* Program ID: CSDllCOMServer.CSExplicitInterfaceObject
* CLSID_CSExplicitInterfaceObject: 4B65FE47-2F9D-37B8-B3CB-5BE4A7BC0926
* IID_ICSExplicitInterfaceObject: 32DBA9B0-BE1F-357D-827F-0196229FA0E2
* DIID_ICSExplicitInterfaceObjectEvents: 95DB823B-E204-428c-92A3-7FB29C0EC576
* LIBID_CSDllCOMServer: F0998D9A-0E79-4F67-B944-9E837F479587
* 
* Properties:
* // With both get and set accessor methods.
* public float FloatProperty
* 
* Methods:
* // HelloWorld returns a string "HelloWorld"
* public string HelloWorld();
* // GetProcessThreadID outputs the running process ID and thread ID
* public void GetProcessThreadID(out uint processId, out uint threadId);
* 
* Events:
* // FloatPropertyChanging is fired before new value is set to the 
* // FloatProperty property. The Cancel parameter allows the client to cancel 
* // the change of FloatProperty.
* void FloatPropertyChanging(float NewValue, ref bool Cancel);
* 
* -------
* The recommended way of modeling a .NET component to be exposed to the COM  
* aware clients is to do away with ClassInterface, and instead, explicitly 
* factor out the members to be exported into a separate interface, and have 
* the .NET component implement that interface. Using a Class Interface is a 
* quick and easy way to get the .NET component exposed to COM aware clients 
* (See CSImplicitInterfaceObject), but it is not the recommended way. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Runtime.InteropServices;
#endregion


namespace CSDllCOMServer
{
    #region Interfaces

    /// <summary>
    /// The public interface describing the COM interface of the coclass 
    /// </summary>
    [Guid("32DBA9B0-BE1F-357D-827F-0196229FA0E2")]          // IID
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    [ComVisible(true)]
    // Dual interface by default. This allows the client to get the best of 
    // both early binding and late binding.
    //[InterfaceType(ComInterfaceType.InterfaceIsDual)]
    //[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    //[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    public interface ISimpleObject
    {
        #region Properties

        float FloatProperty { get; set; }

        #endregion

        #region Methods

        string HelloWorld();

        void GetProcessThreadID(out uint processId, out uint threadId);

        [ComVisible(false)]
        void HiddenFunction();

        #endregion
    }

    /// <summary>
    /// The public interface describing the events the coclass can sink
    /// </summary>
    [Guid("95DB823B-E204-428c-92A3-7FB29C0EC576")]
    [InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
    [ComVisible(true)]
    public interface ISimpleObjectEvents
    {
        #region Events

        [DispId(1)]
        void FloatPropertyChanging(float NewValue, ref bool Cancel);

        #endregion
    }

    #endregion

    [ClassInterface(ClassInterfaceType.None)]           // No ClassInterface
    [ComSourceInterfaces(typeof(ISimpleObjectEvents))]
    [Guid("4B65FE47-2F9D-37B8-B3CB-5BE4A7BC0926")]      // CLSID
    //[ProgId("CSCOMServerDll.CustomSimpleObject")]     // ProgID
    [ComVisible(true)]
    public class SimpleObject : ISimpleObject
    {
        #region Properties

        /// <summary>
        /// The private members don't make it into the type-library and are 
        /// hidden from the COM clients.
        /// </summary>
        private float fField = 0;

        /// <summary>
        /// A public property with both get and set accessor methods.
        /// </summary>
        public float FloatProperty
        {
            get { return this.fField; }
            set
            {
                bool cancel = false;
                // Raise the event FloatPropertyChanging
                if (null != FloatPropertyChanging)
                    FloatPropertyChanging(value, ref cancel);
                if (!cancel)
                    this.fField = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// A public method that returns a string "HelloWorld".
        /// </summary>
        /// <returns>"HelloWorld"</returns>
        public string HelloWorld()
        {
            return "HelloWorld";
        }

        /// <summary>
        /// A public method with two outputs: the current process Id and the
        /// current thread Id.
        /// </summary>
        /// <param name="processId">[out] The current process Id</param>
        /// <param name="threadId">[out] The current thread Id</param>
        public void GetProcessThreadID(out uint processId, out uint threadId)
        {
            processId = NativeMethod.GetCurrentProcessId();
            threadId = NativeMethod.GetCurrentThreadId();
        }

        /// <summary>
        /// A hidden method (ComVisible = false)
        /// </summary>
        public void HiddenFunction()
        {
            Console.WriteLine("HiddenFunction is called.");
        }
        
        #endregion

        #region Events

        [ComVisible(false)]
        public delegate void FloatPropertyChangingEventHandler(float NewValue, ref bool Cancel);

        /// <summary>
        /// A public event that is fired before new value is set to the
        /// FloatProperty property. The Cancel parameter allows the client 
        /// to cancel the change of FloatProperty.
        /// </summary>
        public event FloatPropertyChangingEventHandler FloatPropertyChanging;

        #endregion
    }
}