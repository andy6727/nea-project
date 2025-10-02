using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NEA_Project.src.Battle;
using System.Collections.Generic;

namespace NEA_Project.src.Main
{
    internal class RoundGlobals
    {
        //These variables need to be accessed by different processes at varying times, so it's best to group all
        //of them under a class with public variables. Hence, "globals".
        public static float TotalSeconds { get; set; }
        public static ContentManager Content { get; set; }
        public static SpriteBatch SpriteBatch { get; set; }
        public static List<Hitbox> Hitboxes { get; set; }
        public static List<Attack> Attacks { get; set; }
		public static List<VFX> Effects { get; set; }
        public static Texture2D HitEffect { get ; set; }
		public static Game CurrentGame { get; set; }
        public static bool PlayingGame { get; set; }
        public static void Update(GameTime gametime)
        {
            TotalSeconds = (float)gametime.ElapsedGameTime.TotalSeconds;
        }
    }
}