/*
   MainForm.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is the main UI portion of the program.

   It creates the parent panels (PlayersPanel and GamePanel), controls program flow between the two panels,
   and most other "higher level" UI handling.

   MainForm.Designer.cs just defines the menu strip and the parent form. All other child elements are
   created programmatically.
*/

using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace TileTacToe
{
   public partial class MainForm : Form
   {
      private PlayersPanel playersPanel; // the panel at the start of a game to enter names and choose colors
      private GamePanel gamePanel; // the panel with the 4x4x4 cells where the game is played
      private AboutBoxForm aboutBoxForm; // about box from menu

      private Timer computerTimer; // timer used for computer to take its turn
      private CellButton computerChosenCell; // cell chosen by the computer after it's "thought" about it

      private Timer winnerTimer; // timer used to flash winning line when game is over
      private int winnerFlashState = 0; // cycles from 0-8 to rotate through colors when a line is won

      // all sounds are from freesound.org (creative commons license)
      private SoundPlayer soundClick = new SoundPlayer(Properties.Resources.Click); // generic click sound for menus
      private SoundPlayer soundComputerPick = new SoundPlayer(Properties.Resources.ComputerPick); // sound of computer picking a cell
      private SoundPlayer soundGameLost = new SoundPlayer(Properties.Resources.GameLost); // little jingle when a game is lost
      private SoundPlayer soundGameWon = new SoundPlayer(Properties.Resources.GameWon); // little jingle when a game is won
      private SoundPlayer soundGameStart = new SoundPlayer(Properties.Resources.GameStart); // very short jingle when going to game board

      //
      // Constructor
      //

      public MainForm()
      {
         InitializeComponent();

         // set up PlayersPanel that appears at the start of each game, to ask for player names, colors, etc
         playersPanel = new PlayersPanel() { Size = ClientSize };
         this.Controls.Add(playersPanel);
         // the PlayersChosen event is raised in PlayersPanel when the Start button is clicked
         playersPanel.PlayersChosen += new EventHandler(MainForm_PlayersChosen);

         // set up GamePanel with the 4 platforms and 4x4x4 cells where the game is played
         gamePanel = new GamePanel() { Size = ClientSize };
         this.Controls.Add(gamePanel);
         // the TurnComplete event is raised when a cell is clicked by a human or chosen by the computer
         gamePanel.TurnCompleted += new EventHandler(MainForm_TurnCompleted);
         // the PlayAgain event is raised when the Play Again button is clicked in the game panel
         gamePanel.PlayAgain += new EventHandler(MainForm_PlayAgain);

         aboutBoxForm = new AboutBoxForm();

         // timer to make the computer take some time to 
         computerTimer = new Timer();
         computerTimer.Tick += new EventHandler(ComputerTimer_Tick);
         computerTimer.Interval = 500; // speed (in ms) it takes for computer to "think" about a turn

         // timer to make the winning flash cycle through its tilecolor
         winnerTimer = new Timer();
         winnerTimer.Tick += new EventHandler(WinnerTimer_Tick);
         winnerTimer.Interval = 75; // time (in ms) between cycling of colors on a winning line

         // setup the cellLines that are used to 
         GameLogic.BuildCellLines();
         
         // start a new game!
         StartNewGame();
      }

      // when starting a new game, return to the PlayersPanel to choose players, colors, etc
      private void StartNewGame()
      {
         TileTacToe.IsGameOver = false; // starting a new game

         gamePanel.Visible = false; // hide game board
         gamePanel.UpdatePlayAgainButton();

         // reset all cells to an unowned state
         for (int x = 0; x < TileTacToe.BOARD_SIZE; x++)
            for (int y = 0; y < TileTacToe.BOARD_SIZE; y++)
               for (int z = 0; z < TileTacToe.BOARD_SIZE; z++)
                  TileTacToe.Cells[x, y, z].Owner = 0;

         // stop timers if any enabled
         computerTimer.Enabled = false;
         winnerTimer.Enabled = false;

         newGameToolStripMenuItem.Enabled = false; // disable New Game menu while choosing players

         playersPanel.Visible = true; // show the players panel to pick players
      }

      //
      // Events
      //

      // when the PlayersPanel raises a PlayersChosen event due to the Start button being clicked
      private void MainForm_PlayersChosen(object sender, EventArgs e)
      {
         playersPanel.Visible = false; // hide the players panel

         newGameToolStripMenuItem.Enabled = true; // enable New Game menu after players chosen

         // if the current turn is 0, pick a random player to start
         if (TileTacToe.CurrentTurn == 0)
            TileTacToe.CurrentTurn = (new Random()).Next(1, TileTacToe.NUM_PLAYERS + 1);

         gamePanel.UpdateStatus(TileTacToe.CurrentTurn, "{0} goes first!"); // say who goes first

         gamePanel.Visible = true; // show the game board
         soundGameStart.Play(); // play a very short jingle when going to game board

         // if computer is up first, have it play first
         HandlePossibleComputerTurn(sender, e);
      }

      // when either the gamePanel raises a TurnCompleted event due to a cell being clicked and changing owners,
      // or computerTimer Tick calls this directly due to the computer choosing a cell.
      // in both cases sender is the CellButton that was just claimed.
      private void MainForm_TurnCompleted(object sender, EventArgs e)
      {
         if (GameLogic.CheckForGameOver()) // if the game is over
         {
            TileTacToe.IsGameOver = true; // game is over!
            gamePanel.UpdatePlayAgainButton(); // show the play again button

            if (GameLogic.GetWinningLine()!=null) // if there's a winning line, this isn't a draw
            {
               // count number of humans playing to determine how to celebrate the win
               int numHumans = 0;
               for (int i = 1; i < TileTacToe.NUM_PLAYERS + 1; i++)
                  if (!TileTacToe.Players[i].IsComputer)
                     numHumans++;

               // if there's only one human player and the computer won, no need to congratulate the computer
               if (numHumans==1 && TileTacToe.Players[TileTacToe.CurrentTurn].IsComputer)
               {
                  gamePanel.UpdateStatus("You lost, sorry! Maybe next time!");
                  soundGameLost.Play(); // play a sad jingle when a game lost
               }
               else // otherwise (if two humans or human won), congratulate the winner
               {
                  gamePanel.UpdateStatus(TileTacToe.CurrentTurn, "{0} won! Congratulations!");
                  soundGameWon.Play(); // play a happy single when a game won
               }
               winnerTimer.Enabled = true; // begin flashing the winning line
            }
            else // no winning line but the game is over, this is a draw :(
            {
               gamePanel.UpdateStatus("There is no winning moves left. It's a draw!");
               soundGameLost.Play(); // play a sad jingle when a game is a draw too
            }
         }
         else // the game is not over
         {
            // advance to the next player
            TileTacToe.CurrentTurn = TileTacToe.CurrentTurn % TileTacToe.NUM_PLAYERS + 1;
            // update status to notify of next player's turn
            gamePanel.UpdateStatus(TileTacToe.CurrentTurn, "{0}'s turn");
            // if computer is up first, have it play first
            HandlePossibleComputerTurn(sender, e);
            // otherwise, wait for human to click a cell
         }

      }

      // when the GamePanel raises a PlayAgain event due to the PlayAgain button being clicked, start a new game
      private void MainForm_PlayAgain(object sender, EventArgs e)
      {
         StartNewGame();
      }

      // this checks if it's the computer's turn. if so it will choose a cell to give it a highlight
      // as if computer is mousing over it, and then start a timer to choose in a fraction of a second later
      private void HandlePossibleComputerTurn(object sender, EventArgs e)
      {
         // if current turn is a computer's turn
         if (TileTacToe.Players[TileTacToe.CurrentTurn].IsComputer)
         {
            // not claiming cell just yet, saving a cell to choose
            computerChosenCell = GameLogic.ChooseACell();
            // then highlighting it
            computerChosenCell.BackColor = TileTacToe.Players[TileTacToe.CurrentTurn].TileColor.Lightest;
            // and starting a timer to actually claim it during the Tick
            computerTimer.Enabled = true;
         }
      }

      // timer runs a fraction of a second after computer has chosen a cell, to actually claim it
      // and then "raises" the TurnCompleted event by calling the event handler directly
      private void ComputerTimer_Tick(object sender, EventArgs e)
      {
         computerTimer.Enabled = false; // turn off timer
         computerChosenCell.Owner = TileTacToe.CurrentTurn; // claim cell
         soundComputerPick.Play(); // play a sound different from human
         MainForm_TurnCompleted(computerChosenCell, e); // and go through the motions of turn ending
      }

      // flashes the winning line between light and dark versions of itself
      private void WinnerTimer_Tick(object sender, EventArgs e)
      {
         // get the winning line to flash
         CellButton[] winningLine = GameLogic.GetWinningLine();

         if (winningLine != null) // make certain it is a winning line
         {
            TileColor tileColor = TileTacToe.Players[winningLine[0].Owner].TileColor; // get the owner's color from the winning line
            Color flashColor;

            winnerFlashState = (winnerFlashState + 1) % 8;

            // colors cycle darkest -> darker -> normal -> lighter -> lighter -> normal -> darker -> darkest and repeats
            // this makes darkest and lighter stay twice as long for more "bouncy" transition
            switch (winnerFlashState)
            {
               case 0:
               case 7:
                  flashColor = tileColor.Darkest;
                  break;
               case 1:
               case 6:
                  flashColor = tileColor.Darker;
                  break;
               case 2:
               case 5:
                  flashColor = tileColor.Normal;
                  break;
               default:
                  flashColor = tileColor.Lighter;
                  break;
            }

            foreach (var cell in winningLine) // and recolor all cells in the line for this tick
               cell.BackColor = flashColor;
         }
      }

      // "New Game" menu item under Game menu starts a new game
      private void NewGame_MenuItem_Click(object sender, EventArgs e)
      {
         soundClick.Play(); // play a click sound
         StartNewGame();
      }

      // "Exit" menu item under Game menu closes the program
      private void Exit_MenuItem_Click(object sender, EventArgs e)
      {
         Application.Exit();
      }

      // "About" menu item under Help menu opens the about box
      private void About_MenuItem_Click(object sender, EventArgs e)
      {
         soundClick.Play(); // play a click sound
         aboutBoxForm.ShowDialog();
      }

   }

}
