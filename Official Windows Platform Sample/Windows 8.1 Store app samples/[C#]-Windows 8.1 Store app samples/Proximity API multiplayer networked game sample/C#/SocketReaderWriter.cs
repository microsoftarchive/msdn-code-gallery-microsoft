using System;
using System.Collections.Generic;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CritterStomp
{
    public enum Message
    {
        Start = 101,
        Ready,
        Pause,
        Stop,
        NavigateToGame,
        Navigated,
        Done,
        Closing,
        Complete,
    }

    public class SocketReaderWriter : IDisposable
    {
        DataReader dataReader;
        DataWriter dataWriter;
        StreamSocket streamSocket;

        MainPage mainPage;
        GamePage gamePage;
        GameLobby gameLobby;
        WaitingForHost waitForHost;
        InvitePlayersPage invitePlayerPage;
        ScorePage scorePage;

        public Page currentPage;
        public string singlePairRole;
        public string currentMessage;
        public string fullDisplayName;

        public SocketReaderWriter()
        {
        }

        public SocketReaderWriter(StreamSocket socket, Page page)
        {
            dataReader = new DataReader(socket.InputStream);
            dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
            dataReader.ByteOrder = ByteOrder.LittleEndian;

            dataWriter = new DataWriter(socket.OutputStream);
            dataWriter.UnicodeEncoding = UnicodeEncoding.Utf8;
            dataWriter.ByteOrder = ByteOrder.LittleEndian;

            streamSocket = socket;
            currentPage = page;

            UpdateCurrentPageType();
        }

        public void Dispose()
        {
            dataReader.Dispose();
            dataWriter.Dispose();
            streamSocket.Dispose();
        }

        public void UpdateCurrentPageType()
        {
            mainPage = currentPage as MainPage;
            gamePage = currentPage as GamePage;
            gameLobby = currentPage as GameLobby;
            invitePlayerPage = currentPage as InvitePlayersPage;
            waitForHost = currentPage as WaitingForHost;
            scorePage = currentPage as ScorePage;
        }

        public void WriteMessage(Message message)
        {
            WriteMessage(message.ToString());
        }

        public async void WriteMessage(string streamSocket)
        {
            try
            {
                dataWriter.WriteUInt32(dataWriter.MeasureString(streamSocket));
                dataWriter.WriteString(streamSocket);
                await dataWriter.StoreAsync();
            }
            catch (Exception err)
            {
                PrintMessage(err.ToString());
            }
        }

        public void PrintMessage(string message)
        {
            if (mainPage != null)
            {
                mainPage.Invitation = message;
            }

            if (gameLobby != null)
            {
                gameLobby.AcceptInvitation = message;
            }

            if (invitePlayerPage != null)
            {
                invitePlayerPage.SendInvitations = message;
            }

            if (gamePage != null)
            {
                gamePage.SetScore(message);
            }

            if (scorePage != null)
            {
                scorePage.InfoText = message;
            }
        }

        public async void ReadMessage()
        {
            try
            {
                uint bytesRead = await dataReader.LoadAsync(sizeof(uint));
                if (bytesRead > 0)
                {
                    // Determine how long the string is.
                    uint messageLength = dataReader.ReadUInt32();
                    bytesRead = await dataReader.LoadAsync(messageLength);
                    if (bytesRead > 0)
                    {
                        // Decode the string.
                        currentMessage = dataReader.ReadString(messageLength);
                        int i = ProcessMessage();
                        if (i != (int)Message.Closing && i != (int)Message.Complete)
                        {
                            ReadMessage();
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (err is ObjectDisposedException || err.GetType() == typeof(System.Exception))
                {
                    if (PeerFinder.Role == PeerRole.Client)
                    {
                        if (gamePage != null)
                        {
                            PrintMessage("The host has left the game");
                            gamePage.NavigateToMainPage();
                        }
                    }
                    else
                    {
                        if (scorePage != null)
                        {
                            PrintMessage("A player has left the game");
                            scorePage.CloseSocketHost();
                        }
                    }
                }
                else
                {
                    PrintMessage(err.ToString());
                }
            }
        }

        private int ProcessMessage()
        {
            if (currentMessage != null)
            {
                Message message;
                if (!Enum.TryParse(currentMessage, out message))
                {
                   return ParseMessage();
                }
                else
                {
                    switch (message)
                    {
                        case Message.NavigateToGame:
                            if (PeerFinder.Role == PeerRole.Client)
                            {
                                PrintMessage(currentMessage);
                                if (waitForHost != null)
                                {
                                    waitForHost.NavigateToGame();
                                }
                            }
                            break;

                        case Message.Start:
                            if (gamePage != null)
                            {
                                gamePage.StartGame();
                            }
                            break;

                        case Message.Navigated:
                            try
                            {
                                if (scorePage != null)
                                {
                                    scorePage.SendScores();
                                }
                            }
                            catch (NullReferenceException)
                            {
                                PrintMessage("Error sending scores");
                            }
                            catch (Exception)
                            {
                            }
                            break;

                        case Message.Ready:
                            if (gamePage != null)
                            {
                                gamePage.TrackReady();
                            }
                            break;

                        case Message.Pause:
                            break;

                        case Message.Done:
                            gamePage.NavigateToScorePage();
                            break;

                        case Message.Closing:
                            if (scorePage != null)
                            {
                                scorePage.CloseSocketHost();
                            }
                            break;

                        case Message.Complete:
                            if (scorePage != null)
                            {
                                scorePage.CloseSocket();
                            }
                            break;

                        default:
                            throw new ArgumentException("Unknown message");
                    }
                    return (int)message;
                }
            }
            return -1;
        }

        private int ParseMessage()
        {
            string[] words = currentMessage.Split(' ');

            switch (words[0])
            {
                case Constants.OpCodeParams:
                    if (gamePage != null)
                    {
                        gamePage.SetParams(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]));
                    }
                    break;

                case Constants.OpCodeCritterBorn:
                    if (gamePage != null)
                    {
                        gamePage.SpawnCritter(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]), int.Parse(words[4]));
                    }
                    break;

                case Constants.OpCodeUpdateScore:
                    if (scorePage != null)
                    {
                        // If displayName has a ' ' in it, concatenate the strings together
                        fullDisplayName = String.Join(" ", words, 1, words.Length - 2);
                        scorePage.CompileScores(fullDisplayName, words[words.Length-1]);
                    }
                    break;

                case Constants.OpCodeCritterStomp:
                    if (gamePage != null)
                    {
                        gamePage.UpdateBoard(int.Parse(words[1]), int.Parse(words[2]), int.Parse(words[3]));
                    }
                    break;

                case Constants.OpCodeSendScore:
                    if (scorePage != null)
                    {
                        // If displayName has a ' ' in it, concatenate the strings together
                        fullDisplayName = String.Join(" ", words, 1, words.Length-2);
                        scorePage.ReceiveScores(fullDisplayName, words[words.Length - 1]);
                    }
                    break;

                case Constants.OpCodeUpdateClientTime:
                    if (gamePage != null)
                    {
                        gamePage.UpdateClientTime(words[1]);
                    }
                    break;

                case Constants.OpCodeNumPlayers:
                    if (scorePage != null)
                    {
                        scorePage.UpdateNumPlayers(int.Parse(words[1]));
                    }
                    break;

                case Constants.OpCodeSendDisplayName:
                    fullDisplayName = String.Join(" ", words, 1, words.Length-1);

                    if (invitePlayerPage != null)
                    {
                        invitePlayerPage.AddConnectedPeer(fullDisplayName);
                    }
                    else if (waitForHost != null)
                    {
                        waitForHost.addConnectedPeer(fullDisplayName);
                    }
                    break;
            }
            return int.Parse(words[0]);
        }
    }
}
