/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;

namespace SocketsXO_Server
{
    public static class GameLogic
    {
        static Random _rnd;
        public static string Play(string incoming)
        {
            if (!String.IsNullOrWhiteSpace(incoming))
            {
                incoming = incoming.Trim("\0".ToCharArray());
                string[] pairs = incoming.Split('|');

                string playAs = pairs[0];
                List<string> list = new List<string>();
                for (int i = 1; i < pairs.Length; i++)
                {
                    string[] pair = pairs[i].Split('*');

                    if (String.IsNullOrWhiteSpace(pair[1]))
                    {
                        list.Add(pair[0]);
                    }

                }

                if (_rnd == null)
                    _rnd = new Random(DateTime.Now.Millisecond);


                return list.Count > 0 ? list[_rnd.Next(0, list.Count)] : String.Empty;
            }

            return String.Empty;


        }
    }
}
