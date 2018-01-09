/*
   GameLogic.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This module handles the computer AI and the rules of the game (knowing when someone wins or there's a draw).

   In a 4x4x4 board, there are 76 winning lines. These lines are defined as an array of cell references into
   the TileTacToe.Cells[,,] array. A 4x4x4 board will add these lines/cells to cellLines[76][4].

   While lines are built, each cell is given a weight equal to the number of lines it participates in.
   The inside 4 cells and the outer 4 corners are in 7 winning lines and are worth more than other cells that
   are only in 4 winning lines.

   When one of the lines is completely filled by a single owner, that owner won and the game is over.
   When all lines contain more than one owner, no lines can be won and the game is a draw.

   For the computer AI, each line is given a weight based on who has cells in the line and how many.
   Unwinnable lines (where more than one player has a cell) are skipped. For the remaining lines,
   weights are given in this priority, from best to worst:

      Hard Difficulty:                             Easy Difficulty:
      -------------------------------------        -------------------------------------
      Computer has three cells in a line           Computer has three cells in a line
      Opponent has three cells in a line           Either player has more than one cell in a line
      Either player has two cells in a line        Either player has one cell in a line
      Computer has one cell in a line              A line is empty with no cell taken
      A line is empty with no cell taken
      Opponent has one cell in a line

   When choosing a cell, the computer will pick the line with the best weight, and then the cell within that
   line with the best weight. The lines and cells are randomized so it doesn't always pick 0,0,0 first.

   Note: this logic works with any value of NUM_PLAYERS but assumes a BOARD_SIZE of 4 (4x4x4).
   If a different BOARD_SIZE is used, the weights will need adjusted to make the computer "intelligent".

   Public methods:
      BuildCellLines(): populates cellLines with all possible winning lines
      bool CheckForGameOver(): Returns true if the game is over (win or a draw)
      CellButton[] GetWinningLine(): Returns the CellButtons that make up the line that won
      CellButton ChooseACell(): Returns the CellButton the computer should play next
*/

using System;
using System.Linq;

namespace TileTacToe
{

   public static class GameLogic
   {
      // the game's "AI" centers around winnable lines of cells. these lines are stored in a List with
      // a small array of cell references (all BOARD_SIZE in length) for each list element
      private static CellButton[][] cellLines = new CellButton[TileTacToe.CELL_LINES][];

      // for some randomness in computer AI, so it's not always picking cell 0,0,0 first and making the same moves
      private static Random randomizer = new Random();

      // called from MainForm after Cells[,,] are instantiated (since cells in the lines are references within that array)
      public static void BuildCellLines()
      {
         // from wikipedia: en.wikipedia.org/wiki/3D_tic-tac-toe
         // "There are 76 winning lines. On each of the four 4x4 boards, or horizontal planes, there are four columns,
         // four rows, and two diagonals, accounting for 40 lines. There are 16 vertical lines, each ascending from
         // a cell on the bottom board through the corresponding cells on the other boards. There are eight vertically-
         // oriented planes parallel to the sides of the boards, each of these adding two more diagonals (the horizontal
         // and vertical lines of these planes have already been counted). Finally, there are two vertically-oriented
         // planes that include the diagonal lines of the 4x4 boards, and each of these contributes two more diagonal
         // lines—each of these including two corners and two internal cells."

         // this function builds all lines mentioned above. each list item is an array of GameInfo.Cell references.
         // (this also supports any sized board, but comments will assume a 4x4x4 board)
         // also, as cells are added, their weights are incremented so its Weight is equal to the number of its lines
         // this function builds all 76 lines mentioned above. each line is an array of GameInfo.Cell references.

         int size = TileTacToe.BOARD_SIZE; // to save some typing
         int index = 0; // index into Lines, incremented after a line is complete

         // "On each of the four 4x4 boards, or horizontal planes, there are four columns,
         // four rows, and two diagonals, accounting for 40 lines."

         for (int z = 0; z < size; z++) // for each horizontal plane
            for (int x = 0; x < size; x++) // there are four columns
            {
               cellLines[index] = new CellButton[size];
               for (int y = 0; y < size; y++) // gather the column into a Lines entry
                  cellLines[index][y] = AddCell(x, y, z);
               index++;
            }

         for (int z = 0; z < size; z++) // for each horizontal plane
            for (int y = 0; y < size; y++) // there are four rows
            {
               cellLines[index] = new CellButton[size];
               for (int x = 0; x < size; x++) // gather the row into a Lines entry
                  cellLines[index][x] = AddCell(x, y, z);
               index++;
            }

         // two diagonals for each horizontal plane
         for (int z = 0; z < size; z++) // for each horizontal plane
         {
            // first diagonal from 0,0,z -> 1,1,z -> 2,2,z -> 3,3,z
            cellLines[index] = new CellButton[size];
            for (int i = 0; i < size; i++) // gather first diagonal into a Lines entry
               cellLines[index][i] = AddCell(i, i, z);
            index++;

            // second diagonal from 0,3,z -> 1,2,z -> 2,1,z -> 3,0,z
            cellLines[index] = new CellButton[size];
            for (int i = 0; i < size; i++) // gather second diagonal into a Lines entry
               cellLines[index][i] = AddCell(i, size - i - 1, z);
            index++;
         }

         // for a 4x4x4 board there are 40 lines at this point

         // "There are 16 vertical lines, each ascending from a cell on the bottom board through the
         // corresponding cells on the other boards."

         for (int x = 0; x < size; x++) // for each column
            for (int y = 0; y < size; y++) // and row
            {
               cellLines[index] = new CellButton[size];
               for (int z = 0; z < size; z++) // gather the vertical line into a Lines entry
                  cellLines[index][z] = AddCell(x, y, z);
               index++;
            }

         // for a 4x4x4 board there are 56 lines at this point

         // "There are eight vertically-oriented planes parallel to the sides of the boards, each of these
         // adding two more diagonals (the horizontal and vertical lines of these planes have already been counted)."

         // four vertical diagonals along each column
         for (int x = 0; x < size; x++)
         {
            // first diagonal from x,0,0 -> x,1,1 -> x,2,2 -> x,3,3
            cellLines[index] = new CellButton[size];
            for (int i = 0; i < size; i++) // gather first diagonal along each vertical column
               cellLines[index][i] = AddCell(x, i, i);
            index++;

            // second diagonal from x,0,3 -> x,1,2 -> x,2,1 -> x,3,0
            cellLines[index] = new CellButton[size];
            for (int i = 0; i < size; i++) // gather second diagonal along each vertical column
               cellLines[index][i] = AddCell(x, i, size - i - 1);
            index++;
         }

         // four vertical diagonals along each row
         for (int y = 0; y < size; y++)
         {
            // first diagonal from 0,y,0 -> 1,y,1 -> 2,y,2 -> 3,y,3
            cellLines[index] = new CellButton[size];
            for (int i = 0; i < size; i++)
               cellLines[index][i] = AddCell(i, y, i);
            index++;

            // second diagonal from 0,y,3 -> 1,y,2 -> 2,y,1 -> 3,y,0
            cellLines[index] = new CellButton[size];
            for (int i = 0; i < size; i++)
               cellLines[index][i] = AddCell(i, y, size - i - 1);
            index++;
         }

         // for a 4x4x4 board there are 72 lines at this point

         // "Finally, there are two vertically-oriented planes that include the diagonal lines of the
         // 4x4 boards, and each of these contributes two more diagonal lines—each of these including
         // two corners and two internal cells."

         // diagonal 0,0,0 -> 1,1,1 -> 2,2,2 -> 3,3,3
         cellLines[index] = new CellButton[size];
         for (int i = 0; i < size; i++)
            cellLines[index][i] = AddCell(i, i, i);
         index++;

         // diagonal 0,0,3 -> 1,1,2 -> 2,2,1 -> 3,3,0
         cellLines[index] = new CellButton[size];
         for (int i = 0; i < size; i++)
            cellLines[index][i] = AddCell(i, i, size - i - 1);
         index++;

         // diagonal 0,3,0 -> 1,2,1 -> 2,1,2 -> 3,0,3
         cellLines[index] = new CellButton[size];
         for (int i = 0; i < size; i++)
            cellLines[index][i] = AddCell(i, size - i - 1, i);
         index++;

         // diagonal 3,0,0 -> 2,1,1 -> 1,2,2 -> 0,3,3
         cellLines[index] = new CellButton[size];
         for (int i = 0; i < size; i++)
            cellLines[index][i] = AddCell(size - i - 1, i, i);
         index++;

         // done! at this point there are 76 lines for a 4x4x4 board
         // the four corner cells of the cube and the inner four are involved in 7 lines and have a Weight of 7
         // the rest of the cells are involved in 4 lines and have a Weight of 4

         // testing at other sizes:
         // for a 3x3x3 board there are 49 lines
         // for a 5x5x5 board there are 109 lines

      } // end BuildLines

      // used in BuildLines() when a cell is added to a line. This adds +1 to the weight of GameInfo.Cells[x,y,z]
      // and returns the cell reference
      private static CellButton AddCell(int x, int y, int z)
      {
         TileTacToe.Cells[x, y, z].Weight++; // every line that includes this cell increases its strategic value by 1
         return TileTacToe.Cells[x, y, z];
      }

      // this function takes a line and returns a LineStatus (IsWinnable, Owner, CellsTaken) to describe its state
      private static LineStatus GetLineStatus(CellButton[] line)
      {
         LineStatus status = new LineStatus();

         for (int i = 0; i < line.Length; i++)
         {
            if (status.Owner == 0 && line[i].Owner > 0) // if this is the first unowned cell,
            {
               status.Owner = line[i].Owner; // save owner in status
               status.CellsTaken++; // and increment count of cells taken
            }
            else if (status.Owner > 0 && line[i].Owner != 0) // if line already has an owned cell and this cell is also owned
            {
               if (line[i].Owner != status.Owner) // if this cell's owner is different than line's existing owner,
               {
                  status.IsWinnable = false; // then this line is unwinnable
                  return status; // don't care about the rest, return status immediately
               }
               status.CellsTaken++; // add one to cells taken
            }
         }

         return status;
      }

      // this returns a numerical weight or value for a line of cells, to choose which line for the computer to play
      // it assumes TileTacToe.CurrentTurn is the current player and any other players are opponents
      private static int GetLineWeight(CellButton[] line)
      {
         int majorWeight = 0; // this is how important it is to place a cell on the line
         int minorWeight = 0; // adjusted by how important the line is strategically (the total weight of cells in the line)

         LineStatus status = GetLineStatus(line);

         bool isOpponents = (status.Owner != TileTacToe.CurrentTurn); // true if the line is occupied by an opponent

         if (status.IsWinnable) // only care about lines that can win; unwinnable lines can remain 0 weight
         {


            if (TileTacToe.Difficulty == 0) // easy difficulty
            {
               // in easy mode, line weights are a bit less intelligent

               if (status.CellsTaken == 3 && !isOpponents) // if computer has three cells in a line, highest priority to win
                  majorWeight = 6;
               else if (status.CellsTaken > 1) // otherwise randomly pick any line with more than one owned cell
                  majorWeight = 4;
               else if (status.CellsTaken == 1) // otherwise pick any line with just one owned cell
                  majorWeight = 2;
               else // empty lines take lowest priority in easy mode
                  majorWeight = 1;
            }
            else // hard difficulty
            {
               // easy difficulty exists because I've not won a game with the following weights!

               if (status.CellsTaken == 3 && !isOpponents) // if computer has three cells in a line, highest priority to win
                  majorWeight = 6;
               else if (status.CellsTaken == 3) // if opponent has three cells in a line, block it
                  majorWeight = 5;
               else if (status.CellsTaken == 2) // if either player has two cells in a line, equal weight
                  majorWeight = 4;
               else if (status.CellsTaken == 1 && !isOpponents) // if computer has one cell in a line, claim it before following
                  majorWeight = 3;
               else if (status.CellsTaken == 0) // if the line is empty
                  majorWeight = 2;
               else // lastly, if the opponent has one cell in a line, can ignore it for now
                  majorWeight = 1;
            }

            majorWeight *= 100; // multiply major weight by 100, minor weight will take up lower two digits

            // add up minorWeight which is the sum of the cell's weights to break ties when two lines have equal weight
            foreach (var cell in line)
               minorWeight += cell.Weight;
         }

         // a total weight is majorWeight + minorWeight
         return majorWeight + minorWeight;
      }

      // this checks each line's status and returns true if there's a winner or a draw
      // the discovery of a winner or a draw will set the GameInfo.Mode to GameMode.WINNER or GameMode.DRAW
      public static bool CheckForGameOver()
      {
         // check for a winning line and return true if one is found
         if (GetWinningLine() != null)
            return true;

         // now check for any winnable lines, if one is found game is not over
         foreach (var line in cellLines)
         {
            if (GetLineStatus(line).IsWinnable)
               return false; // found a winnable line, so the game isn't over yet
         }

         // if we reached here, there are no more winnable lines. the game is a draw
         return true;
      }

      // looks through all cellLines to see if any are completely filled, and returns the line if so, null otherwise
      public static CellButton[] GetWinningLine()
      {
         foreach (var line in cellLines)
         {
            LineStatus lineStatus = GetLineStatus(line);
            if (lineStatus.CellsTaken == TileTacToe.BOARD_SIZE)
               return line; // all cells in the line were taken, this must be the winning one
         }
         return null; // no winning line was found
      }

      // this chooses a cell for the computer to play based on the line and cell weights
      public static CellButton ChooseACell()
      {
         // as we get the weight of each line, these two values will update to the best choice so far
         int bestLineIndex = 0;
         int bestLineWeight = 0;

         // this scrambles the order of the Lines array, so the computer won't keep picking the lowest indexes
         // this bit of magic is from stackoverflow.com/questions/108819/best-way-to-randomize-an-array-with-net
         cellLines = cellLines.OrderBy(x => randomizer.Next()).ToArray();

         for (int i = 0; i < cellLines.Length; i++)
         {
            int weight = GetLineWeight(cellLines[i]);
            if (weight > bestLineWeight) // found a line that has more weight then previously noted
            {
               bestLineIndex = i; // note this line as best for now
               bestLineWeight = weight; // and its weight
               cellLines[i] = cellLines[i].OrderBy(x => randomizer.Next()).ToArray(); // scramble the cells within the line too for randomness
            }
         }

         // at this point, bestLineIndex refers to the best line, now to choose a cell based on the cell's weight

         // as with lines above, update these values to the best choice
         int bestCellIndex = 0;
         int bestCellWeight = 0;

         for (int i = 0; i < cellLines[bestLineIndex].Length; i++)
         {
            if (cellLines[bestLineIndex][i].Owner == 0) // only choose an unowned cell
            {
               int weight = cellLines[bestLineIndex][i].Weight;
               if (weight > bestCellWeight) // found a cell that weighs more than previously noted
               {
                  bestCellIndex = i; // note this cell as best for now
                  bestCellWeight = weight; // and its weight
               }
            }
         }

         // at this point, the cell at Lines[bestLineIndex][bestCellIndex] is the winner
         // (remember this is a reference within Cells[,,,], a reference to the actual CellButton)
         return cellLines[bestLineIndex][bestCellIndex];
      }

   }

}
