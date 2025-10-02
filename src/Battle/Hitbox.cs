using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using NEA_Project.src.Main;

namespace NEA_Project.src.Battle
{
    internal class Hitbox
    {
        //Initialising variables
        Vector2 size;
        Vector2 position;
        float duration; //How long the hitbox will be active for before it gets deleted
        List<Player> ignore = new(); //The player that won't be registered by this hitbox. Typically, this will be the attacker so they won't be hit by their own move.
        List<Player> entities; //List of entities that the hitbox will need to process & register
        Texture2D hitboxTexture = RoundGlobals.Content.Load<Texture2D>("hitbox");
        bool drawHitbox; //Whether or not the hitbox should be indicated with a red square
        bool active;
        string attackId;

        //Constructor
        public Hitbox(string attackUsed, Vector2 givenSize, Vector2 givenPosition, float givenDuration, List<Player> toProcess, Player givenPlayer, bool visible)
        {
            attackId = attackUsed;
            size = givenSize;
            position = givenPosition;
            duration = givenDuration;
            entities = toProcess;
            ignore.Add(givenPlayer);
            drawHitbox = visible;
            active = true;
        }

        //Returns the rectangle bounds of the hitbox
        private Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
            }
        }

        public void Draw()
        {
            if (drawHitbox == true)
            {
                RoundGlobals.SpriteBatch.Draw(hitboxTexture, Bounds, new Color(Color.Red, 0.5f));
            }
        }

        //Hitbox detection is updated every game tick
        public void Update()
        {
            if (active == true)
            {
                foreach (var entity in entities)
                {
                    if (Bounds.Intersects(entity.HitboxBounds) && !ignore.Contains(entity))
                    {
                        entity.Hit(attackId);
                        ignore.Add(entity);
                    }
                }
                duration -= RoundGlobals.TotalSeconds;

                //Deactivates and hides the hitbox when it's meant to expire, as described by duration (in seconds)
                if (duration <= 0)
                {
                    active = false;
                    RoundGlobals.Hitboxes.Remove(this);
                    drawHitbox = false;
                }
            }
        }
    }
}