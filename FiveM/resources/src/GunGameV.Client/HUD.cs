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
        private Text timerText, highscoreText, scoreText, statusText; //define Text variables
        private Rect timerRect, highscoreRect, scoreRect, timerOverlay, highscoreOverlay, scoreOverlay; //define Rect variables
        private int highscore, score; //define int variables

        public HUD()
        {
            timerText = new Text(); //Initialise class
            timerText.Position = new PointF(960, 0); //Set the x, y position
            timerText.Centered = true; //Centre the text
            timerText.Scale = 0.5f; //Set the scale of the text

            highscoreText = new Text(); //Initialise class
            highscoreText.Position = new PointF(1072.5f, 0); //Set the x, y position
            highscoreText.Centered = true; //Centre the text
            highscoreText.Scale = 0.5f; //Set the scale of the text

            scoreText = new Text(); //Initialise class
            scoreText.Position = new PointF(847.5f, 0); //Set the x, y position
            scoreText.Centered = true; //Centre the text
            scoreText.Scale = 0.5f; //Set the scale of the text

            statusText = new Text(); //Initialise class
            statusText.Position = new PointF(960, 44); //Set the x, y position
            statusText.Centered = true; //Centre the text
            statusText.Scale = 0.3f; //Set the scale of the text

            timerRect = new Rect(); //Initialise class
            timerRect.Position = new PointF(885, -10); //Set the x, y position
            timerRect.Size = new SizeF(150, 60); //Set the width, height of the rectangle
            timerRect.Color = Color.FromArgb(150, 0, 0, 0); //Set the colour of the rectangle

            highscoreRect = new Rect();
            highscoreRect.Position = new PointF(1035, -8); //Set the x, y position
            highscoreRect.Size = new SizeF(75, 54); //Set the width, height of the rectangle
            highscoreRect.Color = Color.FromArgb(120, 0, 0, 0); //Set the colour of the rectangle

            scoreRect = new Rect(); //Initialise class
            scoreRect.Position = new PointF(810, -8); //Set the x, y position
            scoreRect.Size = new SizeF(75, 54); //Set the width, height of the rectangle
            scoreRect.Color = Color.FromArgb(120, 0, 0, 0); //Set the colour of the rectangle

            timerOverlay = new Rect(); //Initialise class
            timerOverlay.Position = new PointF(885, 50); //Set the x, y position
            timerOverlay.Size = new SizeF(150, 18); //Set the width, height of the rectangle
            timerOverlay.Color = Color.FromArgb(180, 0, 0, 0); //Set the colour of the rectangle

            highscoreOverlay = new Rect(); //Initialise class
            highscoreOverlay.Position = new PointF(1035, 46); //Set the x, y position
            highscoreOverlay.Size = new SizeF(75, 12); //Set the width, height of the rectangle
            highscoreOverlay.Color = Color.FromArgb(180, 255, 77, 77); //Set the colour of the rectangle

            scoreOverlay = new Rect(); //Initialise class
            scoreOverlay.Position = new PointF(810, 46); //Set the x, y position
            scoreOverlay.Size = new SizeF(75, 12); //Set the width, height of the rectangle
            scoreOverlay.Color = Color.FromArgb(180, 77, 255, 77); //Set the colour of the rectangle
        }

        private void UpdateStatus() //Function that compares the users score with the highest other score in the match
        {
            switch (Score.CompareTo(Highscore)) //Compare score to highscore
            {
                case 1: //If user score is greater than then
                    statusText.Caption = "Winning"; //Set the caption
                    statusText.Color = Color.FromArgb(255, 77, 255, 77); //Set the colour
                    break;
                case 0: //If user score equals highscore then
                    statusText.Caption = "Tied"; //Set the caption
                    statusText.Color = Color.FromArgb(255, 77, 77, 255); //Set the colour
                    break;
                case -1: //If user score is less than then
                    statusText.Caption = "Losing"; //Set the caption
                    statusText.Color = Color.FromArgb(255, 255, 77, 77); //Set the colour
                    break;
            }
        }

        public void Draw() //Function that draws the HUD on the screen
        {
            API.SetScriptGfxAlign(76, 84); //Aligns the HUD with the left aligned safezone
            API.SetScriptGfxAlignParams(0f, 0f, 0f, 0f);

            scoreRect.Draw(); //Draw rectangle
            timerRect.Draw(); //Draw rectangle
            highscoreRect.Draw(); //Draw rectangle

            scoreOverlay.Draw(); //Draw rectangle
            timerOverlay.Draw(); //Draw rectangle
            highscoreOverlay.Draw(); //Draw rectangle

            scoreText.Draw(); //Draw text
            timerText.Draw(); //Draw text
            highscoreText.Draw(); //Draw text
            statusText.Draw(); //Draw text

            API.ResetScriptGfxAlign(); //End left align of the HUD
        }

        public string Time { get => timerText.Caption; set => timerText.Caption = value; } //Class property that gets or sets the caption of the timerText
        public int Highscore //Class property that gets or sets the highscore
        {
            get => highscore; //Returns the highscore
            set
            {
                highscore = value; //Sets the highscore
                highscoreText.Caption = value.ToString(); //Sets the highscore caption
                UpdateStatus(); //Updates the status of the match
            }
        }
        public int Score //Class property that gets or sets the score
        {
            get => score; //Returns the score
            set
            {
                score = value; //Sets the score
                scoreText.Caption = value.ToString(); //Sets the score caption
                UpdateStatus(); //Updates the status of the match
            }
        }
    }
}
