using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputManager()
{

    private bool isRestartKeyBeingPressed;

    public bool shouldRestartGame;

    public TimeSpan lastClickTime;
    public TimeSpan nextClickAllowedTime;

    MouseState prevMouseState;
    MouseState currMouseState;

    GameTime gt;

    Card lastCardInteractedWith;
    Card cardBeingInteractedWith;

    Vector2 mouseOffsetOnClick;

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

        prevMouseState = currMouseState;
        currMouseState = Mouse.GetState();

        gt = gameTime;

        if(cardBeingInteractedWith != null)
        {
            Console.WriteLine($"Interacting with card {cardBeingInteractedWith.cardInfo}");
        }

    }

    public MouseState getMouseState()
    {
        return currMouseState;
    }

    public bool isLeftMouseButtonDown()
    {

        if(currMouseState.LeftButton == ButtonState.Pressed)
        {
            return true;
        }

        return false;
    }

    public bool isLeftMouseButtonReleased()
    {

        if(currMouseState.LeftButton == ButtonState.Released)
        {
            return true;
        }

        return false;
    }

    public bool isClickAllowed()
    {

        return gt.TotalGameTime > nextClickAllowedTime;

    }

    public bool isExitGameButtonDown()
    {
        //GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Constants.END_GAME_KEY);
        return Keyboard.GetState().IsKeyDown(Constants.END_GAME_KEY);
    }

    public void setClickCooldown()
    {

		nextClickAllowedTime = gt.TotalGameTime.Add(new TimeSpan(0,0,0,0,Constants.CLICK_DELAY));
    
    }

    public bool clickIsWithinDoubleClickTimeframe()
    {
        return gt.TotalGameTime < lastClickTime.Add(new TimeSpan(0,0,0,0,Constants.DOUBLE_CLICK_TOLERANCE));
    }

    public bool isMouseMoving()
    {
        return currMouseState.X == prevMouseState.X && currMouseState.Y == prevMouseState.Y;
    }

    public void processCardClick(Card card)
    {

        if(isClickAllowed())
        {

            if(isLeftMouseButtonDown() /*&& cardBeingInteractedWith == null*/)
            {

                if(prevMouseState.LeftButton != ButtonState.Pressed)
                {

                    cardBeingInteractedWith = card;
                    Console.WriteLine($"{card.cardInfo} clicked");

                    int cardOffsetY = currMouseState.Y - card.cardPos.Y;
					int cardOffsetX = currMouseState.X - card.cardPos.X;

                    mouseOffsetOnClick = new Vector2(cardOffsetX, cardOffsetY);

                    Console.WriteLine($"Mouse offset set to x = {cardOffsetX}, y = {cardOffsetY}");

                } else { // assume card is being moved

                }


            } else if(isLeftMouseButtonReleased() && cardBeingInteractedWith == card)
            {

                //check for moving

                if(card.isShowingFace)
                {

                    if(lastCardInteractedWith == card)
                    {

                        if(clickIsWithinDoubleClickTimeframe())
                        {

							Console.WriteLine($"{card.cardInfo} was double clicked");
                            
                            setClickCooldown();

                            card.callDoubleClickCallback();

                        } else {

                            lastCardInteractedWith = card;

                        }

                    } else {

                        lastCardInteractedWith = card;

                    }

                } else {

					Console.WriteLine($"Turned over card {card.cardInfo} clicked");

                    card.flipCard(true);

                    setClickCooldown();

                }

                cardBeingInteractedWith = null;
                lastCardInteractedWith = card;

            } else {

                cardBeingInteractedWith = null;

            }

        }

    }

}