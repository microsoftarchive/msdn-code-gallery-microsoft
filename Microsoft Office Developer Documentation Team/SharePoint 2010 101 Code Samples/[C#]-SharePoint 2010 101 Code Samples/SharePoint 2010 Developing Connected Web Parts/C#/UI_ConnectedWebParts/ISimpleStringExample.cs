using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI_ConnectedWebParts
{
    /// <summary>
    /// This project shows how to connect two web parts to exchange information. This
    /// interface defines the kind of information the web parts can share. For this 
    /// simple example, the information is a simple string.
    /// </summary>
    public interface ISimpleStringExample
    {
        
        string SimpleString { get; set; }

    }
}
