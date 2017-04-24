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

        // Events
        public event EventHandler Connected; //Evento lanzado al conectarse al servidor
        public event MessageReceivedEventHandler MessageReceived; //Evento lanzado al enviar un mensaje
        public event EventHandler Disconnected; //Evento lanzado al desconectarse del servidor
        public event EventHandler ServerError; //Evento lanzado al obtener un mensaje de error desde el servidor
        public event EventHandler NoMessages; //Evento lanzado al no obtener mensajes de respuesta por tipo de mensaje
        
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

        virtual protected void OnNoMessages()
        {
            if (NoMessages != null)
                NoMessages(this, EventArgs.Empty);
        }

        virtual protected void OnMessageReceived(GameMessageEventArgs e)
        {
            if (MessageReceived != null)
                MessageReceived(this, e);
        }

        public GameClient()
        {
            _isConnected = false;
            connect();
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
        private void connect()
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
                Console.WriteLine("Error al establecer conexión con el servidor");
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
                Console.WriteLine("GameClient: Error connecting to Server");
                OnServerError();
            }

            try
            {
                //Lo primero que se hace es esperar por un mensaje de control Hello del servidor
                Message hello = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());

                if (hello == Message.Hello)
                {
                    //Recibimos un Hello, procedemos a responder con un hello.
                    netDataWriter.Write((Byte)Message.Hello);
                    netDataWriter.Flush();

                    //Esperamos el ACK del servidor
                    Message srvAns = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());
                    if (srvAns == Message.Ack)
                    {
                        //El servidor nos respondió el ACK. Podemos iniciar a enviar mensajes.
                        OnConnected(); //Disparamos el evento de conexión exitosa.
                    }
                    else
                    {
                        Disconnect();
                        Console.WriteLine("Error al establecer conexión con el servidor");
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("NetClient Manager: Error connecting to Server");
                OnServerError();
            }
        }

        /// <summary>
        /// Envía un mensaje al servidor para que se almacene en la BD
        /// </summary>
        /// <param name="clientMsg">Mensaje a enviar al servidor</param>
        public void SendMessage(Message clientMsg)
        {
            try
            {
                if (_isConnected)
                {
                    netDataWriter.Write((Byte)(clientMsg));
                    netDataWriter.Flush();

                    Message srvAns = (Message)Enum.Parse(typeof(Message), netDataReader.ReadByte().ToString());

                    if (srvAns == Message.Deal)
                    {
                        GameMessageEventArgs mEa = new GameMessageEventArgs(new GameMessage());
                        OnMessageReceived(mEa); //Lanzamos el evento de carta recibida
                    }
                    if (srvAns == Message.Error)
                    {
                        OnServerError(); //Lanzamos el evento de error en el servidor
                    }
                    /*Faltan las demas interacciones*/
                }
            }
            catch
            {
                Console.WriteLine("Error en envío de mensajes");
            }
        }
    }
}
