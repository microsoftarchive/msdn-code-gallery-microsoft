/****************************** Module Header ******************************\
*   Module Name:  <Cell.cs>
*   Project:	<CSWPFMVVMPractice>
* Copyright (c) Microsoft Corporation.
* 
* The Cell class is a Model in the MVVM pattern, which represents a cell in the TicTacToe 
* game grid.
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
    public class Cell : INotifyPropertyChanged
    {
        public Cell(int cellNumber)
        {
            this.cellNumber = cellNumber;
        }

        private int cellNumber;
        /// <summary>
        /// The readonly CellNumber property returns the number of the cell
        /// </summary>
        public int CellNumber
        {
            get
            {
                return cellNumber;
            }
        }

        private PlayerMove move;
        /// <summary>
        /// The Move property represents a player's move on the cell
        /// </summary>
        public PlayerMove Move
        {
            get
            {
                return move;
            }
            set
            {
                if (move == value)
                {
                    return;
                }
                move = value;
                Notify("Move");
            }
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
