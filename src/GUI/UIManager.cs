using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NEA_Project.src.Main;
using System.Collections.Generic;

namespace NEA_Project.src.GUI
{
    public class UIManager
    {
        private Texture2D ButtonTexture { get; }
        private Texture2D SliderTexture { get; }
        private readonly List<Button> _buttons = new();

        //Constructor that loads the various button textures for later use in the AddButton() method
        public UIManager()
        {
            ButtonTexture = GameGlobals.Content.Load<Texture2D>("button");
            SliderTexture = GameGlobals.Content.Load<Texture2D>("sliderOff");
            if (GameGlobals.SelectedFont == GameGlobals.PixelFont)
            {
                SliderTexture = GameGlobals.Content.Load<Texture2D>("sliderOn");
            }
        }

        //Public method to create buttons externally.
        //The appearance of the button is determined by the "buttonType" string.
        //All buttons are added to the _buttons list.
        public Button AddButton(string buttonType, Vector2 pos)
        {
            if (buttonType == "Slider")
            {
                Button b = new(SliderTexture, pos);
                _buttons.Add(b);

                return b;
            }
            else
            {
                Button b = new(ButtonTexture, pos);
                _buttons.Add(b);

                return b;
            }
        }

        //The update and draw methods iterate through the _buttons list, calling the appropriate method for each button instance.
        public void Update()
        {
            foreach (var item in _buttons)
            {
                item.Update();
            }
        }

        public void Draw()
        {
            foreach (var item in _buttons)
            {
                item.Draw();
            }
        }
    }
}
