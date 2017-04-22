using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;

namespace BlackjackLibrary
{
    class BlackjackService
    {
        private IPAddress ipAddress;
        private int port;
        private EvtLogWriter LogWriter = new EvtLogWriter("BlackJackGame", "Application"); //Allows to write to Windows Event Logs
        private int connectedClients;

        public event EventHandler TooManyClients; // Evento disparado al exceder la cantidad de clientes aceptados

        virtual protected void OnTooManyClients()
        {
            if (TooManyClients != null)
                TooManyClients(this, EventArgs.Empty);
        }

        public BlackjackService(int port)
        {
            this.connectedClients = 0;
            // set up port and determine IP Address
            this.port = port;
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);

            //this.ipAddress = IPAddress.Any; // allows requests to any of server's  addresses

            this.ipAddress = IPAddress.Any;
            /*for (int i = 0; i < ipHostInfo.AddressList.Length; ++i)
            {
                if (ipHostInfo.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    this.ipAddress = ipHostInfo.AddressList[i];
                    break;
                }
            }*/
            if (this.ipAddress == null)
                throw new Exception("No IPv4 address for server");

        }

        //public async Task Run()
        public async Task Run()
        {
            TcpListener listener = new TcpListener(this.ipAddress, this.port);
            listener.Start();
            LogWriter.writeInfo("Blackjack Service is running on port" + this.port.ToString());

            while (true)
            {
                try
                {
                    if (this.connectedClients < 2)
                    {
                        TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                        this.connectedClients += 1;
                        await Process(tcpClient);

                    }
                    else
                    {
                        TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                        await Deny(tcpClient);
                        OnTooManyClients();                     
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        } // Start

        private async Task Deny(TcpClient tcpClient)
        {
            string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
            LogWriter.writeWarning("Denied connection request from " + clientEndPoint + Environment.NewLine +
                "Too many clients connected");
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(networkStream);
                StreamWriter writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;
                await writer.WriteLineAsync("code=ERR&desc=Denied");
            }
            catch (Exception ex)
            {
                LogWriter.writeError("Error in Deny " + ex.Message);
            }
            finally
            {
                if (tcpClient.Connected)
                    tcpClient.Close();
            }
        }

        private async Task Process(TcpClient tcpClient)
        {
            string clientEndPoint = tcpClient.Client.RemoteEndPoint.ToString();
            LogWriter.writeInfo("Received connection request from " + clientEndPoint);
            try
            {
                NetworkStream networkStream = tcpClient.GetStream();
                StreamReader reader = new StreamReader(networkStream);
                StreamWriter writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;
                while (true)
                {
                    string request = await reader.ReadLineAsync();
                    if (request != null)
                    {
                        LogWriter.writeInfo("Received client request: " + request);
                        string response = Response(request);
                        LogWriter.writeInfo("Server responded with " + response);
                        await writer.WriteLineAsync(response);
                    }
                    else
                    {
                        LogWriter.writeWarning("Client disconnected " + clientEndPoint);
                        break; // client closed connection
                    }
                }
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                LogWriter.writeError("Error in Process " + ex.Message);
                if (tcpClient.Connected)
                    tcpClient.Close();
            }
        } // Process

        private static string Response(string request)
        {
            return "";
        }
    }
}
