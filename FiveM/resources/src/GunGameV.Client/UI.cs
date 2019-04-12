using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace GunGameV.Client
{
    public class Text : CitizenFX.Core.UI.Text //Inherits from CitizenFX.Core.UI.Text
    {
        public Text() : base("", PointF.Empty, 0f) //Called when a new instance of the Text class is created
        {

        }

        public override void Draw() //Function overrides the original function in CitizenFX.Core.UI.Text
        {
            if (!Enabled) //Check if the class is enabled
            {
                return; //If not enabled then cancel function from going any futher
            }

            float x = Position.X / 1920; //Get x position
            float y = Position.Y / 1080; //Get y position
            float w = WrapWidth / 1920; //Get text wrapping width

            if (Shadow) API.SetTextDropShadow(); //If Shadow then add a drop shadow to the text about to be drawn
            if (Outline) API.SetTextOutline(); //If Outline then add a outline to the text about to be drawn

            API.SetTextFont((int)Font); //Set the text font
            API.SetTextScale(Scale, Scale); //Set the scale of the text
            API.SetTextColour(Color.R, Color.G, Color.B, Color.A); //Set the colour of the text
            API.SetTextJustification((int)Alignment); //Set the text alignment

            if (WrapWidth > 0.0f) //Check if wrapping text
            {
                switch (Alignment) //Wrap by text alignment
                {
                    case CitizenFX.Core.UI.Alignment.Center: //If alignment is centeredd
                        API.SetTextWrap(x - (w / 2), x + (w / 2)); //Set the text wrap
                        break;
                    case CitizenFX.Core.UI.Alignment.Left: //If alignment is left aligned
                        API.SetTextWrap(x, x + w); //Set the text wrap
                        break;
                    case CitizenFX.Core.UI.Alignment.Right: //If alignment is right aligned
                        API.SetTextWrap(x - w, x); //Set the text wrap
                        break;
                }
            }
            else if (Alignment == CitizenFX.Core.UI.Alignment.Right) //If not wrapping text and right aligned
            {
                API.SetTextWrap(0.0f, x); //Set the text wrap
            }

            API.BeginTextCommandDisplayText("CELL_EMAIL_BCON"); //Send an event to the game letting it know we are going to begin adding text to draw

            if (Caption.Length > 99) //Check if the length of the caption is greater than the 99 character limit for drawing text
            {
                string[] strings = CitizenFX.Core.UI.Screen.StringToArray(Caption); //Split the string into an array of strings with a max length of 99 characters

                foreach (string s in strings) //Loop through each string in the array
                {
                    API.AddTextComponentSubstringPlayerName(s); //Tell the game to add this to the list of text to draw on screen
                }
            } else //If the length of the caption isn't greater than 99 then
            {
                API.AddTextComponentSubstringPlayerName(Caption); //Tell the game to add this to the list of text to draw on screen
            }

            API.EndTextCommandDisplayText(x, y); //Draw the text on the screen at the x and y position
        }
    }
    public class Rect : CitizenFX.Core.UI.Rectangle //Inherits from CitizenFX.Core.UI.Rectangle
    {
        public Rect() : base() //Called when a new instance of the Rect class is created
        {

        }

        public override void Draw() //Function overrides the original function in CitizenFX.Core.UI.Rectangle
        {
            if (!Enabled) //Check if the class is enabled
            {
                return; //If not enabled then cancel function from going any futher
            }

            float w = Size.Width / 1920; //Get width
            float h = Size.Height / 1080; //Get height
            float x = Position.X / 1920; //Get x position
            float y = Position.Y / 1080; //Get y position

            if (!Centered) //If not centered then
            {
                x += w * 0.5f; //Add the half the width to the x position
                y += h * 0.5f; //Add the half the height to the y position
            }

            API.DrawRect(x, y, w, h, Color.R, Color.G, Color.B, Color.A); //Draw the rectangle on the screen at the x and y position
        }
    }
}