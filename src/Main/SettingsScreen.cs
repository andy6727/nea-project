using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using NEA_Project.src.GUI;
using System;

namespace NEA_Project.src.Main
{
    internal class SettingsScreen : GameScreen
    {
        //Create references to Game1
        private new Game Game => (Game)base.Game;
        private readonly UIManager _ui = new();
        public SettingsScreen(Game game) : base(game)
        {
            _ui.AddButton(buttonType: null, new(290, 297)).OnClick += ReturnClicked;
            _ui.AddButton(buttonType: "Slider", new(442, 145)).OnClick += FontChange;
        }

        //Initialise variables for game content
        private SpriteBatch _spriteBatch;
        SpriteFont fontFace = GameGlobals.SelectedFont;

        //Loads the font to be used on the menu screen
        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public void ReturnClicked(object sender, EventArgs e)
        {
            RoundGlobals.CurrentGame.LoadMenuCall();
        }

        public void FontChange(object sender, EventArgs e)
        {
            if (GameGlobals.SelectedFont == GameGlobals.SmoothFont)
            {
                GameGlobals.SelectedFont = GameGlobals.PixelFont;
            }
            else
            {
                GameGlobals.SelectedFont = GameGlobals.SmoothFont;
            }
            fontFace = GameGlobals.SelectedFont;
        }

        public override void Update(GameTime gameTime)
        {
            _ui.Update();
        }

        //Draws the menu screen
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

			if (fontFace == GameGlobals.PixelFont)
			{
				GameGlobals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
			}
			else
			{
				GameGlobals.SpriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
			}

			_ui.Draw();
            GameGlobals.SpriteBatch.DrawString(fontFace, "Settings", GameGlobals.OffsetFromCentre(400, 57, "Settings", 2f), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 2f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Smooth Font", GameGlobals.OffsetFromCentre(236, 165, "Smooth Font"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Return", GameGlobals.OffsetFromCentre(400, 329, "Return"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);

            GameGlobals.SpriteBatch.End();
        }
    }
}