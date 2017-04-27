using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlackjackLibrary;

namespace ConsoleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            GameServer BlackJackServer;
            EvtLogWriter LogWriter = new EvtLogWriter("BlackJackGame", "Application"); //Allows to write to Windows Event Logs
            BlackJackServer = new GameServer();
            
            BlackJackServer.CardDealed += BlackJackServer_CardDealed;
            BlackJackServer.ClientDisconnected += BlackJackServer_ClientDisconnected;
            BlackJackServer.PlayerOneConnected += BlackJackServer_PlayerOneConnected;
            BlackJackServer.PlayerTwoConnected += BlackJackServer_PlayerTwoConnected;
            BlackJackServer.TooManyClients += BlackJackServer_TooManyClients;
            BlackJackServer.PlayerOneWins += BlackJackServer_PlayerOneWins;
            BlackJackServer.PlayerTwoWins += BlackJackServer_PlayerTwoWins;
            BlackJackServer.GameTied += BlackJackServer_GameTied;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Servidor Iniciado...");
            BlackJackServer.Initialize();            
        }

        private static void BlackJackServer_GameTied(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Juego empatado.");            
        }

        private static void BlackJackServer_PlayerTwoWins(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Jugador 2 gana el juego.");
        }

        private static void BlackJackServer_PlayerOneWins(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Jugador 1 gana el juego.");
        }

        private static void BlackJackServer_TooManyClients(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Se intentó conectar un tercer jugador. Se rechazó la solicitud.");
        }

        private static void BlackJackServer_PlayerTwoConnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Jugador 2 conectado al servidor!");
        }

        private static void BlackJackServer_PlayerOneConnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Jugador 1 conectado al servidor!");
        }

        private static void BlackJackServer_ClientDisconnected(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Cliente desconectado del servidor!");
        }

        private static void BlackJackServer_CardDealed(object sender, GameMessageEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Se jugó una carta para el jugador ");
            Console.Write(e.GM.PlayerNumber);
            Console.Write(". La carta fue: ");
            Console.WriteLine(e.GM.PlayedCard.Value + " of " + e.GM.PlayedCard.Suit);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Valor actual del deck para el jugador = " + e.GM.DeckValue.ToString());            
        }        
    }
}
