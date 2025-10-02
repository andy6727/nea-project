using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NEA_Project.src.Main
{
    internal class GameGlobals
    {
        //These variables need to be accessed by different processes at varying times, so it's best to group all
        //of them under a class with public variables. Hence, "globals".
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static SpriteFont SmoothFont { get; set; }
        public static SpriteFont PixelFont { get; set; }
        public static SpriteFont SelectedFont { get; set; }
        public static MouseState MouseState { get; set; }
        public static MouseState LastMouseState { get; set; }
        public static bool Clicked { get; set; }
        public static Rectangle MouseCursor { get; set; }
        public static void Update(GameTime gametime)
        {
            LastMouseState = MouseState;
            MouseState = Mouse.GetState();

            Clicked = MouseState.LeftButton == ButtonState.Pressed && LastMouseState.LeftButton == ButtonState.Released;
            MouseCursor = new(MouseState.Position, new(1, 1));
        }

        //Returns a Vector2 value to be used in a DrawString() call such that the desired string is centred properly.
        //We do this by subtracting half the length of the string from the desired X location, and then subtracting
        //half the height of the string from the desired Y location.
        public static Vector2 OffsetFromCentre(float x, float y, string text, float scale = 1)
        {
            return new Vector2(x - ((SelectedFont.MeasureString(text).X * scale) / 2), y - ((SelectedFont.MeasureString(text).Y) / 2));
        }
    }
}