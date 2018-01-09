/*
   AboutBoxForm.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is the form for the About box dialog.
   (Most of it is in the "code behind" in AboutBoxForm.Designer.cs)
*/

using System;
using System.Media;
using System.Windows.Forms;

namespace TileTacToe
{
   public partial class AboutBoxForm : Form
   {
      // constructor
      public AboutBoxForm()
      {
         InitializeComponent();

         // use the signature from Signature class to display in the about box
         this.signatureLabel.Text = (new Signature() { Motto = null }).ToString();
      }

      // Ok button on the about box closes the form
      private void OkButton_Click(object sender, EventArgs e)
      {
         SoundPlayer soundClick = new SoundPlayer(Properties.Resources.Click);
         soundClick.Play();
         this.Close();
      }
   }
}
