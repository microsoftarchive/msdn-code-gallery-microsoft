/****************************** Module Header ******************************\
*   Module Name:  <TicTacToeViewModel.cs>
*   Project:	<CSWPFMVVMPractice>
* Copyright (c) Microsoft Corporation.
* 
* The TicTacToeViewModel class is a ViewModel in the MVVM pattern, which contains the game's 
* logic and data.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace CSWPFMVVMPractice
{
    public class TicTacToeViewModel :  INotifyPropertyChanged
    {
        #region private fields
            
        private int dimension;
        private ObservableCollection<Cell> cells;
        private string currentPlayerName;
        private int currentMoveNumber;    
        private RelayCommand moveCommand;
        private RelayCommand changeDimensionCommand;

        #endregion
        
        #region properties
        /// <summary>
        /// The Dimension property gets and sets the dimension of the game
        /// </summary>
        public int Dimension
        {
            get
            {
                return dimension;
            }
            set
            {
                if (dimension == value)
                {
                    return;
                }
                dimension = value;
                Notify("Dimension");
             
                // if the cells is null, initialize it; otherwise, clear the collection             
                if (cells == null)
                {                    
                    cells = new ObservableCollection<Cell>();
                }
                else
                {                 
                    cells.Clear();
                }
                
                // Add Cell objects to the cells collection according to the dimension of the game                
                for (int i = 0; i < dimension * dimension; i++)
                {
                    cells.Add(new Cell(i));
                }  
            }
            
        }
        /// <summary>
        /// The readonly Cells property returns a collection of cells of the current game
        /// </summary>
        public ObservableCollection<Cell> Cells
        {
            get
            {
                return cells;
            }
        }

        /// <summary>
        /// The CurrentPlayerName property gets and sets the name of the current player
        /// </summary>
        public string CurrentPlayerName
        {
            get
            {
                return currentPlayerName;
            }
            private set
            {
                if (currentPlayerName == value)
                {
                    return;
                }
                currentPlayerName = value;
                Notify("CurrentPlayerName");
            }
        }
      
        #endregion

        #region construction

        public TicTacToeViewModel(int dimension)
        {
            this.Dimension = dimension;           
        }
        #endregion

        #region mehods
        /// <summary>
        /// The NewGame method starts a new game
        /// </summary>
        public void NewGame()
        {       
            // reset the Move property of all the Cell objects         
            for (int i = 0; i < this.cells.Count; i++)
            {
                this.cells[i].Move = null;
            }
            
            // set the current player name and current move number            
            CurrentPlayerName = "x";
            currentMoveNumber = 1;
        }

        /// <summary>
        /// The CanMove method determines whether a player can move on the specified cell
        /// </summary>
        /// <param name="cellNumber">The number of the cell</param>
        /// <returns>If true, can move; otherwise, can't move</returns>
        private bool CanMove(int cellNumber)
        {
            if (cellNumber >= dimension * dimension)
            {
                return false;
            }
            else if (cells[cellNumber].Move == null)
            {
                return true;
            }            
            else
            {
                return false;
            }           
        }
        /// <summary>
        /// The Move method sets a PlayerMove object for the current player on the specified Cell object 
        /// </summary>
        /// <param name="cellNumber">The number of the cell</param>
        private void Move(int cellNumber)
        {
            cells[cellNumber].Move = new PlayerMove(currentPlayerName, currentMoveNumber++);    
    
            // Check if the game is over, i.e. the current player wins the game or the game is tie. If 
            // so, raise the GameOver event and starts a new game.            
            if (HasWon(currentPlayerName))
            {
                RaiseGameOverEvent(new GameOverEventArgs() { IsTie = false, 
                    WinnerName = currentPlayerName });               
                NewGame();
            }
            else if (TieGame())
            {
                RaiseGameOverEvent(new GameOverEventArgs() { IsTie = true });
                NewGame();
            }
    
            // If the game isn't over, switch the current player    
            else
            {
                if (currentPlayerName.Equals("x"))
                {
                    CurrentPlayerName = "o";
                }
                else
                {
                    CurrentPlayerName = "x";
                }
            }
        }
        
        /// <summary>
        /// The HasWon method checks whether the given player has won
        /// </summary>
        /// <param name="player">The name of the player</param>
        /// <returns></returns>
        bool HasWon(string player)
        {
       
            // check the rows in the game grid       
            for (int i = 0; i <= (dimension - 1) * dimension; i += dimension)
            {
                int j = 0;
                for (; j <= dimension - 1; j++)
                {
                    if (cells[i + j].Move == null || !cells[i + j].Move.PlayerName.Equals(player))
                    {
                        break;
                    }
                }
                if (j == dimension)
                {
                    for (j = 0; j <= dimension - 1; j++)
                    {
                        cells[i + j].Move.IsPartOfWin = true;
                    }              
                    return true;
                }
            }
       
            // check the columns in the game grid       
            for (int j = 0; j <= dimension - 1; j++)
            {
                int i = 0;
                for (; i <= (dimension - 1) * dimension; i += dimension)
                {
                    if (cells[i + j].Move == null || !cells[i + j].Move.PlayerName.Equals(player))
                    {
                        break;
                    }
                }
                if (i == dimension * dimension)
                {
                    for (i = 0; i <= (dimension - 1) * dimension; i += dimension)
                    {
                        cells[i + j].Move.IsPartOfWin = true;
                    }                
                    return true;
                }
            }
                      
            // check the diagonal line ( "\" ) in the game grid             
            int x = 0;
            for (; x < dimension * dimension; x += dimension + 1)
            {
                if (cells[x].Move == null || !cells[x].Move.PlayerName.Equals(player))
                {
                    break;
                }
            }
            if (x == dimension * dimension + dimension)
            {
                for (x = 0; x < dimension * dimension; x += dimension + 1)
                {
                    cells[x].Move.IsPartOfWin = true;
                }         
                return true;
            }
                      
            // check the diagonal line ( "/" ) in the game grid            
            int y = dimension - 1;
            for (; y <= dimension * (dimension - 1); y += dimension - 1)
            {
                if (cells[y].Move == null || !cells[y].Move.PlayerName.Equals(player))
                {
                    break;
                }
            }
            if (y == dimension * dimension - 1)
            {
                for (y = dimension - 1; y <= dimension * (dimension - 1); y += dimension - 1)
                {
                    cells[y].Move.IsPartOfWin = true;
                }         
                return true;
            }
              
            // If all the checks above fail, return false     
            return false;
        }

        /// <summary>
        /// The TieGame method checks if the game is tie
        /// </summary>
        /// <returns></returns>
        bool TieGame()
        {
            bool nomove = true;           
            for (int i = 0; i < dimension * dimension; i++)
            {
                if (cells[i].Move == null)
                {
                    nomove = false;
                    break;
                }
            }
            if (nomove)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// The Notify method is used to raise a PropertyChanged event when a property value is 
        /// changed
        /// </summary>
        /// <param name="propName">The name of the property whose value is changed.</param>
        void Notify(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        /// <summary>
        /// The RaiseGameOverEvent method raise the GameOverEvent
        /// </summary>
        /// <param name="e">A GameOverEventArgs object</param>
        void RaiseGameOverEvent(GameOverEventArgs e)
        {
            if (GameOverEvent != null)
            {
                GameOverEvent(this, e);
            }
        }
            
        #endregion

        #region event
     
        public event PropertyChangedEventHandler PropertyChanged;
        public class GameOverEventArgs: EventArgs
        {
            public bool IsTie
            {
                get;
                set;
            }
            public string WinnerName
            {
                get;
                set;
            }
        }
       public delegate void GameOverEventHandler(object sender, GameOverEventArgs e);
       public event GameOverEventHandler GameOverEvent;
       
        #endregion

       #region command
        /// <summary>
        /// The MoveCommand represents a request to move on a given cell
        /// </summary>
       public ICommand MoveCommand
        {
            get
            {
                if (moveCommand == null)
                {
                    moveCommand = new RelayCommand
                        (cellnumber => this.Move(Convert.ToInt32(cellnumber)),
                        cellnumber=>this.CanMove(Convert.ToInt32(cellnumber)));
                }
                return moveCommand;
            }
        }

        /// <summary>
        /// The ChangeDimensionCommand represents a request to change the dimension of the game
        /// </summary>
       public ICommand ChangeDimensionCommand
       {
           get
           {
               if (changeDimensionCommand == null)
               {
                   changeDimensionCommand = new RelayCommand(
                       dimension =>
                       {
                           this.Dimension = Convert.ToInt32(dimension);
                           NewGame();
                       });
               }
               return changeDimensionCommand;
           }
       }
        
        #endregion
    }
}
