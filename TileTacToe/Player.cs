/*
   Player.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   Player is a class to define one of the human or computer players in the game.
   
   Public properties:
      Name : string : The name of the player, probably "Player 1" or "Player 2" unless changed.
      TileColor : TileColor : The color chosen by the player as a TileColor
      IsComputer : bool : Whether the player is a computer (true) or a human (false).
 */

namespace TileTacToe
{
   public class Player
   {
      public string Name { get; set; } // name of the player
      public TileColor TileColor { get; set; } // tile color chosen by the player
      public bool IsComputer { get; set; } // whether the player is computer controlled

      // the string form of a Player is the player's Name
      public override string ToString()
      {
         return Name;
      }
   }
}