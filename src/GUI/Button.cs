using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NEA_Project.src.Main;
using System;

namespace NEA_Project.src.GUI
{
    public class Button
    {
        private Texture2D _texture;
        private Vector2 _position;
        private readonly Rectangle _rect;
        private Color _shade = Color.White;

        //Constructor setting the appearance and position of the button
        public Button(Texture2D t, Vector2 p)
        {
            _texture = t;
            _position = p;
            _rect = new((int)p.X, (int)p.Y, t.Width, t.Height);
        }

        //Update method which handles all of a button's functionalities
        public void Update()
        {
            //We use the Intersects() method to determine if the mouse is within the bounding box of the button,
            //and change the value of _shade to produce feedback to the user that clearly shows when clicking will activate a button.
            if (GameGlobals.MouseCursor.Intersects(_rect))
            {
                _shade = Color.Gray;
                //If the user clicks while their mouse is within a button, it'll invoke an event. The listener and purpose of the event varies case-by-case.
                if (GameGlobals.Clicked)
                {
                    Click();
                    //Here, we can determine whether or not a button is a slider by checking what texture it's currently using.
                    //The "sliderOff" and "sliderOn" textures are exclusive to sliders, so anything executed underneath the if statements will be related to sliders.
                    //In this case, it simply swaps the "sliderOff" and "sliderOn" textures to clearly demonstrate that the slider has moved.
                    if (_texture == GameGlobals.Content.Load<Texture2D>("sliderOff"))
                    {
                        _texture = GameGlobals.Content.Load<Texture2D>("sliderOn");
                    }
                    else if (_texture == GameGlobals.Content.Load<Texture2D>("sliderOn"))
                    {
                        _texture = GameGlobals.Content.Load<Texture2D>("sliderOff");
                    }
                }
            }
            //If the cursor is not intersecting the button's bounds, then that can only mean it's outside of the button.
            //Therefore we can just set the value of _shade back to white.
            else
            {
                _shade = Color.White;
            }
        }

        public event EventHandler OnClick;

        private void Click()
        {
            OnClick?.Invoke(this, EventArgs.Empty);
        }

        public void Draw()
        {
            GameGlobals.SpriteBatch.Draw(_texture, _position, sourceRectangle: null, _shade, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 1f);
        }
    }
}