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
                                        playerOne.Play(gameDealer.Deal());                                        
                                    }
                                    lock (LogWriter)
                                    {
                                        LogWriter.writeInfo("Dealt a card to player one");
                                    }
                                    break;
                                case 2:
                                    lock (gameDealer)
                                    {
                                        playerTwo.Play(gameDealer.Deal());
                                    }
                                    lock (LogWriter)
                                    {
                                        LogWriter.writeInfo("Dealt a card to player two");
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
                                    playerOne.Status = PlayerStatus.Stay;
                                    //OnPlayerStatusChange
                                    break;
                                case 2:
                                    playerTwo.Status = PlayerStatus.Stay;
                                    //OnPlayerStatusChange
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                    int resultado = this.CheckGameStatus();
                    switch (resultado)
                    {
                        case 0:
                            //Empate
                            //Comunicar por red a cada cliente
                            break;
                        case 1:
                            //Jugador 1 gana
                            //Comunicar por red a cada cliente
                            break;
                        case 2:
                            //Jugador 2 gana
                            //Comunicar por red a cada cliente
                            break;
                        default:
                            //Juego continua;
                            continue;
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

        /*public void SendMessage(Message clientMessage, int player)
        {
            NetworkStream netStream;       // Stream del canal de respuesta para enviar datos hacia el cliente.
            //BinaryReader netDataReader;    // Utilizado para leer datos del canal de comunicación
            BinaryWriter netDataWriter;    // Utilizado para escribir datos en el canal de comunicación
            switch (player)
            {
                case 1:
                    try
                    {
                        if (playerOne.Channel.Connected)
                        {
                            netStream = playerOne.Channel.GetStream(); //Obtenemos el canal de comunicación
                            //netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                            netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);
                            netDataWriter.Write((Byte)(clientMessage));
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
                            LogWriter.writeError("Error in SendMessage" + Environment.NewLine + ex.Message);
                        }                        
                    }
                    break;
                case 2:
                    try
                    {
                        if (playerTwo.Channel.Connected)
                        {
                            netStream = playerTwo.Channel.GetStream(); //Obtenemos el canal de comunicación
                            //netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                            netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);
                            netDataWriter.Write((Byte)(clientMessage));
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
                            LogWriter.writeError("Error in SendMessage" + Environment.NewLine + ex.Message);
                        }
                    }
                    break;
                default:
                    break;
            }
        }*/

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
        /// y determina si hay que enviar mensajes a los clientes
        /// </summary>
        /// <returns>El numero del jugador que gana, 
        /// 0 si hay empate y -1 si el juego continua
        /// </returns>
        private int CheckGameStatus()
        {
            lock (this)
            {
                switch (playerOne.Status)
                {
                    case PlayerStatus.FiveCards:
                        break;
                    case PlayerStatus.BlackJack:
                        break;
                    case PlayerStatus.TwentyOne:
                        break;
                    case PlayerStatus.Stay:
                        break;
                    case PlayerStatus.Lost:
                        break;
                    case PlayerStatus.Playing:
                        break;
                    default:
                        break;
                }
                return 0;
            }
        }
    }
}
