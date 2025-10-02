using Microsoft.Xna.Framework;
using NEA_Project.src.Main;
using System;
using System.Collections.Generic;

namespace NEA_Project.src.Battle
{
    internal class AI
    {
        //Initialising variables
        private Player agent; //The player being controlled by the AI
        private Player opponent; //The player being controlled by the human
        private float debounce; //Add a delay for attacking
		private string travelPriority; //Used to force the AI to prioritise a certain movement direction
		private float priorityDuration; //Add a delay for movement direction changes
        private Random random = new Random();

        //Constructor
        public AI(Player toControl, Player toFight)
        {
            this.agent = toControl;
            this.opponent = toFight;
            this.debounce = 0f;
			this.travelPriority = "";
			this.priorityDuration = 0f;
        }

        private void selectAttack(List<Player> collidables)
        {
			//The AI will always grab by default.
			//If the player isn't blocking, it'll instead use a light/medium/heavy attack at random.
			//As such, if the player is blocking, then it will perform a grab because
			//nothing else is being assigned to the attackToUse variable.
			string attackToUse = "Grab";
			if (!this.opponent.IsBlocking)
			{
				int randomAttack = random.Next(1, 4);
				if (randomAttack == 1)
				{
					attackToUse = "Light";
				}
				else if (randomAttack == 2)
				{
					attackToUse = "Medium";
				}
				else if (randomAttack == 3)
				{
					attackToUse = "Heavy";
				}
			}
			this.agent.useAttack(attackToUse, collidables);
		}

		private void move(string direction, bool setPriority = false, float travelTime = 0f)
		{
			if (this.agent.IsActive | this.agent.IsStunned) { return; } //Prevents AI player from walking if it's stunned or using an attack

			//Sets movement direction priority (+duration) if the argument is true,
			//and if there isn't already a priority assigned.
			if (setPriority && this.travelPriority == "")
			{
				this.travelPriority = direction;
				this.priorityDuration = travelTime;
			}

			if (direction == "Forwards")
			{
				if (this.travelPriority == "Backwards") //Travels backwards instead if that's the priority direction
				{
					this.agent.SetVelocity(new Vector2(1, 0));
				}
				else
				{
					this.agent.SetVelocity(new Vector2(-1, 0));
				}
			}
			else if (direction == "Backwards")
			{
				if (this.travelPriority == "Forwards") //Travels forwards instead if that's the priority direction
				{
					this.agent.SetVelocity(new Vector2(-1, 0));
				}
				else
				{
					this.agent.SetVelocity(new Vector2(1, 0));
				}
			}
		}

		//Update method running every game tick
		public void Update(List<Player> collidables)
		{
			//Countdown for travel debounce
			if (this.priorityDuration > 0) {
				this.priorityDuration -= RoundGlobals.TotalSeconds;
			}
			else
			{
				this.travelPriority = "";
			}

			//We calculate the distance between the two player characters.
			//If the AI is too far away from its opponent, it will begin walking to catch up to them.
			float distance = (float)Math.Abs((this.opponent.Position - this.agent.Position).X);
			if (distance >= 85)
			{
				this.move("Forwards");
			}

			//If the AI is close enough, it will switch to using sensible close-range options (i.e. attacking & blocking)
			//We have a debounce to produce an artificial delay in its attacks, 
			//and we check for certain variables of the player that it's controlling
            //to make sure that attacking would be "allowed" as per the game rules.
			else
			{
				//Here we read for the human opponent's active attribute as it's a good gauge for whether or not they're using an attack.
				//If the opponent's active attribute is false, then the AI uses an attack.
				if (!opponent.IsActive)
				{
					this.debounce -= RoundGlobals.TotalSeconds;
					if (this.debounce <= 0 && !this.agent.IsActive && !this.agent.IsStunned)
					{
						this.debounce = 0.5f;
						selectAttack(collidables);
					}
				}

				//If that attribute is true, then the AI will randomly "decide" between blocking and attacking,
				//with a higher chance of blocking.
				else
				{
					int randomOption = random.Next(1, 4);
					if (randomOption == 1) //Chosen to block; walking backwards activates blocking
					{
						this.move("Backwards");
					}
					else if (randomOption == 2) //Chosen to block WITH priority; causes the AI to continue walking backwards for a short time
					{
						this.move("Backwards", true, 1f);
					}
					else if (randomOption == 3) //Chosen to attack
					{
						this.debounce -= RoundGlobals.TotalSeconds;
						if (this.debounce <= 0 && !this.agent.IsActive && !this.agent.IsStunned)
						{
							this.debounce = 0.5f;
							selectAttack(collidables);
						}
					}
				}
            }
        }
    }
}