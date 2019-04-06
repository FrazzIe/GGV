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
    public class Text : CitizenFX.Core.UI.Text
    {
        public Text() : base("", PointF.Empty, 0f)
        {

        }

        public override void Draw()
        {
            if (!Enabled)
            {
                return;
            }

            float x = Position.X / 1920;
            float y = Position.Y / 1080;
            float w = WrapWidth / 1920;

            if (Shadow) API.SetTextDropShadow();
            if (Outline) API.SetTextOutline();

            API.SetTextFont((int)Font);
            API.SetTextScale(Scale, Scale);
            API.SetTextColour(Color.R, Color.G, Color.B, Color.A);
            API.SetTextJustification((int)Alignment);

            if (WrapWidth > 0.0f)
            {
                switch (Alignment)
                {
                    case CitizenFX.Core.UI.Alignment.Center:
                        API.SetTextWrap(x - (w / 2), x + (w / 2));
                        break;
                    case CitizenFX.Core.UI.Alignment.Left:
                        API.SetTextWrap(x, x + w);
                        break;
                    case CitizenFX.Core.UI.Alignment.Right:
                        API.SetTextWrap(x - w, x);
                        break;
                }
            }
            else if (Alignment == CitizenFX.Core.UI.Alignment.Right)
            {
                API.SetTextWrap(0.0f, x);
            }

            API.BeginTextCommandDisplayText("CELL_EMAIL_BCON");

            if (Caption.Length > 99)
            {
                string[] strings = CitizenFX.Core.UI.Screen.StringToArray(Caption);

                foreach (string s in strings)
                {
                    API.AddTextComponentSubstringPlayerName(s);
                }
            } else
            {
                API.AddTextComponentSubstringPlayerName(Caption);
            }

            API.EndTextCommandDisplayText(x, y);
        }
    }
    public class Rect : CitizenFX.Core.UI.Rectangle
    {
        public Rect() : base()
        {

        }

        public override void Draw()
        {
            if (!Enabled)
            {
                return;
            }

            float w = Size.Width / 1920;
            float h = Size.Height / 1080;
            float x = Position.X / 1920;
            float y = Position.Y / 1080;

            if (!Centered)
            {
                x += w * 0.5f;
                y += h * 0.5f;
            }

            API.DrawRect(x, y, w, h, Color.R, Color.G, Color.B, Color.A);
        }
    }
}