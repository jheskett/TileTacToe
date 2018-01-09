/*
   PlayerGroupBox.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   PlayerGroupBox is a GroupBox-derived control that provides a textbox to enter a player name,
   a series of color swatches to choose a color, and a checkbox to choose whether the computer should
   play this player.

   Public properties:
      PlayerName (string): Text within the textbox
      TileColor (TileColor): The color swatch selected (only one will be selected at a time)
      IsComputer (bool): Whether this player should be played by the computer
      (The above properties actually get/set to the properties of the Player passed in the constructor)
      PropertiesChanged (EventHandler): Raised when either name, color or checkbox changes (invoked in properties' setters)
      EnterKeyPressed (EventHandler): Raised when the enter key is hit while the name textbox has focus (invoked in textbox's KeyPress event)
*/

using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace TileTacToe
{
   class PlayerGroupBox : GroupBox
   {
      // event handlers for a parent form/control to react to interaction with this control
      public event EventHandler PropertiesChanged; // when the name, color or computer player option changes
      public event EventHandler EnterKeyPressed; // when the Enter key is pressed while the name textbox has focus

      // constants for building the control
      private const int GROUPBOX_WIDTH = 260; // width of the groupbox
      private const int GROUPBOX_HEIGHT = 150; // height of the groupbox
      private const int SWATCH_SPACING = 36; // space between left edge of each color swatch
      private const int TEXTBOX_WIDTH = 170; // width of the textbox to enter name

      // instead of separate playerName, tileColor and isComputer private properties, the Player passed
      // in the constructor is used for the public properties (since these values will be assigned to the
      // Player object anyway)
      private Player player; // this becomes the Player passed in the constructor

      // private child controls
      private TextBox playerNameTextBox;
      private CheckBox isComputerCheckBox;
      private ColorSwatchButton[] swatches = new ColorSwatchButton[colorChoices.Length]; // array of color swatches (one for each color below)

      // static list of colors to use for color swatches; can add, remove, or rearrange colors, swatches will
      // adjust, but make sure the default color choices are present (Color.Blue and Color.Red presently)
      private static TileColor[] colorChoices =
      {
         new TileColor(Color.Red),
         new TileColor(Color.DarkOrange),
         new TileColor(Color.Green),
         // new TileColor(Color.Teal), // teal is too close to green, uncomment to add it back as an option
         new TileColor(Color.Blue),
         new TileColor(Color.Purple),
       };

      // a "click" like sound when a swatch or checkbox is clicked
      private SoundPlayer soundClick = new SoundPlayer(Properties.Resources.Click);

      // setting a PlayerName will also assign it to the textbox's Text property (and the groupbox's Text for the first name assigned)
      // (using PlayerName instead of Name because a GroupBox already has a Name property)
      public string PlayerName
      {
         get
         {
            return player.Name;
         }
         set
         {
            player.Name = value;
            playerNameTextBox.Text = player.Name; // put the name is the textbox
            PropertiesChanged?.Invoke(this, new EventArgs()); // call event handler if defined
         }
      }

      // setting a SelectedColor will also color the player textbox and set the swatch's Selected property
      // (an individual swatch's Selected property will display a rectangle around the swatch to show it's selected)
      public TileColor TileColor
      {
         get
         {
            return player.TileColor;
         }
         set
         {
            player.TileColor = value;
            playerNameTextBox.ForeColor = player.TileColor.Darker; // color name textbox the darker version of the selected color
            // only one swatch can be selected at a time: turn off each swatch's Selected property except one assigned
            foreach (ColorSwatchButton swatch in swatches)
               swatch.Selected = player.TileColor == swatch.TileColor;
            // note that unlike the textbox or check that only fires on a change, the following fires every time a swatch is clicked
            PropertiesChanged?.Invoke(this, new EventArgs()); // call event handler if defined
         }
      }

      // setting a IsComputer will also check/uncheck its checkbox
      public bool IsComputer
      {
         get
         {
            return player.IsComputer;
         }
         set
         {
            player.IsComputer = value;
            isComputerCheckBox.Checked = player.IsComputer; // make the checkbox control mirror this value
            PropertiesChanged?.Invoke(this, new EventArgs()); // call event handler if defined
         }
      }

      //
      // Constructor
      //

      public PlayerGroupBox(Player playerObject)
      {
         player = playerObject;

         // define groupbox properties
         Size = new Size(GROUPBOX_WIDTH, GROUPBOX_HEIGHT); // fixed size
         Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

         // name textbox near top
         playerNameTextBox = new TextBox()
         {
            Size = new Size(TEXTBOX_WIDTH, 32),
            Location = new Point(ClientSize.Width / 2 - TEXTBOX_WIDTH / 2, 30), // center textbox
            TextAlign = HorizontalAlignment.Center,
            Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
            TabIndex = 1,
            Text = PlayerName,
            ForeColor = TileColor.Darker,
         };
         playerNameTextBox.TextChanged += new EventHandler(PlayerNameTextBox_TextChanged);
         playerNameTextBox.KeyPress += new KeyPressEventHandler(PlayerNameTextBox_KeyPress);
         this.Controls.Add(playerNameTextBox);

         // color swatches from the available choices
         for (int i = 0; i < swatches.Length; i++)
         {
            swatches[i] = new ColorSwatchButton()
            {
               // 0th swatch's x offset is at parent width - (half the number of swatches * spacing)
               // add i*spacing and swatches will be horizontally centered regardless how many swatches exist
               Location = new Point(ClientSize.Width / 2 - (swatches.Length * SWATCH_SPACING) / 2 + (i * SWATCH_SPACING), 75),
               TileColor = colorChoices[i],
               Selected = colorChoices[i] == TileColor, // select the one chosen by the player
            };
            swatches[i].Click += new EventHandler(ColorSwatchButton_Click);
            this.Controls.Add(swatches[i]);
         }

         // checkbox "Computer player" near bottom
         isComputerCheckBox = new CheckBox()
         {
            Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            Location = new Point(60, 115),
            AutoSize = true,
            Size = new Size(225, 25),
            TabIndex = 2,
            Text = "Computer player",
            UseVisualStyleBackColor = true,
            Checked = IsComputer,
         };
         isComputerCheckBox.CheckedChanged += new EventHandler(IsComputerCheckBox_CheckChanged);
         this.Controls.Add(isComputerCheckBox);
      }

      //
      // Event Handlers
      //

      // event raised when any color swatch in the groupbox is clicked
      private void ColorSwatchButton_Click(object sender, EventArgs e)
      {
         ColorSwatchButton clickedSwatch = sender as ColorSwatchButton;

         // go through each color swatch and if sender was one clicked,
         // save it to SelectedColor (public property) and recolor playerName
         foreach (ColorSwatchButton swatch in swatches)
         {
            if (clickedSwatch == swatch) // this swatch is the one clicked
               TileColor = swatch.TileColor; // this property's setter will raise a PropertiesChanged event
         }

         soundClick.Play(); // play a "click" sound
      }

      // event raised when the computer player checkbox changes (by user or programatically)
      private void IsComputerCheckBox_CheckChanged(object sender, EventArgs e)
      {
         IsComputer = isComputerCheckBox.Checked;
         soundClick.Play(); // play a "click" sound
      }

      // event raised when the text changes in the player name textbox (by user or programatically)
      private void PlayerNameTextBox_TextChanged(object sender, EventArgs e)
      {
         PlayerName = playerNameTextBox.Text;
      }

      // event raised when a key is pressed while the player name textbox has focus
      // (only watching for Enter being hit to raise custom EnterKeyPressed event)
      private void PlayerNameTextBox_KeyPress(object sender, KeyPressEventArgs e)
      {
         if (e.KeyChar == (char)Keys.Return) // if Enter key is hit
         {
            EnterKeyPressed?.Invoke(this, new EventArgs()); // call event handler if defined
            e.Handled = true; // suppress the "ding" error sound when hitting enter in a non-multiline textbox
         }
      }

   }

}
