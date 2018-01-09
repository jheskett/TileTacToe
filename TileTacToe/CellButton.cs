/*
   CellButton.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is a custom Button control for a parallelogram-shaped button. Like ColorSwatchButton, this
   is built from a PictureBox; but CellButtons do not have their own Paint handlers.  Its color is
   set by its BackColor.

   The cell's dimensions are defined from constants in TileTacToe.cs. The constructor will use a
   GraphicsPath to set a Region to reshape the button from a rectangle.

   Public properties:
      Weight (int): How many lines the cell participates in (defined in GameLogic)
      Owner (int): The index of the Player who owns the cell, or 0 if nobody owns the cell
      TurnCompleted (EventHandler): Event raised when a cell is claimed by a player's click

   When its Owner property is set to an integer (0-NUM_PLAYERS) it will set the BackColor to
   that player's TileColor.

   When an unowned cell is clicked and the Owner changes, each button will raise a TurnCompleted event.
*/

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Media;
using System.Windows.Forms;

namespace TileTacToe
{
   public class CellButton : PictureBox
   {

      // event raised when an unowned cell is clicked
      public event EventHandler TurnCompleted;

      // some cells have more strategic value than others because a cell may be involved in more winning lines
      // the cell's Weight will be the number of lines it exists in (calculated in GameLogic as it builds the lines)
      public int Weight { get; set; }

      // owner is 0 for an unowned cell, 1 for player 1's cell, 2 for player 2's cell, etc
      private int owner;

      // a "pop" sound when a human picks a cell
      private SoundPlayer soundCellPick = new SoundPlayer(Properties.Resources.CellPick);

      // public accessor for owner property
      public int Owner
      {
         get
         {
            return owner;
         }
         set
         {
            // only accept new value if it's within range (0 to number of players) AND
            // the existing cell is unowned (owner==0) or it's resetting the cell to unowned (value==0)
            if (value >= 0 && value <= TileTacToe.NUM_PLAYERS && (owner == 0 || value == 0))
            {
               owner = value;
               BackColor = TileTacToe.Players[owner].TileColor.Normal;
            }
         }
      }

      //
      // Constructor
      //

      public CellButton()
      {
         // all cells are a fixed size defined by the constants in TileTacToe.cs
         Size = new Size(TileTacToe.CELL_WIDTH, TileTacToe.CELL_HEIGHT);

         // to make the button not only shaped like a parallelogram but only receive
         // mouse events within the shape, a GraphicsPath is made in the shape of a
         // parallelogram and then it's applied to the control's Region.

         // create a GraphicsPath that will describe the path of a parallelogram
         GraphicsPath path = new GraphicsPath(FillMode.Winding);
         // add a parallelogram to the path
         path.AddPolygon(new Point[]
         {
            new Point(TileTacToe.CELL_SKEW, 0), // topleft x offset to right by skew
            new Point(TileTacToe.CELL_WIDTH, 0), // topright is absolute topright
            new Point(TileTacToe.CELL_WIDTH-TileTacToe.CELL_SKEW, TileTacToe.CELL_HEIGHT), // bottomright x offset left by skew
            new Point(0, TileTacToe.CELL_HEIGHT), // bottomleft is absolute bottomleft
            new Point(TileTacToe.CELL_SKEW, 0) // return to topleft
         });
         // and apply it to the button's Region to define the button's boundries
         this.Region = new Region(path);

         // these event handlers are private to this button
         this.Click += new EventHandler(CellButton_OnClick);
         this.MouseEnter += new EventHandler(CellButton_OnEnter);
         this.MouseLeave += new EventHandler(CellButton_OnLeave);
         this.MouseDown += new MouseEventHandler(CellButton_OnMouseDown);

         Owner = 0; // default state
         Weight = 0; // start out with no strategic value to cell
      }

      // helper function to return true if this cell is available (owner 0), the game isn't over (!IsGameOver),
      // and it's not a computer's turn (!Players[CurrentTurn].IsComputer)
      private bool IsCellAvailable()
      {
         return Owner==0 && !TileTacToe.IsGameOver && !TileTacToe.Players[TileTacToe.CurrentTurn].IsComputer;
      }

      //
      // Events
      //

      // clicking a cell (if it's not owned by anyone and it's not the computer's turn) will set the cell's owner to the
      // current player and then raise a TurnCompleted event if parent panel has the event registered
      protected void CellButton_OnClick(object sender, EventArgs e)
      {
         if (IsCellAvailable())
         {
            // set this cell's Owner to the current turn (setter will set BackColor)
            Owner = TileTacToe.CurrentTurn;

            // play a "pop" sound when a cell is chosen
            soundCellPick.Play();

            // invoking TurnCompleted separate from an OnClick to make sure only valid completed turn events happen,
            // and not just any click to any cell
            TurnCompleted?.Invoke(this, new EventArgs());
         }
      }

      // mouse entering a cell (if it's not owned by anyone) will color the cell a very pale version of the current player's color
      protected void CellButton_OnEnter(object sender, EventArgs e)
      {
         if (IsCellAvailable())
            BackColor = TileTacToe.Players[TileTacToe.CurrentTurn].TileColor.Lightest;
      }

      // mouse leaving a cell (if it's not owned by anyone) will return it to 0th player's Normal color (white square)
      protected void CellButton_OnLeave(object sender, EventArgs e)
      {
         if (IsCellAvailable())
            BackColor = TileTacToe.Players[0].TileColor.Normal;
      }

      // to make it seem to be "pressed", a cell (if it's not owned by anyone) will color it a Lighter color between Normal and Pale
      // an OnMouseUp is not required since either an OnLeave or OnClick will fire depending where the mouse is when released
      protected void CellButton_OnMouseDown(object sender, MouseEventArgs e)
      {
         if (IsCellAvailable())
            BackColor = TileTacToe.Players[TileTacToe.CurrentTurn].TileColor.Lighter;
      }

   }
}
