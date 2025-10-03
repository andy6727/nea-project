# Simple 2D Fighting Game Made With MonoGame
An A-level Computer Science NEA ("non-examined assessment") project programmed in C#, using the [MonoGame](https://monogame.net/) framework and the [MonoGame.Extended](https://www.monogameextended.net/docs/about/introduction/) library. Visual Studio was used to write, test and debug the code. The game features singleplayer and local multiplayer modes.

![Preview image](https://github.com/andy6727/nea-project/blob/main/media/demo.png)

## Controls
A player wins by using attacks to reduce their opponent's health to zero. For both sides, attacks can be blocked (except for grabs) to reduce their damage by moving backwards relative to the centre of the screen. <br/>
Every fight has a 60-second timer. Once the timer hits zero, whichever player has the highest health is declared the victor. If both players have the same amount of health, the fight ends in a draw.

Player 1 (left side):
* A - Move left
* D - Move right
* Z - Light attack
* X - Medium attack
* C - Heavy attack
* S - Grab

Player 2 (right side):
* I - Move left
* P - Move right
* J - Light attack
* K - Medium attack
* L - Heavy attack
* O - Grab
