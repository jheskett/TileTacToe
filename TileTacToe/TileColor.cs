/*
   TileColor.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   TileColor is a class to bundle a Color with lighter and darker versions of itself.
   
   Example use:
      TileColor tileColor = new TileColor(Color.Blue)

      Where a Color is ordinarily used (BackColor, Pens, Brushes, etc), use:
         tileColor.Normal to use the base color (passed when it was created) or
         tileColor.Darkest, Darker, Lighter or Lightest for shades of the same color

   Public properties:
      Normal (Color): The Color originally given when the TileColor was created
      Darkest (Color): A Color that's "50%" darker than the Normal color
      Darker (Color): A Color that's "25%" darker than the Normal color
      Lighter (Color): A Color that's "25%" lighter than the Normal color
      Lightest (Color): A Color that's "75%" lighter than the Normal color
   
   Notes:
      Comparing two TileColors (tileColor1==tileColor2) will return true if they
      share the same Normal color, not the same instance of a TileColor.
*/

using System.Drawing;

namespace TileTacToe
{
   public class TileColor
   {
      public Color Normal { get; private set; }
      public Color Darkest { get; private set; }
      public Color Lighter { get; private set; }
      public Color Darker { get; private set; }
      public Color Lightest { get; private set; }

      //
      // Constructor
      //

      // a Color (like Color.Blue) is required when a TileColor is created. The Lighter and
      // Darker values are defined at that time.
      public TileColor(Color baseColor)
      {
         // break apart baseColor to its RGB values
         int r = baseColor.R;
         int g = baseColor.G;
         int b = baseColor.B;

         // normal color is the one passed in the constructor (ie Color.Blue)
         Normal = baseColor;

         // darkest color subtracts 1/2 the difference between base r,g,b and 0
         Darkest = Color.FromArgb(255, r - (r * 1 / 2), g - (g * 1 / 2), b - (b * 1 / 2));

         // darker color subtracts 1/4th the difference between base r,g,b and 0
         Darker = Color.FromArgb(255, r - (r * 1 / 4), g - (g * 1 / 4), b - (b * 1 / 4));

         // lighter color adds 1/4th the difference between base r,g,b and 255
         Lighter = Color.FromArgb(255, (255 - r) * 1 / 4 + r, (255 - g) * 1 / 4 + g, (255 - b) * 1 / 4 + b);

         // lightest color adds 3/4th the difference between base r,g,b and 0
         Lightest = Color.FromArgb(255, (255 - r) * 3 / 4 + r, (255 - g) * 3 / 4 + g, (255 - b) * 3 / 4 + b);
      }

      //
      // Public methods
      //

      // for debugging purposes, returns r,g,b value of this color
      public override string ToString()
      {
         return string.Format("{0},{1},{2}", Normal.R, Normal.G, Normal.B);
      }

      //
      // == operator overload
      //

      // == compares the Normal property of two TileColors; it's true if the colors match, even for separate TileColor instances
      public static bool operator ==(TileColor obj1, TileColor obj2)
      {
         if (ReferenceEquals(obj1, obj2)) // if obj2 *is* obj1, they're equal
            return true;
         if (ReferenceEquals(obj1, null)) // if obj1 is null, not equal (obj2 is non-null)
            return false;
         if (ReferenceEquals(obj2, null)) // if obj2 is null, not equal (obj2 is non-null)
            return false;

         return (obj1.Normal == obj2.Normal); // return whether Normal colors of both TileColors are the same
      }

      // a == overload requires a != also
      public static bool operator !=(TileColor obj1, TileColor obj2)
      {
         return !(obj1 == obj2); // the obj1==obj2 bit will test for equality (and handle nulls)
      }

      // and an .Equals override
      public override bool Equals(object obj)
      {
         if (ReferenceEquals(this, null))
            return false;
         if (ReferenceEquals(obj, null))
            return false;

         return Normal.Equals((obj as TileColor).Normal);
      }

      // recommended: the hash code should reference the Normal color (if they match it's the same) and
      // not the object reference (which would never be the same for two separate instances of the same color)
      public override int GetHashCode()
      {
         return Normal.GetHashCode();
      }

   }
}
