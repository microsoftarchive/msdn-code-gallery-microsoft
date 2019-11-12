/****************************** Module Header ******************************\
*   Module Name:  <PlayerMove.cs>
*   Project:	<CSWPFMVVMPractice>
* Copyright (c) Microsoft Corporation.
* 
* The PlayerMove class is a Model in the MVVM pattern, which represents a player's move.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel;

namespace CSWPFMVVMPractice
{    
    public class PlayerMove : INotifyPropertyChanged
    {
        string playerName;
        /// <summary>
        /// The PlayerName property gets and sets the player's name
        /// </summary>
        public string PlayerName
        {
            get
            {
                return playerName;
            }
            set
            {
                if (string.Compare(playerName, value) == 0)
                {
                    return;
                }
                playerName = value;
                Notify("PlayerName");
            }
        }

        int moveNumber;
        /// <summary>
        /// The MoveNumber property gets and sets the number of this move of the player
        /// </summary>
        public int MoveNumber
        {
            get
            {
                return moveNumber;
            }
            set
            {
                if (moveNumber == value)
                {
                    return;
                }
                moveNumber = value;
                Notify("MoveNumber");
            }
        }

        bool isPartOfWin = false;
        /// <summary>
        /// The IsPartOfWin property gets and sets whether the move is part of all the moves of 
        /// the winner
        /// </summary>
        public bool IsPartOfWin
        {
            get
            {
                return isPartOfWin;
            }
            set
            {
                if (isPartOfWin == value)
                {
                    return;
                }
                isPartOfWin = value;
                Notify("IsPartOfWin");
            }
        }

        public PlayerMove(string playerName, int moveNumber)
        {
            this.playerName = playerName;
            this.moveNumber = moveNumber;
        }

        // INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
