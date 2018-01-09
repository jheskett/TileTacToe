/*
   ColorSwatchButton.cs
   Jeff Heskett
   CIS216 C# Final Program
   April 29, 2017

   This is a custom Button to use in the PlayerGroupBox to choose a color for each player.
   The buttons are actually PictureBoxes to avoid some default Button behaviors.
   
   Public properties:
      TileColor (TileColor): The color of the swatch
      Selected (bool): Whether the swatch is selected

   Notes:
      The parent is responsible for assigning a Click event handler to this button but does not
      need to handle any other mouse events; the button will take care of the rest.
      Swatches are built from three TileColor shades, depending on mouse and button state.
      If the swatch is Selected, it will draw a darker-color rectangle around the swatch.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace TileTacToe
{
   // using PictureBox as a base to avoid undesirable behavior of buttons, such as being selectable
   public class ColorSwatchButton : PictureBox
   {
      // all color swatches are this fixed size; this size encompasses the selecting rectangle around the swatch
      const int SWATCH_SIZE = 32;

      // properties without a public accessor
      private bool mouseDown = false;
      private bool mouseOver = false;

      // properties assigned public accessors later
      private TileColor tileColor; // this is the main color of the swatch (ie Color.Blue)
      private bool selected; // whether the swatch is chosen (dark border around it)

      // public accessor to get or set a swatch's color
      public TileColor TileColor
      {
         get
         {
            return tileColor;
         }
         set
         {
            tileColor = value;
            Invalidate(); // repaint: recolor swatch with new color
         }
      }

      // public accessor to define whether the swatch is selected
      public bool Selected
      {
         get
         {
            return selected;
         }
         set
         {
            selected = value;
            Invalidate(); // repaint: draw or remove darker rectangle around edge of swatch
         }
      }

      // constructor only sets a default size
      public ColorSwatchButton()
      {
         this.Size = new Size(SWATCH_SIZE, SWATCH_SIZE);
      }

      // Unlike CellButtons which change via BackColor only, ColorSwatchButtons are entirely painted by
      // the following code, so each visible change should be followed by an Invalidate() to redraw it

      // override of control's OnPaint
      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e); // call base control's OnPaint first

         if (TileColor != null) // only paint swatch if a color has been defined
         {
            Rectangle rect = this.ClientRectangle;

            // pen used to draw outside "Selected" border and accents around edge of inner swatch
            Pen pen = new Pen(TileColor.Darker, 3);

            // if swatch is selected, draw a dark 3px border around edge of control
            if (Selected)
               e.Graphics.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);

            // fill main portion of swatch with the normal brush, or the lighter color if mouse is over it
            SolidBrush brush = new SolidBrush(mouseOver ? TileColor.Lighter : TileColor.Normal);
            e.Graphics.FillRectangle(brush, 4, 4, rect.Width - 9, rect.Height - 9);
            brush.Dispose();

            // draw lighter color along top and right edge (or dark when mouse down to make it look pressed)
            pen.Color = mouseDown ? TileColor.Darker : TileColor.Lighter;
            e.Graphics.DrawLine(pen, 3, 4, rect.Width - 3, 4);
            e.Graphics.DrawLine(pen, rect.Width - 5, 4, rect.Width - 5, rect.Height - 3);

            // draw darker color along left and bottom edge (or light when mouse down to make it look pressed)
            pen.Color = mouseDown ? TileColor.Lighter : TileColor.Darker;
            e.Graphics.DrawLine(pen, 4, 4, 4, rect.Height - 5);
            e.Graphics.DrawLine(pen, 3, rect.Height - 5, rect.Width - 5, rect.Height - 5);

            pen.Dispose();
         }
      }

      // when the mouse goes down, the light/dark lines around button edge swap to dark on top/right, light on left/bottom
      protected override void OnMouseDown(MouseEventArgs e)
      {
         base.OnMouseDown(e);
         mouseDown = true;
         Invalidate();
      }

      // when the mouse goes up, the light/dark lines revert to natural state (light on top/right, dark on left/bottom)
      protected override void OnMouseUp(MouseEventArgs e)
      {
         base.OnMouseUp(e);
         mouseDown = false;
         Invalidate();
      }

      // when the mouse hovers over a swatch, the main fill area becomes the lighter color
      protected override void OnMouseEnter(EventArgs e)
      {
         base.OnMouseEnter(e);
         mouseOver = true;
         Invalidate();
      }

      // when the mouse stops hoving over a swatch, the main fill area reverts to its normal color
      protected override void OnMouseLeave(EventArgs e)
      {
         base.OnMouseLeave(e);
         mouseOver = false;
         Invalidate();
      }

   }

}
