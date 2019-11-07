/****************************** Module Header ******************************\
* Module Name:    PageMethods.js
* Project:        VBASPNETStaticCodeByPageMethod
* Copyright (c) Microsoft Corporation
*
* This file is used to store getSayHello function.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

// Gets the SayHello value.
function getSayHello(src) {
    var name = document.getElementById(src.id).value;
    PageMethods.sayHello(name,
        OnSucceeded);
}

// Callback function invoked on successful 
// completion of the page method.
function OnSucceeded(result) {   
       alert(result);
    
}

