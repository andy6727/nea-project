using Microsoft.Xna.Framework;
using NEA_Project.src.Main;
using System.Collections.Generic;

namespace NEA_Project.src.Battle
{
    internal class Attack
    {
        Vector2 size;
        Vector2 offset;
        float duration;
        string attackId;
        float delay;
        Player fromPlayer;
        List<Player> targets;
        Vector2 fromPosition;

        //Constructor is given a string id to decide what the values of the Attack object should be,
        //with the attacker, list of entities and the attacker's position being passed in as parameters.
        public Attack(string id, Player attacker, List<Player> enemies, Vector2 attackerPosition)
        {
            attackId = id;
            fromPlayer = attacker;
            targets = enemies;
            fromPosition = attackerPosition;
            if (id == "Light")
            {
                delay = 0.2f;
                size = new Vector2(70, 70);
                duration = 0.1f;
                offset = new Vector2(80, 10);

                attacker.Endlag(0.3f);
                attacker.PlayAnimation("Light", 0.3f);
                RoundGlobals.Attacks.Add(this);
            }
            else if (id == "Medium")
            {
                delay = 0.23f;
                size = new Vector2(70, 70);
                duration = 0.1f;
                offset = new Vector2(80, 20);

                attacker.Endlag(0.4f);
                attacker.PlayAnimation("Medium", 0.4f);
                RoundGlobals.Attacks.Add(this);
            }
            else if (id == "Heavy")
            {
                delay = 0.3f;
                size = new Vector2(70, 70);
                duration = 0.1f;
                offset = new Vector2(80, 20);

                attacker.Endlag(0.46f);
                attacker.PlayAnimation("Heavy", 0.46f);
                RoundGlobals.Attacks.Add(this);
            }
            else if (id == "Grab")
            {
                delay = 0.16f;
                size = new Vector2(80, 80);
                duration = 0.1f;
                offset = new Vector2(80, 40);

                attacker.Endlag(0.46f);
                attacker.PlayAnimation("Grab", 0.46f);
                RoundGlobals.Attacks.Add(this);
            }
        }

        //Creates and initialises hitboxes, passing in its own attributes which defines the properties
        //of those hitboxes.
        private void Activate(Player attacker, List<Player> enemies, Vector2 attackerPosition)
        {
            Vector2 centre = new Vector2(attacker.Position.X + 14, attacker.Position.Y + 14);
            if (attacker.Identity == 2)
            {
                centre = new Vector2(attacker.Position.X + 64, attacker.Position.Y + 14);
                offset.X *= -1;
            }

            Hitbox hitbox = new Hitbox(
                attackId,
                size,
                new Vector2(centre.X + offset.X, centre.Y + offset.Y),
                duration,
                enemies,
                attacker,
                false
                );
            RoundGlobals.Hitboxes.Add(hitbox);
        }

        public void Update()
        {
            if (delay > 0) //Decrease the delay counter each tick
            {
                delay -= RoundGlobals.TotalSeconds;
                if (delay <= 0)
                {
                    Activate(fromPlayer, targets, fromPosition);
                    RoundGlobals.Attacks.Remove(this);
                }
            }
        }
    }
}