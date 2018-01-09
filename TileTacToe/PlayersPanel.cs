/*
   PlayersPanel.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is the panel shown when the game launches or a new game begins. It shows two PlayerGroupBoxes where
   each player can get a name, color and whether its played by the computer.

   Public property:
      PlayersChosen (EventHandler): Event raised when Start is clicked to begin a game.
*/

using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace TileTacToe
{
   public class PlayersPanel : Panel
   {

      // event fired when the Start button is clicked to alert that players are chosen
      public event EventHandler PlayersChosen;

      // private child controls
      private Label titleLabel; // "Welcome to Tile Tac Toe" label at top of panel
      private ComboBox whoFirstComboBox; // combo box to decide who goes first (coin flip, player 1 or 2)
      private Label whoFirstLabel; // "Who goes first?" label to left of combo box
      private Label warningLabel; // Red text to warn if identical colors chosen or a name is empty
      private Button startButton; // Start button to accept settings and start a game
      private ComboBox difficultyComboBox; // combo box to choose difficulty (0=easy, 1=hard)
      private Label difficultyLabel; // "Difficulty" label to the left of combo box

      // array of PlayerGroupBoxes to setup each player
      private PlayerGroupBox[] playerGroupBoxes;

      // click sound for controls
      private SoundPlayer soundClick = new SoundPlayer(Properties.Resources.Click);
      
      //
      // Constructor
      //

      public PlayersPanel()
      {
         // panel properties
         Dock = DockStyle.Fill; // this panel fills entire window
         Location = new Point(0, 0); // from topleft corner
         MinimumSize = new Size(280, 600); // size can be anything, but controls assume parent is at least 2800x600
         Visible = false; // starts off hidden

         // these are the PlayerGroupBoxes to make for each player in the array passed in the constructor
         playerGroupBoxes = new PlayerGroupBox[TileTacToe.Players.Length];

         AddControls(); // create controls for the panel (the "code-behind" for this panel)
         PositionControls();  // position controls relative to this panel

         // add events for player groupboxes
         for (int i = 1; i < playerGroupBoxes.Length; i++)
         {
            // event when name, color, computer player changes
            playerGroupBoxes[i].PropertiesChanged += new EventHandler(PlayerGroupBox_PropertiesChanged);
            // event when enter key used in a playername textbox
            playerGroupBoxes[i].EnterKeyPressed += new EventHandler(PlayerGroupBox_EnterKeyPressed);
         }

         startButton.Click += new EventHandler(StartButton_Click); // watch for Start button being clicked
         startButton.VisibleChanged += new EventHandler(StartButton_VisibleChanged); // watch for Start button showing to grab focus

      }

      // create controls and add them to this panel
      private void AddControls()
      {
         // "Welcome to Tile Tac Toe" header at the top of the panel
         titleLabel = new Label()
         {
            Font = new Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
            Size = new Size(280, 30),
            Text = "Welcome to Tile Tac Toe",
            TextAlign = ContentAlignment.MiddleCenter, // text centered within control
            Anchor = AnchorStyles.Top, // when parent dock fills and changes size, this will keep it centered to top

         };
         this.Controls.Add(titleLabel);

         // add a groupbox for each player except 0th player
         for (int i = 1; i < playerGroupBoxes.Length; i++)
         {
            playerGroupBoxes[i] = new PlayerGroupBox(TileTacToe.Players[i])
            {
               Anchor = AnchorStyles.Top,
               Text = "Player " + i + " choose a name and color",
               TabIndex = i + 1,
            };
            this.Controls.Add(playerGroupBoxes[i]);
         }

         // combo box to decide who goes first
         whoFirstComboBox = new ComboBox()
         {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            FormattingEnabled = true,
            Size = new Size(100, 30),
            Anchor = AnchorStyles.Top,
            TabIndex = playerGroupBoxes.Length + 2,
         };
         whoFirstComboBox.Items.AddRange(new object[] { "Coin Toss", "Player 1", "Player 2" });
         whoFirstComboBox.SelectedIndex = 0; // make first option default ("Coin Toss")
         whoFirstComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
         this.Controls.Add(whoFirstComboBox);

         // label that reads "Who goes first?" to the left of the combo box
         whoFirstLabel = new Label()
         {
            Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            Size = new Size(200, 28),
            Text = "Who goes first?",
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Top,
         };
         this.Controls.Add(whoFirstLabel);

         // combo box for difficulty
         difficultyComboBox = new ComboBox()
         {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            FormattingEnabled = true,
            Size = new Size(100, 30),
            Anchor = AnchorStyles.Top,
            TabIndex = playerGroupBoxes.Length + 3,
         };
         // with only two options, a checkbox may be better; but may add more difficulties later
         difficultyComboBox.Items.AddRange(new object[] { "Easy", "Hard" });
         difficultyComboBox.SelectedIndex = TileTacToe.Difficulty; // set default to the one defined in TileTacToe.cs
         difficultyComboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
         this.Controls.Add(difficultyComboBox);

         // label that reads "Difficulty" to the left of the combo box
         difficultyLabel = new Label()
         {
            Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            Size = new Size(200, 28),
            Text = "Computer difficulty",
            TextAlign = ContentAlignment.MiddleRight,
            Anchor = AnchorStyles.Top,
         };
         this.Controls.Add(difficultyLabel);


         // label in red that informs user why the start button is disabled ("Both players need unique colors" or "Both players need a name")
         warningLabel = new Label()
         {
            Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            Size = new Size(280, 28),
            ForeColor = Color.Red,
            TextAlign = ContentAlignment.MiddleCenter,
            Anchor = AnchorStyles.Top,
         };
         this.Controls.Add(warningLabel);

         // Start button to begin a game
         startButton = new Button()
         {
            Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(0))),
            Size = new Size(125, 40),
            Text = "Start",
            UseVisualStyleBackColor = true,
            Anchor = AnchorStyles.Top,
            TabIndex = 1,
         };
         this.Controls.Add(startButton);
      }

      // sets the Location of the controls based on the panel's size
      private void PositionControls()
      {
         int yoffset = 30; // accumulator that adds control heights as they get added (includes menu strip!)
         int ypadding = 8; // space to leave between controls in y direction

         // title at top of panel
         titleLabel.Location = new Point((ClientSize.Width - titleLabel.Width) / 2, yoffset);
         yoffset += titleLabel.Height + ypadding * 2;

         // loop through player groupboxes to add each below the other, adding to yoffset for each one
         for (int i = 1; i < playerGroupBoxes.Length; i++)
         {
            playerGroupBoxes[i].Location = new Point((ClientSize.Width - playerGroupBoxes[i].Width) / 2, yoffset);
            yoffset += playerGroupBoxes[i].Height + ypadding;
         }
         yoffset += ypadding; // add a bit of space after group boxes

         // "Who plays first" label and combobox share same yoffset
         whoFirstComboBox.Location = new Point(ClientSize.Width / 2 + 15, yoffset); // left edge centered to client center and 15px to right
         whoFirstLabel.Location = new Point(ClientSize.Width / 2 - whoFirstLabel.Width + 10, yoffset); // right-edge to client center and 10px to right
         yoffset += whoFirstLabel.Height + ypadding;

         // "Difficulty" label and combobox
         difficultyComboBox.Location = new Point(ClientSize.Width / 2 + 15, yoffset);
         difficultyLabel.Location = new Point(ClientSize.Width / 2 - difficultyLabel.Width + 10, yoffset);
         yoffset += difficultyLabel.Height;

         // red "Both players need unique colors" warning
         warningLabel.Location = new Point((ClientSize.Width - warningLabel.Width) / 2, yoffset);
         yoffset += warningLabel.Height; // + ypadding;

         // Start at the bottom of the panel
         startButton.Location = new Point((ClientSize.Width - startButton.Width) / 2, yoffset);
         yoffset += startButton.Height + ypadding * 2;

         this.Height = yoffset; // make panel high enough to cover all controls
      }

      //
      // Events
      //

      // raised when a property of a PlayerGroupBox changes (player name, color swatch, etc)
      // this disables the Start button if both players choose same color or a name is blank
      private void PlayerGroupBox_PropertiesChanged(object sender, EventArgs e)
      {
         if (PlayersHaveSameColor()) // if any players share the same color
         {
            warningLabel.Text = "Players can't share a color!";
            if (startButton.Focused) // if start button has focus, to prevent a textbox from getting focus when
               warningLabel.Focus(); // start button is disabled, focus a label instead

            startButton.Enabled = false;
         }
         else if (PlayerNameIsMissing()) // if any players don't have a name
         {
            warningLabel.Text = "All players need a name!";
            startButton.Enabled = false;
            // don't move focus; textbox was likely just emptied by the user
         }
         else // everything looks good; remove warning and enable Start button
         {
            warningLabel.Text = "";
            startButton.Enabled = true;
         }

         // check for computer-controlled players
         bool hasComputer = false;
         for (int i = 1; i <= TileTacToe.NUM_PLAYERS; i++)
            if (TileTacToe.Players[i].IsComputer)
               hasComputer = true; // at least one player is computer-controlled

         // if there's at least one computer, show difficulty combobox
         difficultyComboBox.Visible = hasComputer;
         difficultyLabel.Visible = hasComputer; // and its label
      }

      // shared by both combo boxes to make a click when something is selected
      // (the values are set in the StartButton_Click event since this may not get raised)
      private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
      {
         soundClick.Play(); // play a click sound
      }

      // raised when Enter key is pressed while inside a PlayerGroupBox playerName text box.
      // from player 1 it moves to player 2; from player 2 it moves to the start button if it's enabled
      // or back to player 1 if the start button is not enabled.
      private void PlayerGroupBox_EnterKeyPressed(object sender, EventArgs e)
      {
         PlayerGroupBox groupBox = sender as PlayerGroupBox;

         // go through all but the last groupbox, and if that was the one that raised the event, tab to next groupbox
         for (int i = 1; i < playerGroupBoxes.Length - 1; i++)
         {
            if (groupBox == playerGroupBoxes[i])
            {
               SelectNextControl(playerGroupBoxes[i + 1], true, true, true, true);
            }
         }
         // if the last groupbox raised the event, tab to start button if enabled, or the first groupbox otherwise
         if (groupBox == playerGroupBoxes[playerGroupBoxes.Length - 1])
         {
            if (startButton.Enabled)
               startButton.Select();
            else
               SelectNextControl(playerGroupBoxes[1], true, true, true, true);
         }
      }

      // raised when the start button is clicked or Enter key pressed while the button has focus
      // user accepts new game settings and wants to start a game
      private void StartButton_Click(object sender, EventArgs e)
      {
         // remove leading and trailing whitespace from player names
         for (int i = 1; i < playerGroupBoxes.Length; i++)
         {
            playerGroupBoxes[i].PlayerName = playerGroupBoxes[i].PlayerName.Trim();
         }

         // set the current turn to the combo box's index (0=coin toss, 1=player 1, 2=player 2, etc)
         TileTacToe.CurrentTurn = whoFirstComboBox.SelectedIndex;
         // set the difficulty to the combo box's index (0=easy, 1=hard)
         TileTacToe.Difficulty = difficultyComboBox.SelectedIndex;

         // raise a PlayersChosen event to tell parent form we're done
         PlayersChosen?.Invoke(this, new EventArgs());
      }

      // when the start button is made visible, select it
      private void StartButton_VisibleChanged(object sender, EventArgs e)
      {
         if (startButton.Visible)
            startButton.Select();
      }

      //
      // Helper methods
      //

      // for PlayerGroupBox_PropertiesChanged: returns true if any players have the same color chosen
      private bool PlayersHaveSameColor()
      {
         for (int i = 1; i <= TileTacToe.NUM_PLAYERS; i++)
         {
            for (int j = i + 1; j <= TileTacToe.NUM_PLAYERS; j++)
            {
               if (TileTacToe.Players[i].TileColor == TileTacToe.Players[j].TileColor)
                  return true; // a player has the same color as another player
            }
         }
         return false;
      }

      // for PlayerGroupBox_PropertiesChanged: returns true if any player's name is blank
      private bool PlayerNameIsMissing()
      {
         for (int i = 1; i <= TileTacToe.NUM_PLAYERS; i++)
         {
            if (TileTacToe.Players[i].Name.Trim() == "")
               return true; // a player has an empty/blank name
         }
         return false;
      }

   }
}
