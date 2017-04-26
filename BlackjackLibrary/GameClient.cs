using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace BlackjackLibrary
{
    /// <summary>
    /// Esta clase administra la conexión de cada uno de los clientes hacia el servidor.
    /// En cada interfaz GUI de un cliente, debe instanciarse un objeto de esta clase.
    /// </summary>
    public class GameClient
    {
        public String Server { get { return "localhost"; } }  // Address of server. In this case - local IP address.
        public int Port { get { return 10830; } }
        //private Task tcpTask;                           //Subproceso para transmitir/recibir
        private bool _isConnected;                      //Indica si el cliente está o no conectado
        private TcpClient commChannel;                  // Canal de comunicación hacia el servidor
        private NetworkStream netStream;                // Stream del canal de respuesta para enviar datos hacia el servidor.
        private BinaryReader netDataReader;             // Utilizado para leer datos del canal de comunicación
        private BinaryWriter netDataWriter;             // Utilizado para escribir datos en el canal de comunicación
        public int PlayerNumber { get; set; }
        private static EvtLogWriter LogWriter = new EvtLogWriter("BlackJackClient", "Application"); //Allows to write to Windows Event Logs

        // Events
        public event EventHandler Connected; //Evento lanzado al conectarse al servidor
        public event MessageReceivedEventHandler MessageReceived; //Evento lanzado al enviar un mensaje
        public event EventHandler Disconnected; //Evento lanzado al desconectarse del servidor
        public event EventHandler ServerError; //Evento lanzado al obtener un mensaje de error desde el servidor
        public event EventHandler GameTied; //Evento que sucede al empatar un juego
        public event EventHandler PlayerWin; //Evento que se dispara al ganar un juego
        public event EventHandler PlayerLoose; //Evento que se dispara al perder un juego
        public event EventHandler GameContinue; //Evento que sucede cuando el juego puede continuar

        /*Estos métodos validan que la suscripción a los eventos no esté vacía. Si está vacía, no lanza el evento de forma innecesaria*/
        virtual protected void OnDisconnected()
        {
            if (Disconnected != null)
                Disconnected(this, EventArgs.Empty);
        }

        virtual protected void OnServerError()
        {
            if (ServerError != null)
                ServerError(this, EventArgs.Empty);
        }

        virtual protected void OnConnected()
        {
            if (Connected != null)
                Connected(this, EventArgs.Empty);
        }

        virtual protected void OnGameTied()
        {
            if (GameTied != null)
                GameTied(this, EventArgs.Empty);
        }

        virtual protected void OnPlayerWin()
        {
            if (PlayerWin != null)
                PlayerWin(this, EventArgs.Empty);
        }

        virtual protected void OnPlayerLoose()
        {
            if (PlayerLoose != null)
                PlayerLoose(this, EventArgs.Empty);
        }

        virtual protected void OnGameContinue()
        {
            if (GameContinue != null)
                GameContinue(this, EventArgs.Empty);
        }

        virtual protected void OnMessageReceived(GameMessageEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        public GameClient()
        {
            _isConnected = false;            
        }

        public void Disconnect()
        {
            if (_isConnected)
                this.CloseConn();
        }

        private void CloseConn() // Close connection.
        {
            netDataReader.Close();
            netDataWriter.Close();
            netStream.Close();
            commChannel.Close();
            OnDisconnected();
            _isConnected = false;
        }

        // Start connection thread and login or register.
        public void Connect()
        {
            try
            {
                if (!_isConnected)
                {
                    _isConnected = true;
                    //tcpTask = new Task(() => ConnectionSetup());
                    //tcpTask.Start();
                    ConnectionSetup();
                }
            }
            catch (Exception)
            {
                LogWriter.writeError("BlackJack Client: Error connecting to Server");
                OnServerError();
            }
        }

        private void ConnectionSetup()
        {
            try
            {
                commChannel = new TcpClient(Server, Port);  //Connect to server
                netStream = commChannel.GetStream();
                netDataReader = new BinaryReader(netStream, Encoding.UTF8);
                netDataWriter = new BinaryWriter(netStream, Encoding.UTF8);
            }
            catch (Exception)
            {
                LogWriter.writeError("BlackJack Client: Error connecting to Server");
                OnServerError();
            }

            try
            {
                //Lo primero que se hace es esperar por un mensaje de control Hello del servidor
                Message hello = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());

                if (hello == Message.Hello)
                {
                    //Obtenemos el número de jugador
                    this.PlayerNumber = netDataReader.ReadInt32();
                    //Recibimos un Hello, procedemos a responder con un ready y nuestro número de jugador.
                    netDataWriter.Write((Byte)Message.Ready);
                    netDataWriter.Write(this.PlayerNumber);
                    netDataWriter.Flush();

                    //Esperamos el ACK del servidor
                    Message srvAns = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());
                    if (srvAns == Message.Ack)
                    {
                        //El servidor nos respondió el ACK. Podemos iniciar a enviar mensajes.
                        OnConnected(); //Disparamos el evento de conexión exitosa.
                        GameMessageEventArgs g = new GameMessageEventArgs(new GameMessage(new Card(), Message.Ack, this.PlayerNumber));
                        OnMessageReceived(g);
                        Receiver();
                    }
                    else
                    {
                        Disconnect();
                        LogWriter.writeError("BlackJack Client: Error connecting to Server");
                    }
                }
            }
            catch (Exception)
            {
                LogWriter.writeError("BlackJack Client: Error connecting to Server");
                OnServerError();
            }
        }

        /// <summary>
        /// This method only listens and raises events when a message is received
        /// </summary>
        private void Receiver()
        {
            try
            {
                int sizeOfGameMessage = 0;
                GameMessage infoCard = new GameMessage();
                while (_isConnected)
                {
                    Message srvAns = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());

                    switch (srvAns)
                    {
                        case Message.Error:
                            OnServerError(); //Lanzamos el evento de error en el servidor
                            break;
                        case Message.Ready:
                            sizeOfGameMessage = netDataReader.ReadInt32();
                            infoCard = (GameMessage)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfGameMessage));
                            OnGameContinue(); //Evento de continuar el juego
                            break;
                        case Message.Deal:
                            sizeOfGameMessage = netDataReader.ReadInt32();
                            infoCard = (GameMessage)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfGameMessage));
                            GameMessageEventArgs mEa = new GameMessageEventArgs(infoCard);
                            OnMessageReceived(mEa); //Lanzamos el evento de carta recibida                            
                            break; 
                        case Message.Tie:
                            sizeOfGameMessage = netDataReader.ReadInt32();
                            infoCard = (GameMessage)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfGameMessage));
                            OnGameTied(); //Evento de juego empatado
                            break; 
                        case Message.PlayerWins:
                            sizeOfGameMessage = netDataReader.ReadInt32();
                            infoCard = (GameMessage)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfGameMessage));
                            OnPlayerWin(); //Evento de jugador gana
                            break;
                        case Message.PlayerLooses:
                            sizeOfGameMessage = netDataReader.ReadInt32();
                            infoCard = (GameMessage)ObjSerializer.ByteArrayToObject(netDataReader.ReadBytes(sizeOfGameMessage));
                            OnPlayerLoose(); //Evento de jugador pierde
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                lock (LogWriter)
                {
                    LogWriter.writeError("Blackjack Client: Error sending messages: " + Environment.NewLine + e.Message); 
                }
            }
        }

        /// <summary>
        /// This method only sends messages to the server
        /// </summary>
        /// <param name="clientMsg">Mensaje a enviar al servidor</param>
        public void SendMessage(Message clientMsg)
        {
            if (_isConnected)
            {
                try
                {
                    netDataWriter.Write((Byte)(clientMsg));
                    netDataWriter.Write(this.PlayerNumber);
                    netDataWriter.Flush();


                }
                catch (Exception e)
                {
                    LogWriter.writeError("Blackjack Client: Error sending messages: " + Environment.NewLine + e.Message);
                } 
            }
            else
            {
                LogWriter.writeError("Blackjack Client: Se intentó escribir estando desconectado.");
            }
        }
    }
}
