namespace TileTacToe
{
   partial class AboutBoxForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBoxForm));
         this.titleLabel = new System.Windows.Forms.Label();
         this.versionLabel = new System.Windows.Forms.Label();
         this.dateLabel = new System.Windows.Forms.Label();
         this.signatureLabel = new System.Windows.Forms.Label();
         this.okButton = new System.Windows.Forms.Button();
         this.logoPictureBox = new System.Windows.Forms.PictureBox();
         ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
         this.SuspendLayout();
         // 
         // titleLabel
         // 
         this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.titleLabel.AutoSize = true;
         this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.titleLabel.Location = new System.Drawing.Point(129, 19);
         this.titleLabel.Name = "titleLabel";
         this.titleLabel.Size = new System.Drawing.Size(123, 30);
         this.titleLabel.TabIndex = 1;
         this.titleLabel.Text = "Tile Tac Toe";
         this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // versionLabel
         // 
         this.versionLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.versionLabel.AutoSize = true;
         this.versionLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.versionLabel.Location = new System.Drawing.Point(176, 50);
         this.versionLabel.Name = "versionLabel";
         this.versionLabel.Size = new System.Drawing.Size(72, 17);
         this.versionLabel.TabIndex = 2;
         this.versionLabel.Text = "Version 1.0";
         // 
         // dateLabel
         // 
         this.dateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
         this.dateLabel.AutoSize = true;
         this.dateLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.dateLabel.Location = new System.Drawing.Point(160, 69);
         this.dateLabel.Name = "dateLabel";
         this.dateLabel.Size = new System.Drawing.Size(88, 17);
         this.dateLabel.TabIndex = 3;
         this.dateLabel.Text = "April 29, 2017";
         // 
         // signatureLabel
         // 
         this.signatureLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.signatureLabel.Location = new System.Drawing.Point(18, 111);
         this.signatureLabel.Name = "signatureLabel";
         this.signatureLabel.Size = new System.Drawing.Size(251, 91);
         this.signatureLabel.TabIndex = 4;
         this.signatureLabel.Text = "signature";
         this.signatureLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
         // 
         // okButton
         // 
         this.okButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.okButton.Location = new System.Drawing.Point(97, 212);
         this.okButton.Name = "okButton";
         this.okButton.Size = new System.Drawing.Size(91, 37);
         this.okButton.TabIndex = 5;
         this.okButton.Text = "Ok";
         this.okButton.UseVisualStyleBackColor = true;
         this.okButton.Click += new System.EventHandler(this.OkButton_Click);
         // 
         // logoPictureBox
         // 
         this.logoPictureBox.Image = global::TileTacToe.Properties.Resources.TileTacToe;
         this.logoPictureBox.Location = new System.Drawing.Point(18, 19);
         this.logoPictureBox.Name = "logoPictureBox";
         this.logoPictureBox.Size = new System.Drawing.Size(75, 75);
         this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
         this.logoPictureBox.TabIndex = 0;
         this.logoPictureBox.TabStop = false;
         // 
         // AboutBoxForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(281, 261);
         this.Controls.Add(this.okButton);
         this.Controls.Add(this.signatureLabel);
         this.Controls.Add(this.dateLabel);
         this.Controls.Add(this.versionLabel);
         this.Controls.Add(this.titleLabel);
         this.Controls.Add(this.logoPictureBox);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.Name = "AboutBoxForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "About Tile Tac Toe";
         ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.PictureBox logoPictureBox;
      private System.Windows.Forms.Label titleLabel;
      private System.Windows.Forms.Label versionLabel;
      private System.Windows.Forms.Label dateLabel;
      private System.Windows.Forms.Label signatureLabel;
      private System.Windows.Forms.Button okButton;
   }
}