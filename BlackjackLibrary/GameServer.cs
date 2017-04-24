using System;
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

        virtual protected void OnTooManyClients()
        {
            if (TooManyClients != null)
                TooManyClients(this, EventArgs.Empty);
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
                    this.connectedClients += 1;
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
                        InteractWithClient(ref commChannel, ref netDataReader, ref netDataWriter);
                        lock (LogWriter)
                        {
                            LogWriter.writeInfo("Player #" + playerNum.ToString() + " Connected Successfully. Ready to play"); 
                        }
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
                                        GameMessage gm = new GameMessage(dCard, Message.Deal, 1);
                                        SendMessage(gm, gm.PlayerNumber);
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
                                        GameMessage gm = new GameMessage(dCard, Message.Deal, 2);
                                        SendMessage(gm, gm.PlayerNumber);
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
                                        playerOne.Status = PlayerStatus.Stay; 
                                    }
                                    break;
                                case 2:
                                    lock (playerTwo)
                                    {
                                        playerTwo.Status = PlayerStatus.Stay; 
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
                            GameMessage gm1Tie = new GameMessage(new Card(), Message.Tie, 1);
                            SendMessage(gm1Tie, gm1Tie.PlayerNumber);
                            GameMessage gm2Tie = new GameMessage(new Card(), Message.Tie, 2);
                            SendMessage(gm2Tie, gm2Tie.PlayerNumber);
                            Finish();
                            break;
                        case GameResult.PlayerOneWins:
                            GameMessage gm1Win1 = new GameMessage(new Card(), Message.PlayerWins, 1);
                            SendMessage(gm1Win1, gm1Win1.PlayerNumber);
                            GameMessage gm2Win1 = new GameMessage(new Card(), Message.PlayerLooses, 2);
                            SendMessage(gm2Win1, gm2Win1.PlayerNumber);
                            Finish();
                            break;
                        case GameResult.PlayerTwoWins:
                            GameMessage gm1Win2 = new GameMessage(new Card(), Message.PlayerLooses, 1);
                            SendMessage(gm1Win2, gm1Win2.PlayerNumber);
                            GameMessage gm2Win2 = new GameMessage(new Card(), Message.PlayerWins, 2);
                            SendMessage(gm2Win2, gm2Win2.PlayerNumber);
                            Finish();
                            break;
                        case GameResult.Continue:
                            GameMessage gm1Cont = new GameMessage(new Card(), Message.Ready, 1);
                            SendMessage(gm1Cont, gm1Cont.PlayerNumber);
                            GameMessage gm2Cont = new GameMessage(new Card(), Message.Ready, 2);
                            SendMessage(gm2Cont, gm2Cont.PlayerNumber);
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
                switch (playerOne.Status)
                {
                    case PlayerStatus.FiveCards:
                        if (playerTwo.Status == PlayerStatus.FiveCards)
                        {
                            return GameResult.Tie;
                        }
                        if (playerTwo.Status == PlayerStatus.Playing)
                        {
                            return GameResult.Continue;
                        }
                        else
                        {
                            return GameResult.PlayerOneWins;
                        }
                    case PlayerStatus.BlackJack:
                        if (playerTwo.Status == PlayerStatus.FiveCards)
                        {
                            return GameResult.PlayerTwoWins;
                        }
                        if (playerTwo.Status == PlayerStatus.BlackJack)
                        {
                            return GameResult.Tie;
                        }
                        if (playerTwo.Status == PlayerStatus.Playing)
                        {
                            return GameResult.Continue;
                        }
                        else
                        {
                            return GameResult.PlayerOneWins;
                        }
                    case PlayerStatus.TwentyOne:
                        if (playerTwo.Status == PlayerStatus.FiveCards)
                        {
                            return GameResult.PlayerTwoWins;
                        }
                        if (playerTwo.Status == PlayerStatus.BlackJack)
                        {
                            return GameResult.PlayerTwoWins;
                        }
                        if (playerTwo.Status == PlayerStatus.TwentyOne)
                        {
                            return GameResult.Tie;
                        }
                        if (playerTwo.Status == PlayerStatus.Playing)
                        {
                            return GameResult.Continue;
                        }
                        else
                        {
                            return GameResult.PlayerOneWins;
                        }
                    case PlayerStatus.Stay:
                        if (playerTwo.Status == PlayerStatus.FiveCards)
                        {
                            return GameResult.PlayerTwoWins;
                        }
                        if (playerTwo.Status == PlayerStatus.BlackJack)
                        {
                            return GameResult.PlayerTwoWins;
                        }
                        if (playerTwo.Status == PlayerStatus.TwentyOne)
                        {
                            return GameResult.PlayerTwoWins;
                        }
                        if (playerTwo.Status == PlayerStatus.Stay)
                        {
                            if (playerOne.Total > playerTwo.Total)
                            {
                                return GameResult.PlayerOneWins;
                            }
                            else
                            {
                                return GameResult.PlayerTwoWins;
                            }
                        }
                        if (playerTwo.Status == PlayerStatus.Playing)
                        {
                            return GameResult.Continue;
                        }
                        else
                        {
                            return GameResult.PlayerOneWins;
                        }
                    case PlayerStatus.Lost:
                        if (playerTwo.Status == PlayerStatus.Lost)
                        {
                            return GameResult.Tie;
                        }
                        else
                        {
                            return GameResult.PlayerTwoWins;
                        }                        
                    case PlayerStatus.Playing:
                        return GameResult.Continue;                        
                    default:
                        return GameResult.Continue;
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
