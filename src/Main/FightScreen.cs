using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using MonoGame.Extended.Screens;
using NEA_Project.src.Battle;

namespace NEA_Project.src.Main
{
    public class FightScreen : GameScreen
    {
        //Create references to Game1
        private new Game Game => (Game)base.Game;
        public FightScreen(Game game, bool multiplayer = true) : base(game) { this.singleplayer = !multiplayer; }

        //Initialise variables for game content
        private SpriteBatch _spriteBatch;
        private Texture2D hudTexture;
        private Texture2D healthBackground;
        private Texture2D healthBar;
        Player player1;
        Player player2;
        List<Player> players = new();
        AI cpuPlayer; //The player being controlled by the AI
		SpriteFont fontFace = GameGlobals.SelectedFont;

        //Initialise variables for game rules
        private int roundTime = 60;
        private float timerTick = 1;
        private string conclusion;
        private bool singleplayer = true;

		public override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            RoundGlobals.SpriteBatch = _spriteBatch;

            //A dictionary containing all the spritesheets which the player uses to set up its animations
            var spriteDictionary = new Dictionary<string, Texture2D>
            {
                { "idle", Content.Load<Texture2D>("standing") },
                { "walk", Content.Load<Texture2D>("walk") },
                { "attacks", Content.Load<Texture2D>("attacks") },
                { "block", Content.Load<Texture2D>("block") },
				{ "stuns", Content.Load<Texture2D>("stuns") },
				{ "knockout", Content.Load<Texture2D>("knockout") }
			};

            var hitboxList = new List<Hitbox>();
            RoundGlobals.Hitboxes = hitboxList;

            var attackList = new List<Attack>();
            RoundGlobals.Attacks = attackList;

            //Creates the player, using "square" as its sprite before adding it to the "players" list
            player1 = new Player(spriteDictionary, 1);
            player2 = new Player(spriteDictionary, 2, !singleplayer);

            players.Add(player1);
            players.Add(player2);

            if (singleplayer)
            {
                cpuPlayer = new AI(player2, player1);
            }

            hudTexture = Content.Load<Texture2D>("healthDisplay");
			healthBackground = Content.Load<Texture2D>("healthBackground");
			healthBar = Content.Load<Texture2D>("health");

			var vfxList = new List<VFX>();
            RoundGlobals.Effects = vfxList;
			RoundGlobals.HitEffect = Content.Load<Texture2D>("hitFX");
		}

        public override void Update(GameTime gameTime)
        {
			//Tells player objects to update themselves.
			//We pass in the "players" list so the players can check for collisions using iteration.
			player1.Update(players);
			player2.Update(players);

			//Tells AI object to update itself similarly to how player objects are updated to register client inputs.
			if (cpuPlayer != null)
            {
                cpuPlayer.Update(players);
            }

			RoundGlobals.Update(gameTime);

			if (RoundGlobals.PlayingGame == true)
            {
                foreach (Hitbox listed in RoundGlobals.Hitboxes.ToList())
                {
                    listed.Update();
                }
                foreach (Attack listed in RoundGlobals.Attacks.ToList())
                {
                    listed.Update();
                }
				foreach (VFX effect in RoundGlobals.Effects.ToList())
				{
					effect.Update();
				}

				//Subtracts the game tick from the timerTick until it reaches zero.
				//A second will have passed when timerTick hits zero, so we decrease the round timer by one second.
				timerTick -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (timerTick <= 0 && roundTime > 0)
                {
                    roundTime -= 1;
					timerTick = 1;

					//A roundTime of zero means the complete length of the round has passed, so we need to declare a conclusion.
					if (roundTime == 0)
                    {
                        Game.Refresh();
                    }
                }
			}
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // Starts MonoGame's spritebatch feature, then draws the square sprite before stopping the spritebatch feature--
            // --the spriteBatch should only be active when it's needed, so it can be stopped when all the desired sprites are drawn.
            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.PointClamp);

            //Drawing heads-up display
			_spriteBatch.Draw(hudTexture, Vector2.Zero, sourceRectangle: null, color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.1f);
			_spriteBatch.Draw(healthBackground, Vector2.Zero, sourceRectangle: null, color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0f);
            _spriteBatch.Draw(healthBar, new Vector2(50, 46), sourceRectangle: new Rectangle(300, 40, (int)(((float)player1.Health / 100f) * 300f), 40), color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.2f);
			_spriteBatch.Draw(healthBar, new Vector2(750 - ((float)player2.Health / 100f) * 300f, 46), sourceRectangle: new Rectangle(300, 40, (int)(((float)player2.Health / 100f) * 300f), 40), color: Color.White, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.2f);
			_spriteBatch.DrawString(fontFace, roundTime.ToString(), GameGlobals.OffsetFromCentre(401, 70, roundTime.ToString()), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.2f);

			player1.Draw(); //Tells the player to draw itself while _spriteBatch is active
            player2.Draw();

            foreach (Hitbox listed in RoundGlobals.Hitboxes.ToList())
            {
                listed.Draw();
            }

			foreach (VFX effect in RoundGlobals.Effects.ToList())
			{
				effect.Draw();
			}

			//Here we declare the final state of the round.
			//Whoever has the highest health value wins, or the players draw if they're at equal health.
			if (RoundGlobals.PlayingGame == false)
            {
                if (player1.Health < player2.Health)
                {
                    conclusion = "Player 2 wins!";
                }
                else if (player2.Health < player1.Health)
				{
					conclusion = "Player 1 wins!";
				}
                else if (player1.Health == player2.Health) //Health values are equal, it's a draw
				{
					conclusion = "Draw!";
				}
				_spriteBatch.DrawString(fontFace, conclusion, GameGlobals.OffsetFromCentre(400, 120, conclusion), Color.Black, rotation: 0f, origin: Vector2.Zero, scale: 1f, SpriteEffects.None, layerDepth: 0.2f);
			}

            _spriteBatch.End();
        }
    }
}