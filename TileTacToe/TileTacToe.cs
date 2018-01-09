/*
   TileTacToe.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is the main module of the game, where Main() starts.

   It's also where constants and important globals are defined for use throughout the game.

   Public properties:
      IsGameOver (bool): Property that becomes true when the game is over by win or draw
      CurrentTurn (int): Index into Players[] of the current turn
      Cells (CellButton[,,]): The 4x4x4 array of CellButtons where the game is played
      Players (Player[]): Array of players; 0 index is for unowned player, 1+ is for regular players
      Difficulty (int): The challenge level of computer AI (0=easy, 1=hard)

   Note: if NUM_PLAYERS and BOARD_SIZE are ever changed to higher numbers, the panels should be
   made AutoScroll or bigger somehow. The window is sized to fit 2 players and a 4x4x4 board.
   CELL_LINES would also need adjusted to a new BOARD_SIZE. And as noted in GameLogic, the game's
   logic assumes a 4x4x4 board also. In all other respects those constants are safe to change.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TileTacToe
{
   public static class TileTacToe
   {
      // constants for the game's fundamental stats
      public const int NUM_PLAYERS = 2; // number of players
      public const int BOARD_SIZE = 4; // size of each side of the 3D board (4 for 4x4x4)
      public const int CELL_LINES = 76; // there are 76 winning lines in a 4x4x4 board (49 for 3x3x3 and 109 for 5x5x5)

      // constants used in both CellButton and GamePanel to define cell size/shape and position
      public const int CELL_WIDTH = 80; // width of each cell on the game board
      public const int CELL_HEIGHT = 25; // height of each cell on the game board
      public const int CELL_SKEW = 25; // how much the topleft of the cell offsets to the right to form a parallelogram

      // public properties to keep track of the state of the game
      public static bool IsGameOver = false; // becomes true when the game is over
      public static int CurrentTurn = 1; // the player who's turn it is to play (0=Unknown/Coin Flip, 1=Player 1, 2=Player 2, etc)
      public static int Difficulty = 1; // 0=Easy, 1=Hard

      // the 3D array of cells where the game is played (each cell is instantiated in GamePanel's constructor)
      public static CellButton[,,] Cells = new CellButton[BOARD_SIZE, BOARD_SIZE, BOARD_SIZE];

      // the list of players (with some default values): [0]=Unowned, [1]=Player 1, [2]=Player 2
      // note: length of this array is NUM_PLAYERS+1 since the first actual player starts at 1
      // the Owner property of a CellButton is an index into this array
      public static Player[] Players = new Player[]
      {
         new Player() { Name = "Unowned", TileColor = new TileColor(Color.WhiteSmoke) },
         new Player() { Name = "Player 1", TileColor = new TileColor(Color.Blue), IsComputer = false },
         new Player() { Name = "Player 2", TileColor = new TileColor(Color.Red), IsComputer = true }
      };

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main()
      {
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new MainForm());
      }

   }

}
