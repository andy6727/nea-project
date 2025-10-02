using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NEA_Project.src.Main;
using System;
using System.Collections.Generic;

namespace NEA_Project.src.Battle
{
    internal class Player
    {
        //Initialising variables
        int identity; //Used to assign P1 and P2; identity 1 = player 1, identity 2 = player 2
        private bool clientControlled; //Is true if the player is human, false if controlled by AI
        private readonly AnimationManager _anims = new();
		private string playingAnimation = string.Empty;
		private float animDuration;

		//Using bools to create debounces for non-continuous actions, such as attacks.
		bool zDown = false;
        bool xDown = false;
        bool cDown = false;
        bool sDown = false;

        bool jDown = false;
        bool kDown = false;
        bool lDown = false;
        bool oDown = false;

        //Gameplay attributes
        private int health = 100;
        private bool blocking = false;
        private Color shade = Color.White;

        private bool stunned = false;
		private float stunDuration;

		private bool active = false; //Used to control player endlag
		private float endlagDuration;

		private float knockbackDuration;

		Vector2 position;
		Vector2 velocity;
		Vector2 knockback;
		float speed; //Measure of how much the position gets affected by the velocity

		//Constructor
		public Player(Dictionary<string, Texture2D> texture, int identity, bool isControllable = true)
        {
            this.identity = identity;
            speed = 1.5f;

            _anims.AddAnimation("Idle", new(texture["idle"], 60, 1, (float)1 / 60, 1));
            _anims.AddAnimation("Forwards", new(texture["walk"], 50, 2, (float)1 / 60, 1));
            _anims.AddAnimation("Backwards", new(texture["walk"], 50, 2, (float)1 / 60, 2));
            _anims.AddAnimation("Light", new(texture["attacks"], 30, 4, (float)1 / 60, 1));
            _anims.AddAnimation("Medium", new(texture["attacks"], 30, 4, (float)1 / 60, 2));
            _anims.AddAnimation("Heavy", new(texture["attacks"], 30, 4, (float)1 / 60, 3));
            _anims.AddAnimation("Block", new(texture["block"], 1, 1, (float)1 / 60, 1));
            _anims.AddAnimation("Grab", new(texture["attacks"], 30, 4, (float)1 / 60, 4));
			_anims.AddAnimation("Stun1", new(texture["stuns"], 30, 2, (float)1 / 60, 1, false));
			_anims.AddAnimation("Stun2", new(texture["stuns"], 30, 2, (float)1 / 60, 2, false));
			_anims.AddAnimation("Knockout", new(texture["knockout"], 47, 1, (float)1 / 60, 1, false));

			//Checks which of the two players is being constructed, positioning them and assigining animations as necessary
			if (identity == 1) //Player 1; position the character on the left
            {
                position = new Vector2(212, 200);
            }
            else if (identity == 2) //Player 2; position the character on the right
            {
                position = new Vector2(460, 200);
            }

            this.clientControlled = isControllable;
        }

        //Returns the rectangle bounds of the player
        public Rectangle CollisionBounds
        {
            get
            {
                return new Rectangle((int)position.X, (int)position.Y, 70, 115);
            }
        }

        //Player 2's hitbox bounds are pushed further rightwards to prevent player 1
        //from having a range advantage, which comes as a result of flipping the sprite horizontally.
        public Rectangle HitboxBounds
        {
            get
            {
                int offset = 0;
                if (identity == 2)
                {
                    offset = 64;
                }
                return new Rectangle((int)position.X + offset, (int)position.Y, 70, 115);
            }
        }

        //Attribute manipulation methods
        public int Health
        {
            get
            {
				return this.health;
            }
        }

        public int Identity
        {
            get
            {
                return this.identity;
            }
        }

        public Vector2 Position
        {
            get
            {
                return this.position;
            }
        }

		public Vector2 Velocity
		{
			get
			{
				return this.velocity;
			}
		}

        //The getters named "Is[...]" are primarily used for AI players, so that it can "make decisions" that adapt to the game state.
		public bool IsActive
		{
			get
			{
				return this.active;
			}
		}

		public bool IsBlocking
		{
			get
			{
				return this.blocking;
			}
		}

		public bool IsStunned
		{
			get
			{
				return this.stunned;
			}
		}

		public void SetVelocity(Vector2 velocityToSet)
        {
            this.velocity = velocityToSet;
        }

		public void TakeDamage(int dmg)
        {
            int result = this.health - dmg;

            if (result > 0)
            {
                this.health = result;
			}
            else if (result <= 0)
            {
                this.health = 0;
                this.PlayAnimation("Knockout", 2);
				RoundGlobals.CurrentGame.Refresh();
            }
        }

        private void TakeStun(float stunLength)
        {
            if (stunLength > 0)
            {
                this.blocking = false;
				this.stunned = true;
				this.stunDuration = stunLength;
                this.shade = new Color(255, 89, 89); //Sets player colour to a light red to make it clear they are stunned
				Random variant = new Random(); //We have two stun animations, so we need to play one of them randomly
                this.PlayAnimation(string.Concat("Stun", variant.Next(1, 3)), stunLength);
			}
        }

        public void Endlag(float lagLength)
        {
			this.active = true;
			this.endlagDuration = lagLength;
        }

        private void TakeKnockback(Vector2 magnitude, float knockbackLength)
        {
            //Reverses knockback against player 1 so they are knocked left instead of right
            if (identity == 1)
            {
                magnitude *= -1;
            }
			this.knockback = magnitude;
			this.knockbackDuration = knockbackLength;
        }

        //Hit method checks through all the gameplay attributes and selects whether or not
        //the player should lose health, get stunned, etc.
        public void Hit(string hitBy)
        {
            int toDeal = 0;
            float toStun = 0;
            Vector2 toKnockback = new Vector2(0, 0);
            if (hitBy == "Light")
            {
                toDeal = 10;
                toStun = 0.2f;
                toKnockback = new Vector2(3, 0);
				VFX hitEffect = new(position + new Vector2(0, 16), 0.4f);
			}
            else if (hitBy == "Medium")
            {
                toDeal = 14;
                toStun = 0.3f;
                toKnockback = new Vector2(5, 0);
				VFX hitEffect = new(position, 0.7f);
			}
            else if (hitBy == "Heavy")
            {
                toDeal = 20;
                toStun = 0.5f;
                toKnockback = new Vector2(8, 0);
				VFX hitEffect = new(position);
			}
            else if (hitBy == "Grab")
            {
                toDeal = 10;
                toStun = 1f;
				VFX hitEffect = new(position, 0.7f);
			}

            //Reduces the damage and knockback of the incoming attack, unless that attack is a grab
            if (this.blocking == true && hitBy != "Grab")
            {
                toDeal = (int)Math.Round(toDeal * 0.2, 0);
                if (toStun > 0)
                {
                    TakeKnockback(toKnockback * 5, toStun * 0.1f);
                }
            }
			
			TakeDamage(toDeal);
            TakeStun(toStun);
            if (toStun > 0)
            {
                TakeKnockback(toKnockback, toStun);
            }
        }

        public void PlayAnimation(string key, float animLength)
        {
			//K.O. animations should have higher priorities than every other animation,
            //so we prevent the code from being executed if a K.O. animation is playing.
			if (this.playingAnimation != "Knockout") 
            {
				this.playingAnimation = key;
				this.animDuration = animLength;
			}
        }

        public void useAttack(string attackType, List<Player>filter)
        {
            Attack attackObject = new(attackType, this, filter, position);
        }

        //Update method running every game tick
        public void Update(List<Player> collidables)
        {
            if (this.stunDuration > 0) //Decrease the stun counter each tick
            {
				this.stunDuration -= RoundGlobals.TotalSeconds;
                if (this.stunDuration <= 0)
                {
					this.stunned = false;
                    this.shade = Color.White; //Resets player colour to white when they're no longer stunned
                }
            }
            if (this.endlagDuration > 0) //Decrease the active counter each tick
            {
				this.endlagDuration -= RoundGlobals.TotalSeconds;
                if (this.endlagDuration <= 0)
                {
					this.active = false;
                }
            }
            if (this.animDuration > 0) //Decrease the animation length counter each tick
            {
				this.animDuration -= RoundGlobals.TotalSeconds;
                if (this.animDuration <= 0)
                {
					_anims.ResetAnimation(playingAnimation);
					this.playingAnimation = string.Empty;
				}
            }
            if (this.knockbackDuration > 0)
            {
				this.knockbackDuration -= RoundGlobals.TotalSeconds;
                if (this.knockbackDuration <= 0)
                {
					this.knockback = Vector2.Zero;
                }
                if (this.knockback.X > 0)
                {
					this.knockback.X--;
                }
                else if (this.knockback.X < 0)
                {
					this.knockback.X++;
                }
            }

            if (this.stunned == false && this.active == false && RoundGlobals.PlayingGame == true)
            { //Prevent actions when the player is stunned or in endlag
              //Reads the state of the keyboard for movement and other actions
              //We use separate if statements instead of else ifs because otherwise whichever key is at the top of the selection statements (in this case the A key)
              //will be prioritised over the rest, which isn't the behaviour we want.
                if (identity == 1 && this.clientControlled) //Player 1 controls
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.A)) //Left
                    {
                        velocity.X -= 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.D)) //Right
                    {
                        velocity.X += 1;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Z) && zDown == false)
                    {
                        zDown = true;
                        useAttack("Light", collidables);
                    }
                    if (Keyboard.GetState().IsKeyUp(Keys.Z))
                    {
                        zDown = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.X) && xDown == false)
                    {
                        xDown = true;
						useAttack("Medium", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.X))
                    {
                        xDown = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.C) && cDown == false)
                    {
                        cDown = true;
						useAttack("Heavy", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.C))
                    {
                        cDown = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.S) && sDown == false)
                    {
                        sDown = true;
						useAttack("Grab", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.S))
                    {
                        sDown = false;
                    }
                }
                else if (identity == 2 && this.clientControlled) //Player 2 controls
                {
                    if (Keyboard.GetState().IsKeyDown(Keys.I)) //Left
                    {
                        velocity.X -= 1;
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.P)) //Right
                    {
                        velocity.X += 1;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.J) && jDown == false)
                    {
                        jDown = true;
						useAttack("Light", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.J))
                    {
                        jDown = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.K) && kDown == false)
                    {
                        kDown = true;
						useAttack("Medium", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.K))
                    {
                        kDown = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.L) && lDown == false)
                    {
                        lDown = true;
						useAttack("Heavy", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.L))
                    {
                        lDown = false;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.O) && oDown == false)
                    {
                        oDown = true;
						useAttack("Grab", collidables);
					}
                    if (Keyboard.GetState().IsKeyUp(Keys.O))
                    {
                        oDown = false;
                    }
                }

                if (knockbackDuration <= 0)
                {
                    position += velocity * speed;
                }
            }

            //Stops playing the continuous animations (walking, idle) when a manual action is performed.
            if (this.playingAnimation != string.Empty)
            {
                _anims.Update(this.playingAnimation);
            }
            else if (this.playingAnimation == string.Empty)
            {
                //Update movement animations based on velocity
                if (velocity.X > 0) //Travelling to the right of screen
                {
                    if (identity == 1)
                    {
                        _anims.Update("Forwards");
                        this.blocking = false;
					}
                    else if (identity == 2)
                    {
                        _anims.Update("Backwards");
                        this.blocking = true;
                    }
                }
                else if (velocity.X < 0) //Travelling to the left of screen
                {
                    if (identity == 1)
                    {
                        _anims.Update("Backwards");
                        this.blocking = true;
                    }
                    else if (identity == 2)
                    {
                        _anims.Update("Forwards");
                        this.blocking = false;
					}
                }
                else if (velocity.X == 0)
                {
                    _anims.Update("Idle");
					this.blocking = false;
				}
            }

            position += knockback;

            //Negates the positional change if the player would leave the game screen
            if (position.X < -32 | position.X > 704)
            {
                position -= velocity * speed;
                position -= knockback;
            }

            //Checks the "collidables" list, passed in as an argument via Game.Update().
            //Each item's bounds are checked to see if they intersect with the player's bounds.
            //If they do, the positional change is negated to prevent the item from moving further.
            //In this case, the items will always be the players.
            foreach (var item in collidables)
            {
                if (item != this && item.CollisionBounds.Intersects(CollisionBounds))
                {
                    position -= velocity * speed;
                }

                if (item != this)
                {
                    float positionDifference = Math.Abs(position.X - item.position.X);
                    if (this.blocking == true && item.active == true && positionDifference <= 150)
                    {
                        position -= velocity * speed;
                        _anims.Update("Block");
                    }
                }
            }
            velocity = Vector2.Zero; //Stops velocity if none of the movement keys are being pressed down; removes inertia
        }

        //Calls the draw method of the currently-playing animation
        public void Draw()
        {
            _anims.Draw(position, identity, shade);
        }
    }
}