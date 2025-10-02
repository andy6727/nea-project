using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NEA_Project.src.Main;
using System;
using System.Collections.ObjectModel;
using System.Security.Principal;

namespace NEA_Project.src.Battle
{
	internal class VFX
	{
		//Initialising variables
		private readonly AnimationManager _anims = new();
		private Vector2 _position;
		private float _scale;
		private float _lifeTime;
		private SpriteEffects _spriteEffect = SpriteEffects.None;

		//Constructor
		public VFX(Vector2 position, float scale = 1f)
		{
			_anims.AddAnimation("Impact", new(RoundGlobals.HitEffect, 30, 1, (float)1 / 60, 1));
			this._position = position;
			this._scale = scale;
			this._lifeTime = 0.5f;
			RoundGlobals.Effects.Add(this);

			Random variant = new Random(); //Uses randomness to determine the orientation of the effect sprite
			var selection = variant.Next(0, 3);
			if (selection == 0)
			{
				_spriteEffect = SpriteEffects.FlipHorizontally;
			}
			else if (selection == 1) {
				_spriteEffect = SpriteEffects.FlipVertically;
			}
		}

		//Update method running every game tick
		public void Update()
		{
			_anims.Update("Impact");

			//Prevents the effect from being drawn after it ends
			this._lifeTime -= RoundGlobals.TotalSeconds;
			if (this._lifeTime <= 0)
			{
				RoundGlobals.Effects.Remove(this);
			}
		}

		public void Draw()
		{
			if (RoundGlobals.PlayingGame)
			{
				_anims.DrawEffect(_position, _scale, _spriteEffect);
			}
		}
	}
}
