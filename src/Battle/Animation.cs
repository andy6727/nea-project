using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NEA_Project.src.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NEA_Project.src.Battle;
public class Animation
{
    private readonly Texture2D _texture;
    private readonly List<Rectangle> _sourceRectangles = new(); //List storing the bounding rectangles of each individual frame in the spritesheet
    private readonly int _frames; //Which frame is the animation currently on?
    private int _frame; //Indexing the current frame
    private readonly float _frameTime; //Time between frame switching in seconds (e.g. 60FPS animation will have _frameTime ≈ 0.0167)
    private float _frameTimeLeft;
    private bool _active = false; //Switch to start/stop animations
    private bool _looping;

	//Constructor
	public Animation(Texture2D texture, int framesX, int framesY, float frameTime, int row = 1, bool loop = true)
    {
        _texture = texture;
        _frameTime = frameTime;
        _frameTimeLeft = _frameTime;
        _frames = framesX;
        _looping = loop;
		//We calculate the width and height of each frame in the spritesheet based on
		//the dimensions of the spritesheet and the number of frames on each row & column.
		var frameWidth = _texture.Width / framesX;
        var frameHeight = _texture.Height / framesY;

        //Adds the calculated bounding rectangles to _sourceRectangles
        for (int i = 0; i < _frames; i++)
        {
            _sourceRectangles.Add(new(i * frameWidth, (row - 1) * frameHeight, frameWidth, frameHeight));
        }
    }

    public void Stop()
    {
        _active = false;
    }

    public void Start()
    {
        _active = true;
    }

    public void Reset()
    {
        _frame = 0;
        _frameTimeLeft = _frameTime;
    }

    //Increments to the next frame so long as the animation is supposed to be playing.
    //By processing and comparing _frameTime and _frameTimeLeft, we can use MonoGame's gameTime class
    //to wait the correct amount of time before moving onto the next frame.
    public void Update()
    {
        if (!_active) return;

        _frameTimeLeft -= RoundGlobals.TotalSeconds;

        if (_frameTimeLeft <= 0)
        {
            _frameTimeLeft += _frameTime;

            //Progresses the animation onto the next frame.
            //The next frame can't be a higher number than the amount of frames the animation has,
            //so if it is, we return the animation to its first frame assuming it is a looped animation,
            //or stop progressing the animation altogether if it isn't supposed to loop.
			if (_frame + 1 < _frames)
			{
				_frame += 1; //Still within the frames limit for the animation, so we can just move to the next frame
			}
			else
			{
				if (_looping)
				{
					_frame = 0; //Animation is meant to loop; return to the first frame
				}
				else
				{
					_frame = _frames - 1; //Animation isn't meant to loop; maintain the last frame
				}
			}
		}
    }

    //Takes in a Vector2 to draw the frame at the correct position, i.e. for moving objects
    //It also takes the player identity so that we can flip player 2's sprites to mirror player 1.
    public void Draw(Vector2 pos, int player, Color shade)
    {
        if (_sourceRectangles.Count > _frame)
        {
		    if (player == 1)
            {
                RoundGlobals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame], shade, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 1);
            }
            else if (player == 2)
            {
                RoundGlobals.SpriteBatch.Draw(_texture, pos, _sourceRectangles[_frame], shade, 0, Vector2.Zero, Vector2.One, SpriteEffects.FlipHorizontally, 1);
            }
		}
	}

    //Separate draw method specifically for hit visual effects
    public void DrawEffect(Vector2 pos, float scale, SpriteEffects spriteEffect)
    {
		RoundGlobals.SpriteBatch.Draw(_texture, new Vector2(pos.X + (128 * scale/2), pos.Y + (128 * scale/2)), _sourceRectangles[_frame], Color.Black, 0, new Vector2(64, 64), scale, spriteEffect, 1);
	}
}