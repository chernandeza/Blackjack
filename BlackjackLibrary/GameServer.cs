﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace BlackjackLibrary
{
    public class GameServer
    {
        private IPAddress ip = IPAddress.Parse("127.0.0.1");    //Localhost IP
        private int port = 10830;       //Server Port
        public bool running;     //Is server running?
        private TcpListener listener;     //TCPListener object to listen for connections
        private static EvtLogWriter LogWriter = new EvtLogWriter("BlackJackGame", "Application"); //Allows to write to Windows Event Logs
        private byte connectedClients;
        private Player playerOne;
        private Player playerTwo;
        public Dealer gameDealer;
                
        public event EventHandler PlayerOneConnected; // Evento se dispara cuando se conecta un cliente
        public event EventHandler PlayerTwoConnected; // Evento se dispara cuando se conecta un cliente
        public event EventHandler ClientDisconnected; //Evento se dispara cuando se desconecta un cliente.
        public event EventHandler TooManyClients; // Evento disparado al exceder la cantidad de clientes aceptados
        public event MessageReceivedEventHandler CardDealed; //Evento lanzado al repartir una carta
        public event EventHandler PlayerOneWins; // Evento se dispara cuando gana el jugador 1
        public event EventHandler PlayerTwoWins; // Evento se dispara cuando gana el jugador 2
        public event EventHandler GameTied; // Evento se dispara cuando el juego se empata



        /*Estos métodos validan que la suscripción a los eventos no esté vacía. Si está vacía, no lanza el evento de forma innecesaria*/
        virtual protected void OnClientDisconnected()
        {
            if (ClientDisconnected != null)
                ClientDisconnected(this, EventArgs.Empty);
        }

        virtual protected void OnP1Connected()
        {
            if (PlayerOneConnected != null)
                PlayerOneConnected(this, EventArgs.Empty);
        }

        virtual protected void OnP2Connected()
        {
            if (PlayerTwoConnected != null)
                PlayerTwoConnected(this, EventArgs.Empty);
        }

        virtual protected void OnP1Wins()
        {
            if (PlayerOneWins != null)
                PlayerOneWins(this, EventArgs.Empty);
        }

        virtual protected void OnP2Wins()
        {
            if (PlayerTwoWins != null)
                PlayerTwoWins(this, EventArgs.Empty);
        }

        virtual protected void OnGameTied()
        {
            if (GameTied != null)
                GameTied(this, EventArgs.Empty);
        }

        virtual protected void OnTooManyClients()
        {
            if (TooManyClients != null)
                TooManyClients(this, EventArgs.Empty);
        }

        virtual protected void OnCardDealed(GameMessageEventArgs e)
        {
            if (CardDealed != null)
                CardDealed(this, e);
        }

        public GameServer()
        {
            this.running = false;
            this.connectedClients = 0;
            gameDealer = new Dealer();
            playerOne = new Player();
            playerTwo = new Player();
        }

        /// <summary>
        /// Initialize the network server. Listens for connections.
        /// </summary>
        public void Initialize()
        {
            listener = new TcpListener(ip, port);
            listener.Start();
            this.running = true;
            lock (LogWriter)
            {
                LogWriter.writeInfo("GameServer Initialized. Waiting for players");
            }
            Listen();
        }

        private void Listen()
        {
            while (running)
            {
                if (this.connectedClients < 2)
                {
                    TcpClient tcpClient = listener.AcceptTcpClient();  // Acepta una conexión entrante
                    lock (this)
                    {
                        this.connectedClients += 1;
                    }                    
                    if (this.connectedClients == 1)
                    {
                        playerOne.Channel = tcpClient;
                        OnP1Connected();
                        //Crear una nueva tarea para cada uno de los clientes.
                        new Task(() => ConnectionSetup(playerOne.Channel, 1)).Start();
                    }
                    else
                    {
                        playerTwo.Channel = tcpClient;
                        OnP2Connected();
                        //Crear una nueva tarea para cada uno de los clientes.
                        new Task(() => ConnectionSetup(playerTwo.Channel, 2)).Start();
                    }                    
                }
                else
                {
                    if(this.connectedClients != 2)
                        OnTooManyClients();
                }
            }
        }

        private void ConnectionSetup(TcpClient tcpC, int playerNum)
        {
            TcpClient commChannel;                   // Canal de comunicación hacia el servidor
            NetworkStream netStream;                 // Stream del canal de respuesta para enviar datos hacia el servidor.
            BinaryReader netDataReader;              // Utilizado para leer datos del canal de comunicación
            BinaryWriter netDataWriter;              // Utilizado para escribir datos en el canal de comunicación
            commChannel = tcpC;
            string clientEndPoint = commChannel.Client.RemoteEndPoint.ToString();
            lock (LogWriter)
            {
                LogWriter.writeInfo("Received connection request from " + clientEndPoint);
            }
            try
            {
                netStream = commChannel.GetStream(); //Obtenemos el canal de comunicación
                netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);

                //Enviamos un "HELLO" al cliente y esperamos respuesta
                netDataWriter.Write((Byte)(Message.Hello));
                netDataWriter.Write(playerNum);
                netDataWriter.Flush();

                //Esperamos un "Ready" proveniente del cliente
                Message clientMsg = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());
                int gamePlayer = netDataReader.ReadInt32();
                if (clientMsg == Message.Ready)
                {
                    // Se recibió un mensaje de "Ready". 
                    if (playerNum == gamePlayer)
                    {
                        // El cliente ya conoce su número de jugador. Se inicia la interacción con el cliente.
                        // Enviamos un mensaje de Acknowledge y luego iniciamos la interacción.
                        netDataWriter.Write((Byte)(Message.Ack));
                        netDataWriter.Flush();
                        lock (LogWriter)
                        {
                            LogWriter.writeInfo("Player #" + playerNum.ToString() + " Connected Successfully. Ready to play");
                        }
                        while (this.connectedClients < 2) { } //Evita recibir mensajes hasta que hayan 2 jugadores
                        InteractWithClient(ref commChannel, ref netDataReader, ref netDataWriter);                        
                    }
                }
                else
                {
                    throw new Exception("Server Unresponsive");
                }
            }
            catch (Exception ex)
            {
                lock (LogWriter)
                {
                    LogWriter.writeError("Error in ConnectionSetup" + Environment.NewLine + ex.Message); 
                }
            }
        }

        private void InteractWithClient(ref TcpClient commChannel, ref BinaryReader netDataReader, ref BinaryWriter netDataWriter)  // Receive all incoming packets.
        {
            try
            {
                while (commChannel.Client.Connected)  // While we are connected.
                {
                    Message ctrlMsg = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());
                    int playerNum = 0;
                    switch (ctrlMsg)
                    {
                        case Message.Ack:
                            break;
                        case Message.Ready:
                            break;
                        case Message.Deal:
                            playerNum = netDataReader.ReadInt32();
                            switch (playerNum)
                            {
                                case 1:
                                    lock (gameDealer)
                                    {
                                        Card dCard = gameDealer.Deal();
                                        playerOne.Play(dCard);
                                        //Enviar mensaje a jugador 1
                                        GameMessage gm = new GameMessage(dCard, Message.Deal, 1, playerOne.Total);
                                        SendMessage(gm, gm.PlayerNumber);
                                        GameMessageEventArgs mEa = new GameMessageEventArgs(gm);
                                        OnCardDealed(mEa);
                                        lock (LogWriter)
                                        {
                                            LogWriter.writeInfo("Dealt a card to player one: " + dCard.Value.ToString() + " of " + dCard.Suit.ToString());
                                        }
                                    }                                    
                                    break;
                                case 2:
                                    lock (gameDealer)
                                    {
                                        Card dCard = gameDealer.Deal();
                                        playerTwo.Play(dCard);
                                        //Enviar mensaje a jugador 2
                                        GameMessage gm = new GameMessage(dCard, Message.Deal, 2, playerTwo.Total);
                                        SendMessage(gm, gm.PlayerNumber);
                                        GameMessageEventArgs mEa = new GameMessageEventArgs(gm);
                                        OnCardDealed(mEa);
                                        lock (LogWriter)
                                        {
                                            LogWriter.writeInfo("Dealt a card to player two: " + dCard.Value.ToString() + " of " + dCard.Suit.ToString());
                                        }
                                    }                                    
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case Message.Stay:
                            playerNum = netDataReader.ReadInt32();
                            switch (playerNum)
                            {
                                case 1:
                                    lock (playerOne)
                                    {
                                        playerOne.IsPlaying = false;
                                        if (playerOne.Status != PlayerStatus.Lost)
                                            playerOne.Status = PlayerStatus.Stay;
                                        lock (LogWriter)
                                        {
                                            LogWriter.writeInfo("Player one Stays.");
                                        }
                                    }
                                    break;
                                case 2:
                                    lock (playerTwo)
                                    {
                                        playerTwo.IsPlaying = false;
                                        if (playerTwo.Status != PlayerStatus.Lost)
                                            playerTwo.Status = PlayerStatus.Stay;
                                        lock (LogWriter)
                                        {
                                            LogWriter.writeInfo("Player two Stays.");
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    GameResult resultado = this.CheckGameStatus();
                    switch (resultado)
                    {
                        case GameResult.Tie: //Empate
                            GameMessage gm1Tie = new GameMessage(new Card(), Message.Tie, 1, playerOne.Total);
                            SendMessage(gm1Tie, gm1Tie.PlayerNumber);
                            GameMessage gm2Tie = new GameMessage(new Card(), Message.Tie, 2, playerTwo.Total);
                            SendMessage(gm2Tie, gm2Tie.PlayerNumber);
                            //Finish();
                            break;
                        case GameResult.PlayerOneWins:
                            GameMessage gm1Win1 = new GameMessage(new Card(), Message.PlayerWins, 1, playerOne.Total);
                            SendMessage(gm1Win1, gm1Win1.PlayerNumber);
                            GameMessage gm2Win1 = new GameMessage(new Card(), Message.PlayerLooses, 2, playerTwo.Total);
                            SendMessage(gm2Win1, gm2Win1.PlayerNumber);
                            //Finish();
                            break;
                        case GameResult.PlayerTwoWins:
                            GameMessage gm1Win2 = new GameMessage(new Card(), Message.PlayerLooses, 1, playerOne.Total);
                            SendMessage(gm1Win2, gm1Win2.PlayerNumber);
                            GameMessage gm2Win2 = new GameMessage(new Card(), Message.PlayerWins, 2, playerTwo.Total);
                            SendMessage(gm2Win2, gm2Win2.PlayerNumber);
                            //Finish();
                            break;
                        case GameResult.Continue:
                            switch (playerNum)
                            {
                                case 1:
                                    GameMessage gm1Cont = new GameMessage(new Card(), Message.Ready, 1, playerOne.Total);
                                    SendMessage(gm1Cont, gm1Cont.PlayerNumber);
                                    break;
                                case 2:
                                    GameMessage gm2Cont = new GameMessage(new Card(), Message.Ready, 2, playerTwo.Total);
                                    SendMessage(gm2Cont, gm2Cont.PlayerNumber);
                                    break;
                                default:
                                    break;
                            }                            
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                netDataWriter.Write((Byte)Message.Error);
                netDataWriter.Flush();
                LogWriter.writeError("Error interacting with client" + Environment.NewLine + ex.Message);
            }
        }

        public void SendMessage(GameMessage clientMessage, int player)
        {
            NetworkStream netStream;       // Stream del canal de respuesta para enviar datos hacia el cliente.
            //BinaryReader netDataReader;    // Utilizado para leer datos del canal de comunicación
            BinaryWriter netDataWriter;    // Utilizado para escribir datos en el canal de comunicación
            switch (player)
            {
                case 1:
                    lock (playerOne)
                    {
                        try
                        {
                            if (playerOne.Channel.Connected)
                            {
                                netStream = playerOne.Channel.GetStream(); //Obtenemos el canal de comunicación
                                                                           //netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                                netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);
                                netDataWriter.Write((Byte)(clientMessage.Message));
                                netDataWriter.Write(ObjSerializer.ObjectToByteArray(clientMessage).Length);
                                netDataWriter.Write(ObjSerializer.ObjectToByteArray(clientMessage));
                                netDataWriter.Flush();
                            }
                            else
                            {
                                throw new SocketException();
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (LogWriter)
                            {
                                LogWriter.writeError("Error in SendMessage to player one" + Environment.NewLine + ex.Message);
                            }
                        } 
                    }
                    break;
                case 2:
                    lock (playerTwo)
                    {
                        try
                        {
                            if (playerTwo.Channel.Connected)
                            {
                                netStream = playerTwo.Channel.GetStream(); //Obtenemos el canal de comunicación
                                                                           //netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                                netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);
                                netDataWriter.Write((Byte)(clientMessage.Message));
                                netDataWriter.Write(ObjSerializer.ObjectToByteArray(clientMessage).Length);
                                netDataWriter.Write(ObjSerializer.ObjectToByteArray(clientMessage));
                                netDataWriter.Flush();
                            }
                            else
                            {
                                throw new SocketException();
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (LogWriter)
                            {
                                LogWriter.writeError("Error in SendMessage to player two" + Environment.NewLine + ex.Message);
                            }
                        } 
                    }
                    break;
                default:
                    break;
            }
        }

        public void Finish()
        {
            try
            {
                playerOne.Channel.Close();
                playerTwo.Channel.Close();
                this.listener.Stop();
                lock (LogWriter)
                {
                    LogWriter.writeInfo("NetworkServer.Finish(): All connections manually closed"); 
                }
            }
            catch (Exception ex)
            {
                lock (LogWriter)
                {
                    LogWriter.writeError("Error in NetworkServer.Finish()" + Environment.NewLine + ex.Message); 
                }
            }
        }

        /// <summary>
        /// Este metodo revisa el estado actual del juego 
        /// y determina si hay gane, empate o el juego continua
        /// </summary>
        /// <returns>Estado actual del juego</returns>
        private GameResult CheckGameStatus()
        {
            lock (this)
            {
                if (!playerOne.IsPlaying && !playerTwo.IsPlaying)
                {
                    switch (playerOne.Status)
                    {
                        case PlayerStatus.FiveCards:
                            if (playerTwo.Status == PlayerStatus.FiveCards)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Tie. Both Five Cards.");
                                }
                                OnGameTied();
                                return GameResult.Tie;
                            }
                            else
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: One Wins. Five Cards.");
                                }
                                OnP1Wins();
                                return GameResult.PlayerOneWins;
                            }
                        case PlayerStatus.BlackJack:
                            if (playerTwo.Status == PlayerStatus.FiveCards)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. Five Cards.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;
                            }
                            if (playerTwo.Status == PlayerStatus.BlackJack)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Tie. Both BlackJack.");
                                }
                                OnGameTied();
                                return GameResult.Tie;
                            }
                            else
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: One Wins. BlackJack");
                                }
                                OnP1Wins();
                                return GameResult.PlayerOneWins;
                            }
                        case PlayerStatus.TwentyOne:
                            if (playerTwo.Status == PlayerStatus.FiveCards)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. Five Cards.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;
                            }
                            if (playerTwo.Status == PlayerStatus.BlackJack)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. BlackJack.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;
                            }
                            if (playerTwo.Status == PlayerStatus.TwentyOne)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Tie");
                                }
                                OnGameTied();
                                return GameResult.Tie;
                            }
                            else
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: One Wins. 21.");
                                }
                                OnP1Wins();
                                return GameResult.PlayerOneWins;
                            }
                        case PlayerStatus.Stay:
                            if (playerTwo.Status == PlayerStatus.FiveCards)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. Five Cards.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;
                            }
                            if (playerTwo.Status == PlayerStatus.BlackJack)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. BlackJack.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;
                            }
                            if (playerTwo.Status == PlayerStatus.TwentyOne)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. 21.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;
                            }
                            if (playerTwo.Status == PlayerStatus.Stay)
                            {
                                if (playerOne.Total > playerTwo.Total)
                                {
                                    lock (LogWriter)
                                    {
                                        LogWriter.writeInfo("Result: One Wins. Closer to 21.");
                                    }
                                    OnP1Wins();
                                    return GameResult.PlayerOneWins;
                                }
                                else
                                {
                                    if (playerOne.Total == playerTwo.Total)
                                    {
                                        lock (LogWriter)
                                        {
                                            LogWriter.writeInfo("Result: Tie");
                                        }
                                        OnGameTied();
                                        return GameResult.Tie;
                                    }
                                    else
                                    {
                                        lock (LogWriter)
                                        {
                                            LogWriter.writeInfo("Result: Two Wins. Closer to 21.");
                                        }
                                        OnP2Wins();
                                        return GameResult.PlayerTwoWins;
                                    }
                                }
                            }
                            if (playerTwo.Status == PlayerStatus.Lost)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: One Wins. Player 2 lost.");
                                }
                                OnP1Wins();
                                return GameResult.PlayerOneWins;
                            }
                            else
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeError("Result: This should never happen.");
                                }
                                return GameResult.Continue;
                            }                            
                        case PlayerStatus.Lost:
                            if (playerTwo.Status == PlayerStatus.Lost)
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Tie. Both Lost.");
                                }
                                return GameResult.Tie;
                            }
                            else
                            {
                                lock (LogWriter)
                                {
                                    LogWriter.writeInfo("Result: Two Wins. Player 1 Lost.");
                                }
                                OnP2Wins();
                                return GameResult.PlayerTwoWins;                                
                            }                            
                        default:
                            lock (LogWriter)
                            {
                                LogWriter.writeInfo("Result: Default Continue");
                            }
                            return GameResult.Continue;
                    }
                }
                else
                {
                    if (playerOne.Status == PlayerStatus.Lost && playerTwo.Status == PlayerStatus.Lost)
                    {
                        lock (LogWriter)
                        {
                            LogWriter.writeInfo("Result: Tie. Both Lost.");
                        }
                        OnGameTied();
                        return GameResult.Tie;
                    }
                    else
                    {
                        lock (LogWriter)
                        {
                            LogWriter.writeInfo("Result: Continue. Players still playing.");
                        }
                        return GameResult.Continue;
                    }                    
                }
            }
        }
    }    

    public enum GameResult
    {
        Tie,
        PlayerOneWins,
        PlayerTwoWins,
        Continue
    }
}
