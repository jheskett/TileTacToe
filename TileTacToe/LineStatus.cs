/*
   LineStatus.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is a small class to bundle the status of a CellButton[] line into one value.

   Public properties:
      IsWinnable (bool): Whether the line is has at most one owner claiming a cell within it
      Owner (int): The numeric owner of the line if it's winnable
      CellsTaken (int): The number of cells in the line the owner occupies
*/

namespace TileTacToe
{
   class LineStatus
   {

      public bool IsWinnable { get; set; } // if more than one player has a cell in this line, it's unwinnable
      public int Owner { get; set; } // the first player occupying a line (0=none, 1=player 1, 2=player 2, etc)
      public int CellsTaken { get; set; } // number of cells owned by the Owner

      // constructor
      public LineStatus()
      {
         IsWinnable = true; // assume a line is winnable until found otherwise
         Owner = 0; // assume line is empty
         CellsTaken = 0; // and no cells are taken
      }
   }
}
