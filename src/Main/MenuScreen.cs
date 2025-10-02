using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using NEA_Project.src.GUI;
using System;

namespace NEA_Project.src.Main
{
    internal class MenuScreen : GameScreen
    {
        //Create references to Game1
        private new Game Game => (Game)base.Game;
        private readonly UIManager _ui = new();
        public MenuScreen(Game game) : base(game)
        {
            _ui.AddButton(buttonType: null, new(290, 133)).OnClick += PlayClicked;
            _ui.AddButton(buttonType: null, new(290, 215)).OnClick += SettingsClicked;
            _ui.AddButton(buttonType: null, new(290, 297)).OnClick += ExitClicked;
        }

        //Initialise variables for game content
        private SpriteBatch _spriteBatch;
        SpriteFont fontFace = GameGlobals.SelectedFont;

        //Loads the font to be used on the menu screen
        public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        //Button event handlers
        public void PlayClicked(object sender, EventArgs e)
        {
            RoundGlobals.CurrentGame.LoadStartCall();
        }

        public void SettingsClicked(object sender, EventArgs e)
        {
            RoundGlobals.CurrentGame.LoadSettingsCall();
        }

        public void ExitClicked(object sender, EventArgs e)
        {
            RoundGlobals.CurrentGame.Exit();
        }

        public override void Update(GameTime gameTime)
        {
            _ui.Update();
        }

        //Draws the menu screen
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            if (fontFace == GameGlobals.PixelFont) {
				GameGlobals.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
			}
            else
            {
				GameGlobals.SpriteBatch.Begin(samplerState: SamplerState.AnisotropicClamp);
			}

            _ui.Draw();
            GameGlobals.SpriteBatch.DrawString(fontFace, "NEA Game Project", GameGlobals.OffsetFromCentre(400, 57, "NEA Game Project", 2f), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 2f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Play", GameGlobals.OffsetFromCentre(400, 164, "Play"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Settings", GameGlobals.OffsetFromCentre(400, 246, "Settings"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Exit", GameGlobals.OffsetFromCentre(400, 329, "Exit"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);

            GameGlobals.SpriteBatch.End();
        }
    }
}