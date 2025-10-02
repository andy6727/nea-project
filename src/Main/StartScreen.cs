using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Screens;
using NEA_Project.src.GUI;
using System;

namespace NEA_Project.src.Main
{
    internal class StartScreen : GameScreen
    {
        //Create references to Game1
        private new Game Game => (Game)base.Game;
        private readonly UIManager _ui = new();
        public StartScreen(Game game) : base(game)
        {
            _ui.AddButton(buttonType: null, new(290, 133)).OnClick += SingleplayerClicked;
            _ui.AddButton(buttonType: null, new(290, 215)).OnClick += MultiplayerClicked;
			_ui.AddButton(buttonType: null, new(290, 297)).OnClick += ReturnClicked;
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
        public void SingleplayerClicked(object sender, EventArgs e)
        {
            RoundGlobals.CurrentGame.LoadGameCall(multiplayer: false);
        }

        public void MultiplayerClicked(object sender, EventArgs e)
        {
            RoundGlobals.CurrentGame.LoadGameCall(multiplayer: true);
        }

		public void ReturnClicked(object sender, EventArgs e)
		{
			RoundGlobals.CurrentGame.LoadMenuCall();
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
            GameGlobals.SpriteBatch.DrawString(fontFace, "Choose a game type:", GameGlobals.OffsetFromCentre(400, 57, "Choose a game type:", 2f), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 2f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Singleplayer", GameGlobals.OffsetFromCentre(400, 164, "Singleplayer"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);
            GameGlobals.SpriteBatch.DrawString(fontFace, "Multiplayer", GameGlobals.OffsetFromCentre(400, 246, "Multiplayer"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);
			GameGlobals.SpriteBatch.DrawString(fontFace, "Return", GameGlobals.OffsetFromCentre(400, 329, "Return"), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);

			GameGlobals.SpriteBatch.End();
        }
    }
}