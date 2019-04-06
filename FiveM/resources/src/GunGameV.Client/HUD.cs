using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using CitizenFX.Core.Native;

namespace GunGameV.Client
{
    public class HUD
    {
        private Text timerText, highscoreText, scoreText, statusText;
        private Rect timerRect, highscoreRect, scoreRect, timerOverlay, highscoreOverlay, scoreOverlay;
        private int highscore, score;

        public HUD()
        {
            timerText = new Text();
            timerText.Position = new PointF(960, 0);
            timerText.Centered = true;
            timerText.Scale = 0.5f;

            highscoreText = new Text();
            highscoreText.Position = new PointF(1072.5f, 0);
            highscoreText.Centered = true;
            highscoreText.Scale = 0.5f;

            scoreText = new Text();
            scoreText.Position = new PointF(847.5f, 0);
            scoreText.Centered = true;
            scoreText.Scale = 0.5f;

            statusText = new Text();
            statusText.Position = new PointF(960, 44);
            statusText.Centered = true;
            statusText.Scale = 0.3f;

            timerRect = new Rect();
            timerRect.Position = new PointF(885, -10);
            timerRect.Size = new SizeF(150, 60);
            timerRect.Color = Color.FromArgb(150, 0, 0, 0);

            highscoreRect = new Rect();
            highscoreRect.Position = new PointF(1035, -8);
            highscoreRect.Size = new SizeF(75, 54);
            highscoreRect.Color = Color.FromArgb(120, 0, 0, 0);

            scoreRect = new Rect();
            scoreRect.Position = new PointF(810, -8);
            scoreRect.Size = new SizeF(75, 54);
            scoreRect.Color = Color.FromArgb(120, 0, 0, 0);

            timerOverlay = new Rect();
            timerOverlay.Position = new PointF(885, 50);
            timerOverlay.Size = new SizeF(150, 18);
            timerOverlay.Color = Color.FromArgb(180, 0, 0, 0);

            highscoreOverlay = new Rect();
            highscoreOverlay.Position = new PointF(1035, 46);
            highscoreOverlay.Size = new SizeF(75, 12);
            highscoreOverlay.Color = Color.FromArgb(180, 255, 77, 77);

            scoreOverlay = new Rect();
            scoreOverlay.Position = new PointF(810, 46);
            scoreOverlay.Size = new SizeF(75, 12);
            scoreOverlay.Color = Color.FromArgb(180, 77, 255, 77);
        }

        private void UpdateStatus()
        {
            switch (Score.CompareTo(Highscore))
            {
                case 1:
                    statusText.Caption = "Winning";
                    statusText.Color = Color.FromArgb(255, 77, 255, 77);
                    break;
                case 0:
                    statusText.Caption = "Tied";
                    statusText.Color = Color.FromArgb(255, 77, 77, 255);
                    break;
                case -1:
                    statusText.Caption = "Losing";
                    statusText.Color = Color.FromArgb(255, 255, 77, 77);
                    break;
            }
        }

        public void Draw()
        {
            API.SetScriptGfxAlign(76, 84);
            API.SetScriptGfxAlignParams(0f, 0f, 0f, 0f);

            scoreRect.Draw();
            timerRect.Draw();
            highscoreRect.Draw();

            scoreOverlay.Draw();
            timerOverlay.Draw();
            highscoreOverlay.Draw();

            scoreText.Draw();
            timerText.Draw();
            highscoreText.Draw();
            statusText.Draw();

            API.ResetScriptGfxAlign();
        }

        public string Time { get => timerText.Caption; set => timerText.Caption = value; }
        public int Highscore
        {
            get => highscore;
            set
            {
                highscore = value;
                highscoreText.Caption = value.ToString();
                UpdateStatus();
            }
        }
        public int Score
        {
            get => score;
            set
            {
                score = value;
                scoreText.Caption = value.ToString();
                UpdateStatus();
            }
        }
    }
}
