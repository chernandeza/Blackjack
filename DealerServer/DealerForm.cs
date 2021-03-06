﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using BlackjackLibrary;
using System.Threading.Tasks;

namespace DealerServer
{
    public partial class DealerForm : Form
    {
        Dictionary<String, Image> ImageDictionary;
        GameServer BlackJackServer;
        Deck myDealerDeck; //Eliminar
        private static EvtLogWriter LogWriter = new EvtLogWriter("BlackJackGame", "Application"); //Allows to write to Windows Event Logs
        int nextPlayer;

        public DealerForm()
        {
            ImageDictionary = new Dictionary<string, Image>();
            BlackJackServer = new GameServer();
            BlackJackServer.CardDealed += BlackJackServer_CardDealed;
            BlackJackServer.ClientDisconnected += BlackJackServer_ClientDisconnected;
            BlackJackServer.PlayerOneConnected += BlackJackServer_PlayerOneConnected;
            BlackJackServer.PlayerTwoConnected += BlackJackServer_PlayerTwoConnected;
            BlackJackServer.TooManyClients += BlackJackServer_TooManyClients;            
            nextPlayer = 1;         
            InitializeComponent();
        }

        private void BlackJackServer_TooManyClients(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BlackJackServer_PlayerTwoConnected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BlackJackServer_PlayerOneConnected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BlackJackServer_ClientDisconnected(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void BlackJackServer_CardDealed(object sender, GameMessageEventArgs e)
        {
            //throw new NotImplementedException();
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

        private void btnRecorrer_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to deal a card?", "Dealer Server", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {

            }
        }

        private void DealerForm_Load(object sender, EventArgs e)
        {
            LoadCardImages();
            btnDeal.Enabled = true;
            flowLayoutPlayerOne.Enabled = false;
            dealCardToolStripMenuItem.Enabled = false;
            btnDeal.Enabled = false;
        }

        private void btnDeal_Click(object sender, EventArgs e)
        {
            String imgID = myDealerDeck.CardDeck.Pop().FileID;
            pboxDealtCard.Image = ImageDictionary[imgID];
            PictureBox pb = new PictureBox();
            pb.Image = ImageDictionary[imgID];
            pb.Size = new Size(50, 70);
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            switch (nextPlayer)
            {
                case 1:
                    flowLayoutPlayerOne.Controls.Add(pb);
                    nextPlayer = 2;
                    break;
                case 2:
                    flowLayoutPlayerTwo.Controls.Add(pb);
                    nextPlayer = 1;
                    break;
            }            
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                //taskAsync = new Task(BlackJackServer.Initialize, TaskCreationOptions.LongRunning);
                //taskAsync.Start();
                new Task(() => BlackJackServer.Initialize()).Start();
                myDealerDeck = BlackJackServer.gameDealer.DealerDeck;
                MessageBox.Show("Servidor inicializado exitosamente.", "Blackjack", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnDeal.Enabled = true;
                stopToolStripMenuItem.Enabled = true;
                startToolStripMenuItem.Enabled = false;
                dealCardToolStripMenuItem.Enabled = true;
                btnDeal.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inicializando servidor.", "Blackjack", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogWriter.writeError(ex.Message);
            }
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                BlackJackServer.Finish();
                MessageBox.Show("Servidor finalizado exitosamente.", "Blackjack", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Application.Exit();
            }
            catch (Exception)
            {
                MessageBox.Show("Error finalizando servidor.", "Blackjack", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }
}
