using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BlackjackLibrary;
using System.Threading.Tasks;

namespace BlackJackClient
{
    public partial class BlackJackClient : Form
    {
        Dictionary<String, Image> ImageDictionary;
        GameClient gClient;

        public BlackJackClient()
        {
            InitializeComponent();
            ImageDictionary = new Dictionary<string, Image>();
            gClient = new GameClient();
            gClient.Connected += GClient_Connected;
            gClient.Disconnected += GClient_Disconnected;
            gClient.GameContinue += GClient_GameContinue;
            gClient.GameTied += GClient_GameTied;
            gClient.MessageReceived += GClient_MessageReceived;
            gClient.PlayerLoose += GClient_PlayerLoose;
            gClient.PlayerWin += GClient_PlayerWin;
        }

        private void GClient_PlayerWin(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GClient_PlayerLoose(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GClient_MessageReceived(object sender, GameMessageEventArgs e)
        {
            switch (e.GM.Message)
            {
                case BlackjackLibrary.Message.Error:
                    break;
                case BlackjackLibrary.Message.Ack:
                    this.BeginInvoke(new MethodInvoker(delegate
                    {
                        lblGameNum.Text = "You are the player #" + e.GM.PlayerNumber.ToString();
                        lblGameStatus.Text = "Press the buttons to deal a card or to stay.";
                        btnDeal.Enabled = true;
                        btnStay.Enabled = true;
                    }));
                    break;
                case BlackjackLibrary.Message.Ready:
                    break;
                case BlackjackLibrary.Message.Deal:

                    break;
                case BlackjackLibrary.Message.Stay:
                    break;
                case BlackjackLibrary.Message.Tie:
                    break;
                case BlackjackLibrary.Message.FiveCards:
                    break;
                case BlackjackLibrary.Message.TwentyOne:
                    break;
                case BlackjackLibrary.Message.BlackJack:
                    break;
                case BlackjackLibrary.Message.PlayerWins:
                    break;
                case BlackjackLibrary.Message.PlayerLooses:
                    break;
                default:
                    break;
            }

        }

        private void GClient_GameTied(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GClient_GameContinue(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GClient_Disconnected(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void GClient_Connected(object sender, EventArgs e)
        {
            this.BeginInvoke(new MethodInvoker(delegate
            {
                lblGameStatus.Text = "Connected to server!";
                btnConnect.Enabled = false;
            }));
        }

        private void LoadCardImages()
        {
            try
            {
                string[] imageFileList = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\cardImages", "*.png");
                foreach (string imageFile in imageFileList)
                {
                    Image eachImage = Image.FromFile(imageFile);
                    ImageDictionary.Add(Path.GetFileName(imageFile), eachImage);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error al cargar las imágenes", "Dealer Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BlackJackClient_Load(object sender, EventArgs e)
        {
            LoadCardImages();
            flowLayoutPanelCards.Enabled = false;
            btnDeal.Enabled = false;
            btnStay.Enabled = false;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                Task tGClient = new Task(() => gClient.Connect());
                tGClient.Start();
                lblGameStatus.Text = "Trying to connect to server...";                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting: " + ex.Message, "BlackJack", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
