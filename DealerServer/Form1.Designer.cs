namespace DealerServer
{
    partial class DealerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.dealerServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dealerActionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shuffleDeckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dealCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutDealerServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gboxGameInfo = new System.Windows.Forms.GroupBox();
            this.gboxPlayerTwo = new System.Windows.Forms.GroupBox();
            this.flowLayoutPlayerTwo = new System.Windows.Forms.FlowLayoutPanel();
            this.lblP2Online = new System.Windows.Forms.Label();
            this.gboxPlayerOne = new System.Windows.Forms.GroupBox();
            this.flowLayoutPlayerOne = new System.Windows.Forms.FlowLayoutPanel();
            this.lblP1Online = new System.Windows.Forms.Label();
            this.gboxLastCard = new System.Windows.Forms.GroupBox();
            this.pboxDealtCard = new System.Windows.Forms.PictureBox();
            this.gboxDealer = new System.Windows.Forms.GroupBox();
            this.btnDeal = new System.Windows.Forms.Button();
            this.btnRecorrer = new System.Windows.Forms.Button();
            this.menuStripMain.SuspendLayout();
            this.gboxGameInfo.SuspendLayout();
            this.gboxPlayerTwo.SuspendLayout();
            this.gboxPlayerOne.SuspendLayout();
            this.gboxLastCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pboxDealtCard)).BeginInit();
            this.gboxDealer.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dealerServerToolStripMenuItem,
            this.dealerActionsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(603, 24);
            this.menuStripMain.TabIndex = 2;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // dealerServerToolStripMenuItem
            // 
            this.dealerServerToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startToolStripMenuItem,
            this.stopToolStripMenuItem});
            this.dealerServerToolStripMenuItem.Name = "dealerServerToolStripMenuItem";
            this.dealerServerToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
            this.dealerServerToolStripMenuItem.Text = "Dealer Server";
            // 
            // startToolStripMenuItem
            // 
            this.startToolStripMenuItem.Name = "startToolStripMenuItem";
            this.startToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.startToolStripMenuItem.Text = "Start Server";
            this.startToolStripMenuItem.Click += new System.EventHandler(this.startToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Enabled = false;
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.stopToolStripMenuItem.Text = "Stop Server";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // dealerActionsToolStripMenuItem
            // 
            this.dealerActionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shuffleDeckToolStripMenuItem,
            this.dealCardToolStripMenuItem});
            this.dealerActionsToolStripMenuItem.Name = "dealerActionsToolStripMenuItem";
            this.dealerActionsToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.dealerActionsToolStripMenuItem.Text = "Dealer Actions";
            // 
            // shuffleDeckToolStripMenuItem
            // 
            this.shuffleDeckToolStripMenuItem.Enabled = false;
            this.shuffleDeckToolStripMenuItem.Name = "shuffleDeckToolStripMenuItem";
            this.shuffleDeckToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.shuffleDeckToolStripMenuItem.Text = "Shuffle Deck";
            // 
            // dealCardToolStripMenuItem
            // 
            this.dealCardToolStripMenuItem.Name = "dealCardToolStripMenuItem";
            this.dealCardToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.dealCardToolStripMenuItem.Text = "Deal Card";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutDealerServerToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // aboutDealerServerToolStripMenuItem
            // 
            this.aboutDealerServerToolStripMenuItem.Name = "aboutDealerServerToolStripMenuItem";
            this.aboutDealerServerToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.aboutDealerServerToolStripMenuItem.Text = "About Dealer Server";
            // 
            // gboxGameInfo
            // 
            this.gboxGameInfo.Controls.Add(this.gboxPlayerTwo);
            this.gboxGameInfo.Controls.Add(this.gboxPlayerOne);
            this.gboxGameInfo.Location = new System.Drawing.Point(13, 278);
            this.gboxGameInfo.Name = "gboxGameInfo";
            this.gboxGameInfo.Size = new System.Drawing.Size(578, 214);
            this.gboxGameInfo.TabIndex = 3;
            this.gboxGameInfo.TabStop = false;
            this.gboxGameInfo.Text = "Current Player Information";
            // 
            // gboxPlayerTwo
            // 
            this.gboxPlayerTwo.Controls.Add(this.flowLayoutPlayerTwo);
            this.gboxPlayerTwo.Controls.Add(this.lblP2Online);
            this.gboxPlayerTwo.Location = new System.Drawing.Point(292, 19);
            this.gboxPlayerTwo.Name = "gboxPlayerTwo";
            this.gboxPlayerTwo.Size = new System.Drawing.Size(280, 189);
            this.gboxPlayerTwo.TabIndex = 1;
            this.gboxPlayerTwo.TabStop = false;
            this.gboxPlayerTwo.Text = "Player Two";
            // 
            // flowLayoutPlayerTwo
            // 
            this.flowLayoutPlayerTwo.Location = new System.Drawing.Point(6, 35);
            this.flowLayoutPlayerTwo.Name = "flowLayoutPlayerTwo";
            this.flowLayoutPlayerTwo.Size = new System.Drawing.Size(268, 147);
            this.flowLayoutPlayerTwo.TabIndex = 2;
            // 
            // lblP2Online
            // 
            this.lblP2Online.AutoSize = true;
            this.lblP2Online.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblP2Online.ForeColor = System.Drawing.Color.Red;
            this.lblP2Online.Location = new System.Drawing.Point(6, 16);
            this.lblP2Online.Name = "lblP2Online";
            this.lblP2Online.Size = new System.Drawing.Size(52, 16);
            this.lblP2Online.TabIndex = 1;
            this.lblP2Online.Text = "Offline";
            // 
            // gboxPlayerOne
            // 
            this.gboxPlayerOne.Controls.Add(this.flowLayoutPlayerOne);
            this.gboxPlayerOne.Controls.Add(this.lblP1Online);
            this.gboxPlayerOne.Location = new System.Drawing.Point(6, 19);
            this.gboxPlayerOne.Name = "gboxPlayerOne";
            this.gboxPlayerOne.Size = new System.Drawing.Size(280, 189);
            this.gboxPlayerOne.TabIndex = 0;
            this.gboxPlayerOne.TabStop = false;
            this.gboxPlayerOne.Text = "Player One";
            // 
            // flowLayoutPlayerOne
            // 
            this.flowLayoutPlayerOne.Location = new System.Drawing.Point(6, 35);
            this.flowLayoutPlayerOne.Name = "flowLayoutPlayerOne";
            this.flowLayoutPlayerOne.Size = new System.Drawing.Size(268, 147);
            this.flowLayoutPlayerOne.TabIndex = 1;
            // 
            // lblP1Online
            // 
            this.lblP1Online.AutoSize = true;
            this.lblP1Online.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblP1Online.ForeColor = System.Drawing.Color.Red;
            this.lblP1Online.Location = new System.Drawing.Point(6, 16);
            this.lblP1Online.Name = "lblP1Online";
            this.lblP1Online.Size = new System.Drawing.Size(52, 16);
            this.lblP1Online.TabIndex = 0;
            this.lblP1Online.Text = "Offline";
            // 
            // gboxLastCard
            // 
            this.gboxLastCard.Controls.Add(this.pboxDealtCard);
            this.gboxLastCard.Location = new System.Drawing.Point(406, 7);
            this.gboxLastCard.Name = "gboxLastCard";
            this.gboxLastCard.Size = new System.Drawing.Size(165, 231);
            this.gboxLastCard.TabIndex = 4;
            this.gboxLastCard.TabStop = false;
            this.gboxLastCard.Text = "Last Card Dealt";
            // 
            // pboxDealtCard
            // 
            this.pboxDealtCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pboxDealtCard.Location = new System.Drawing.Point(3, 16);
            this.pboxDealtCard.Name = "pboxDealtCard";
            this.pboxDealtCard.Size = new System.Drawing.Size(159, 212);
            this.pboxDealtCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pboxDealtCard.TabIndex = 0;
            this.pboxDealtCard.TabStop = false;
            // 
            // gboxDealer
            // 
            this.gboxDealer.Controls.Add(this.btnDeal);
            this.gboxDealer.Controls.Add(this.gboxLastCard);
            this.gboxDealer.Controls.Add(this.btnRecorrer);
            this.gboxDealer.Location = new System.Drawing.Point(13, 28);
            this.gboxDealer.Name = "gboxDealer";
            this.gboxDealer.Size = new System.Drawing.Size(578, 244);
            this.gboxDealer.TabIndex = 6;
            this.gboxDealer.TabStop = false;
            this.gboxDealer.Text = "Dealer Actions";
            // 
            // btnDeal
            // 
            this.btnDeal.BackColor = System.Drawing.SystemColors.Control;
            this.btnDeal.BackgroundImage = global::DealerServer.Properties.Resources.greenRightArrow;
            this.btnDeal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDeal.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeal.Location = new System.Drawing.Point(217, 95);
            this.btnDeal.Name = "btnDeal";
            this.btnDeal.Size = new System.Drawing.Size(140, 60);
            this.btnDeal.TabIndex = 2;
            this.btnDeal.Text = "Deal";
            this.btnDeal.UseVisualStyleBackColor = false;
            this.btnDeal.Click += new System.EventHandler(this.btnDeal_Click);
            // 
            // btnRecorrer
            // 
            this.btnRecorrer.BackgroundImage = global::DealerServer.Properties.Resources.rest;
            this.btnRecorrer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRecorrer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRecorrer.Location = new System.Drawing.Point(6, 18);
            this.btnRecorrer.Name = "btnRecorrer";
            this.btnRecorrer.Size = new System.Drawing.Size(160, 217);
            this.btnRecorrer.TabIndex = 1;
            this.btnRecorrer.UseVisualStyleBackColor = false;
            this.btnRecorrer.Click += new System.EventHandler(this.btnRecorrer_Click);
            // 
            // DealerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 504);
            this.Controls.Add(this.gboxDealer);
            this.Controls.Add(this.gboxGameInfo);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "DealerForm";
            this.Text = "Dealer Server";
            this.Load += new System.EventHandler(this.DealerForm_Load);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.gboxGameInfo.ResumeLayout(false);
            this.gboxPlayerTwo.ResumeLayout(false);
            this.gboxPlayerTwo.PerformLayout();
            this.gboxPlayerOne.ResumeLayout(false);
            this.gboxPlayerOne.PerformLayout();
            this.gboxLastCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pboxDealtCard)).EndInit();
            this.gboxDealer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pboxDealtCard;
        private System.Windows.Forms.Button btnRecorrer;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem dealerServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dealerActionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shuffleDeckToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutDealerServerToolStripMenuItem;
        private System.Windows.Forms.GroupBox gboxGameInfo;
        private System.Windows.Forms.GroupBox gboxLastCard;
        private System.Windows.Forms.GroupBox gboxDealer;
        private System.Windows.Forms.Button btnDeal;
        private System.Windows.Forms.ToolStripMenuItem dealCardToolStripMenuItem;
        private System.Windows.Forms.GroupBox gboxPlayerTwo;
        private System.Windows.Forms.GroupBox gboxPlayerOne;
        private System.Windows.Forms.Label lblP1Online;
        private System.Windows.Forms.Label lblP2Online;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPlayerTwo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPlayerOne;
    }
}

