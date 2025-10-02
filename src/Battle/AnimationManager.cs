using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace NEA_Project.src.Battle
{
    public class AnimationManager
    {
        private readonly Dictionary<object, Animation> _anims = new(); //Dictionary storing all the loaded animations as keys
        private object _lastKey; //Storing the last-used animation key

		//Adds a key-animation pair to the dictionary, then sets _lastKey to that animation
		//if it's currently nil.
		public void AddAnimation(object key, Animation animation)
        {
            _anims.Add(key, animation);
            _lastKey ??= key;
        }

        public void ResetAnimation(object key)
        {
            _anims[key].Stop();
            _anims[key].Reset();
        }

        //Update method taking in a key to request which animation to play.
        //If the key is valid, then its associated animation will play.
        //Otherwise, the last-played animation will stop.
        public void Update(object key)
        {
            if (_anims.TryGetValue(key, out Animation value))
            {
                _anims[key].Start();
                _anims[key].Update();
                _lastKey = key;
            }
            else
            {
               ResetAnimation(_lastKey);
            }
        }

        //Arguments that need to be used in the Animation class must first be passed
        //through the AnimationManager class.
        public void Draw(Vector2 position, int identity, Color shade)
        {
            _anims[_lastKey].Draw(position, identity, shade);
        }

		//Separate draw method specifically for hit visual effects
		public void DrawEffect(Vector2 position, float scale, SpriteEffects spriteEffect)
        {
            _anims[_lastKey].DrawEffect(position, scale, spriteEffect);
		}
    }
}
