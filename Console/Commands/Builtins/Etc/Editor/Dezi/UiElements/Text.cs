using dezi.Helper;
using dezi.Input;
using System;
using System.Collections.Generic;

namespace dezi.UiElements
{
    /// <summary>
    /// Can be used to display non-interactive text
    /// </summary>
    public class Text : UiElement
    {
        public string TextToDisplay { get; set; }

        public Text(string textToDisplay)
        {
            this.TextToDisplay = textToDisplay;
        }

        public Text(string textToDisplay, int height, int width)
            : this(textToDisplay)
        {
            this.Height = height;
            this.Width = width;
        }

        public Text(string textToDisplay, int height, int width, int yCoordinate, int xCoordinate)
            : this(textToDisplay, yCoordinate, xCoordinate)
        {
            this.CoordinateY = yCoordinate;
            this.CoordinateX = xCoordinate;
            this.Height = height;
            this.Width = width;
        }

        protected override void UpdateSubmoduleSizesAndPositions(int widthDelta, int heightDelta)
        {
        }

        public override void Render(IList<string> uiOutput)
        {
            int displayingHeight = (int)Math.Ceiling((decimal)this.TextToDisplay.Length / this.Width);
            for (int i = 0; i < Height && i < displayingHeight; i++)
            {
                string newSubstring;
                if (this.TextToDisplay.Length - (i * this.Width) > this.Width)
                {
                    newSubstring = this.TextToDisplay.Substring(i * this.Width, this.Width);
                }
                else
                {
                    newSubstring = this.TextToDisplay.Substring(i * this.Width, this.TextToDisplay.Length - (i * this.Width));
                }
                uiOutput[this.CoordinateY + i] = StringHelper.PutInNewStringAtIndex(uiOutput[this.CoordinateY + i], this.CoordinateX, newSubstring);
            }
        }

        public override void HandleInput(InputAction inputAction)
        {
            throw new NotImplementedException();
        }
    }
}
