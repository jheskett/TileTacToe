/*
   Signature.cs
   Jeff Heskett
   CIS 289 C# Programming
   04-18-2017

   This module contains the Signature class for use in assignments.

   Changes:
      04/18/2017: Added support for non-console environments
      03/30/2017: Separated Signature and More into separate .cs files
*/

using System;

// This class defines a signature block to display in assignments.
public class Signature
{

   // public properties of the Signature
   public string Name { get; set; } // name of student
   public string Email { get; set; } // email address of student
   public string Course { get; set; } // course assignment is for
   public string Motto { get; set; } // (optional) quote or motto to display in block


   // constructor with defaults
   public Signature()
   {
      Name = "Jeff Heskett";
      Email = "jeffrey.heskett@maine.edu";
      Course = "CIS 289 Topics in CIS (C# Programming)";
      Motto = "\"There are no bugs, only undocumented features.\" - Unknown";
   }

   // this method writes the formatted signature block to the console
   public void WriteBlock()
   {
      Console.WriteLine(ToString());
   }

   // returns the signature formatted into a multi-line signature block
   public override string ToString()
   {
      string block; // the signature block returned

      try // for console only (this will throw an exception in a non-console environment)
      {
         // repeat '-' to span width of console, to be used at the top and bottom of signature
         string line = new string('-', Console.WindowWidth - 1); // -1 to prevent word wrap from reach end of line

         // block begins with the above line, followed by Name, Email and Course, each indented one space
         block = string.Format("{0}\n {1}\n {2}\n {3}", line, Name, Email, Course);
         // the motto is optional; append it to the block if it exists
         if (!string.IsNullOrEmpty(Motto))
            block += "\n " + Motto;
         // finish block with another line that spans console's width
         block += "\n" + line + "\n";

      }
      catch // non-console will return just the signature without the extra lines formatted above and below signature
      {
         block = string.Format("{0}\n{1}\n{2}", Name, Email, Course);
         if (!string.IsNullOrEmpty(Motto))
            block += "\n" + Motto;
      }

      return block;
   }

}
