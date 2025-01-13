

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputManager()
{

    private bool isRestartKeyBeingPressed;

    public bool shouldRestartGame;

    public TimeSpan lastClickTime;
    public TimeSpan nextClickAllowedTime;

    public Card lastCardClicked;

    public void Update(GameTime gameTime)
    {

        if(shouldRestartGame)
            shouldRestartGame = false;

        if (Keyboard.GetState().IsKeyDown(Constants.RESTART_GAME_KEY))
        {
            isRestartKeyBeingPressed = true;
        }
        
        if(isRestartKeyBeingPressed && Keyboard.GetState().IsKeyUp(Constants.RESTART_GAME_KEY))
        {
            isRestartKeyBeingPressed = false;
            shouldRestartGame = true;
        }

        if(isLeftMouseButtonReleased())
        {
            lastClickTime = gameTime.TotalGameTime;
        }


    }

    public MouseState GetMouseState()
    {
        return Mouse.GetState();
    }

    public bool isLeftMouseButtonDown()
    {
        if(Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
		    Console.WriteLine("LB down registered");
            return true;
        }

        return false;
    }

    public bool isLeftMouseButtonReleased()
    {
        if(Mouse.GetState().LeftButton == ButtonState.Released)
        {
            return true;
        }

        return false;
    }

    public bool isClickAllowed(GameTime gameTime)
    {

        return gameTime.TotalGameTime > nextClickAllowedTime;

    }

    public void setClickCooldown(GameTime gameTime)
    {

		nextClickAllowedTime = gameTime.TotalGameTime.Add(new TimeSpan(0,0,0,0,Constants.CLICK_DELAY));
    
    }

    public bool clickIsWithinDoubleClickTimeframe(GameTime gameTime)
    {
        return gameTime.TotalGameTime < lastClickTime.Add(new TimeSpan(0,0,0,0,Constants.CLICK_DELAY));
    }

}