/*
   GamePanel.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   The GamePanel is a full-window panel that builds and draws four platforms of 4x4 cells.
   Its primary concern is building the game board. The individual CellButtons, MainForm, and
   GameLogic are where the game logic happens.

   Public properties:
      TurnCompleted (EventHandler): Event raised when one of the cells is claimed
      PlayAgain (EventHandler): Event raised when the Play Again button is clicked
   
   Public methods:
      UpdatePlayAgainButton(): Shows/hides the Play Again button depending on IsGameOver value
      UpdateStatus(int turn, string text): Displays text at top of the panel, using the turn's color
      UpdateStatus(string text): Displays text at the top of the panel in black
*/


using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;

namespace TileTacToe
{
   public class GamePanel : Panel
   {
      // event raised when a player finishes their turn (bubbled up from CellButton's TurnCompleted)
      public event EventHandler TurnCompleted;
      // event raised when the "Play Again" buttin is clicked
      public event EventHandler PlayAgain;

      const int Z_SPACING = 120; // distance between platforms
      const int X_OFFSET = 40; // x offset to start drawing cells from topleft of form
      const int Y_OFFSET = 75; // y offset to start drawing cells from topleft of form

      private Label gameStatusLabel; // the "Player 1 goes first" etc label at the top is public so other modules can update its text
      private Button playAgainButton; // the "Play Again" button that appears at the end of a game

      private SoundPlayer soundClick = new SoundPlayer(Properties.Resources.Click); // click sound for Play Again being clicked

      //
      // Constructor
      //

      public GamePanel()
      {
         Dock = DockStyle.Fill; // this panel fills entire window
         Location = new Point(0, 0); // from topleft corner
         MinimumSize = new Size(350, 600); // this assumes a 4x4x4 board
         Visible = false; // starts off hidden

         // set up a Paint event to draw the platforms beneath the buttons
         this.Paint += new PaintEventHandler(GamePanel_Paint);

         // label at the top of the panel: "Player 1's turn", "Player 2 goes first!", "Player 1 won!" etc
         gameStatusLabel = new Label()
         {
            Font = new Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
            Location = new Point(30, 30),
            Size = new Size(350, 32),
            TextAlign = ContentAlignment.MiddleCenter,
         };
         this.Controls.Add(gameStatusLabel);

         // button at bottomright of panel to Play Again when a game is over
         playAgainButton = new Button()
         {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0))),
            Location = new Point(304, 514),
            Size = new Size(85, 35),
            Text = "Play Again",
            UseVisualStyleBackColor = true,
            Visible = true,
         };
         this.Controls.Add(playAgainButton);
         playAgainButton.Click += new EventHandler(PlayAgainButton_Click);
         playAgainButton.VisibleChanged += new EventHandler(PlayAgainButton_VisibleChanged); // to grab focus when button shown

         // finally instantiate each CellButton on the 4x4x4 board and position it
         for (int x = 0; x < TileTacToe.BOARD_SIZE; x++)
            for (int y = 0; y < TileTacToe.BOARD_SIZE; y++)
               for (int z = 0; z < TileTacToe.BOARD_SIZE; z++)
               {
                  TileTacToe.Cells[x, y, z] = new CellButton()
                  {
                     Location = GetCellLocation(x, y, z),
                  };
                  TileTacToe.Cells[x, y, z].TurnCompleted += new EventHandler(GamePanel_TurnCompleted);
                  this.Controls.Add(TileTacToe.Cells[x, y, z]);
               }

      }

      // returns the location as a Point for a cell at x,y,z based on the GamePanel's constants
      private Point GetCellLocation(int x, int y, int z)
      {
         // if each cell is a total 80 width, with a skew of 25, then the x offset from cell to cell actually 55 (80-25)
         // the absolute x position is also adjusted by the y value since the topleft of one cell is offset by the skew from
         // the topleft of the cell beneath it. (they "lean" and don't line up vertically on the same x axis)
         return new Point(x * (TileTacToe.CELL_WIDTH - TileTacToe.CELL_SKEW + 1) + X_OFFSET + (3 - y) * (TileTacToe.CELL_SKEW + 1),
                          y * (TileTacToe.CELL_HEIGHT + 1) + Y_OFFSET + (z * Z_SPACING));
      }

      //
      // Public Methods
      //

      // public method to show/hide the PlayAgain button that only appears when the game is over
      public void UpdatePlayAgainButton()
      {
         playAgainButton.Visible = TileTacToe.IsGameOver;
      }

      // this is called to update the status label at the top of the panel ("Player 1 goes first!" etc)
      // if the passed text includes a {0} it will replace it with the player's name
      public void UpdateStatus(int turn, string text)
      {
         if (text.Contains("{0}")) // if text contains a {0} then use the turn's player name
            gameStatusLabel.Text = string.Format(text, TileTacToe.Players[turn]);
         else // otherwise just use the unformatted text
            gameStatusLabel.Text = text;

         // color label the same as the turn, unless turn is 0 then color it black
         gameStatusLabel.ForeColor = turn > 0 ? TileTacToe.Players[turn].TileColor.Darker : Color.Black;
      }

      // overload without a turn will update status with black color
      public void UpdateStatus(string text)
      {
         UpdateStatus(0, text);
      }

      //
      // Events
      //


      // Paint event on main form draws four huge parallelograms beneath each tier of cells to create
      // platforms of contrasting color.
      private void GamePanel_Paint(object sender, PaintEventArgs e)
      {

         e.Graphics.SmoothingMode = SmoothingMode.AntiAlias; // make edges smoother with anti-aliasing
         Pen pen = new Pen(Color.Black, 1); // 1px black border around outside

         int size = TileTacToe.BOARD_SIZE;

         // defines points to outline the four platforms beneath each tier of cells
         Point[][] PlatformPoints = new Point[size][];
         Point point; // disposable Point for breaking out X/Y values
         for (int z = 0; z < size; z++)
         {
            PlatformPoints[z] = new Point[4]; // each platform is a sub-array of 4 points
            // use locations for out-of-bound cells 4,0,z 4,4,z and 0,4,z as boundries
            point = GetCellLocation(0, 0, z); // topleft
            PlatformPoints[z][0] = new Point(point.X + TileTacToe.CELL_SKEW - 1, point.Y - 2);
            point = GetCellLocation(size, 0, z); // topright
            PlatformPoints[z][1] = new Point(point.X + TileTacToe.CELL_SKEW + 3, point.Y - 2);
            point = GetCellLocation(size, size, z); // bottomright
            PlatformPoints[z][2] = new Point(point.X + TileTacToe.CELL_SKEW + 1, point.Y + 1);
            point = GetCellLocation(0, size, z); // bottomleft
            PlatformPoints[z][3] = new Point(point.X + TileTacToe.CELL_SKEW - 4, point.Y + 1);
         }


         for (int z = 0; z < TileTacToe.BOARD_SIZE; z++)
         {
            // fill a grey parallelogram
            e.Graphics.FillPolygon(Brushes.DarkGray, PlatformPoints[z]);
            // outline it with the black pen
            e.Graphics.DrawPolygon(pen, PlatformPoints[z]);
         }

         pen.Dispose();
      }

      // event raised by an unowned cell being clicked, raise an event up to MainForm to handle turn completing
      private void GamePanel_TurnCompleted(object sender, EventArgs e)
      {
         // sending up the same sender and event args of the cell that was clicked
         TurnCompleted?.Invoke(sender, e);
      }

      // click of the "Play Again" button that appears when the game is over
      private void PlayAgainButton_Click(object sender, EventArgs e)
      {
         soundClick.Play(); // play a "click" when the button is pressed
         PlayAgain?.Invoke(sender, e);
      }

      // when the Play Again button is shown, Select() it to grab focus
      private void PlayAgainButton_VisibleChanged(object sender, EventArgs e)
      {
         if (playAgainButton.Visible)
            playAgainButton.Select();
      }

   }
}
