using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace NEA_Project.src.Main
{
    public class Game : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
		private readonly ScreenManager _screenManager;
        private float refreshDelay;
		private bool started = false;

		public Game()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

			_screenManager = new ScreenManager();
			Components.Add(_screenManager);
			RoundGlobals.CurrentGame = this;
		}

		private void LoadMenu() //Loads the menu screen
		{
			_screenManager.LoadScreen(new MenuScreen(this), new FadeTransition(GraphicsDevice, Color.White, 0.35f));
		}

		private void LoadGame(bool multiplayer) //Starts a match
		{
			_screenManager.LoadScreen(new FightScreen(this, multiplayer), new FadeTransition(GraphicsDevice, Color.White, 0.35f));
            RoundGlobals.PlayingGame = true;
			started = true;
		}

		private void LoadSettings() //Loads the settings
		{
			_screenManager.LoadScreen(new SettingsScreen(this), new FadeTransition(GraphicsDevice, Color.White, 0.35f));
		}

		private void LoadStart() //Loads the game type selection menu
		{
			_screenManager.LoadScreen(new StartScreen(this), new FadeTransition(GraphicsDevice, Color.White, 0.35f));
		}

		//Refresh() is called when a player's health reaches zero, stopping the game.
		public void Refresh()
        {
			RoundGlobals.PlayingGame = false;
            refreshDelay = 1.5f;
		}

		//Allows buttons to start the game
		public void LoadGameCall(bool multiplayer)
		{
			LoadGame(multiplayer);
		}

		//Allows the return button in various menus to open the main menu
		public void LoadMenuCall()
		{
			LoadMenu();
		}

		//Allows the settings button in the main menu to open the settings screen
		public void LoadSettingsCall()
		{
			LoadSettings();
		}

		//Allows the play button in the main menu to open a screen asking the user for their choice of game type
		public void LoadStartCall()
		{
			LoadStart();
		}

		protected override void Initialize()
        {
            RoundGlobals.Content = Content;
			GameGlobals.Content = Content;

			base.Initialize();
            LoadMenu();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            RoundGlobals.SpriteBatch = _spriteBatch;
			GameGlobals.SpriteBatch = _spriteBatch;
			GameGlobals.SmoothFont = Content.Load<SpriteFont>("Smooth");
			GameGlobals.PixelFont = Content.Load<SpriteFont>("Pixel");
			GameGlobals.SelectedFont = Content.Load<SpriteFont>("Pixel");
		}

        protected override void Update(GameTime gameTime)
        {
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			//This timer starts when a game has ended, which is defined as
			//the "started" and "PlayingGame" variables being true and false respectively.
			//The timer lasts for one second, giving time for the "Player [...] wins!" message
			//to be seen by both players before returning to the main menu.
			if (started == true && RoundGlobals.PlayingGame == false)
            {
				if (refreshDelay > 0)
				{
					refreshDelay -= (float)gameTime.ElapsedGameTime.TotalSeconds;
				}
				else if (refreshDelay <= 0)
				{
					started = false;
					_screenManager.Dispose();
					LoadMenu();
				}
			}

			base.Update(gameTime);
			GameGlobals.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            base.Draw(gameTime);
        }
    }
}