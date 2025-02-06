using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Solitaire_Cracked_;

public class InputManager()
{

    bool isRestartKeyBeingPressed;

    public bool shouldRestartGame { get; private set; }

    public TimeSpan lastClickTime { get; private set; }
    public TimeSpan nextClickAllowedTime { get; private set; }

    MouseState prevMouseState;
    MouseState currMouseState;

    GameTime gt;

    Card lastCardInteractedWith;
    Card cardBeingInteractedWith;

    Vector2 mouseOffsetOnClick;

    public delegate Card GetTopMostCardCallbackHandler(Vector2 mousePos);
    public event GetTopMostCardCallbackHandler getTopmostCardAtMousePos;

    public delegate CardStackBase GetCardStackAtMousePosCallbackHandler(Vector2 mousePos);
    public event GetCardStackAtMousePosCallbackHandler getCardStackAtMousePos;

    public delegate void DrawPileClickCallbackHandler();
    public event DrawPileClickCallbackHandler drawPileClicked;

    public void Update(GameTime gameTime)
    {

        prevMouseState = currMouseState;
        currMouseState = Mouse.GetState();

        gt = gameTime;

        if(isClickAllowed())
        {

            if(isLeftMouseButtonDown())
            {

                //mouse was clicked this frame
                if(prevMouseState.LeftButton != ButtonState.Pressed)
                {

                    var mousePos = new Vector2(currMouseState.X, currMouseState.Y);

                    var topMostcard = getTopmostCardAtMousePos?.Invoke(mousePos);

                    cardBeingInteractedWith = topMostcard;

                } else {

                    if(cardBeingInteractedWith != null)
                        Console.WriteLine($"Moving card {cardBeingInteractedWith.cardInfo}");

                }

            }

        }

        if(isLeftMouseButtonReleased())
        {

            var mousePos = new Vector2(currMouseState.X, currMouseState.Y);

            var stackUnderMouse = getCardStackAtMousePos?.Invoke(mousePos);

            if(stackUnderMouse != null && stackUnderMouse.stackType == stackType.drawPile)
            {

                drawPileClicked?.Invoke();

            }

            lastClickTime = gameTime.TotalGameTime;
            cardBeingInteractedWith = null;

            //TODO - process letting go of card


        }

    }

    public void checkForRestarts()
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

        if(currMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed)
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

    public void processCardClick(Card card)
    {

        if(isClickAllowed())
        {

            if(isLeftMouseButtonDown())
            {

                var mousePos = new Vector2(currMouseState.X, currMouseState.Y);

                var topMostcard = getTopmostCardAtMousePos?.Invoke(mousePos);

                //mouse was clicked this frame
                if(prevMouseState.LeftButton != ButtonState.Pressed)
                {

                    if(topMostcard == card)
                    {

                        //topmost card check for flipped cards
                        if(card.isShowingFace == true || card.isTopmostCard)
                        {

                            cardBeingInteractedWith = card;
                            Console.WriteLine($"{card.cardInfo} clicked");

                            //think this should be the other way round
                            int cardOffsetY = currMouseState.Y - card.cardPos.Y;
                            int cardOffsetX = currMouseState.X - card.cardPos.X;

                            mouseOffsetOnClick = new Vector2(cardOffsetX, cardOffsetY);

                            Console.WriteLine($"Mouse offset set to x = {cardOffsetX}, y = {cardOffsetY}");

                            card.setCardMoving(true);
                            card.movingCardPos = card.cardPos;
                            card.cardLayer = 0;

                        }
                    }


                } else { // assume card is being moved
                    
                    if(cardBeingInteractedWith != null)
                    {
                        
                        var newXpos = mousePos.X - mouseOffsetOnClick.X;
                        var newYpos = mousePos.Y - mouseOffsetOnClick.Y;
                        
                        Console.WriteLine($"Setting card coords to X={newXpos}, Y={newYpos}");
                        cardBeingInteractedWith.movingCardPos = new Rectangle((int) newXpos, (int) newYpos, Constants.CARD_WIDTH, Constants.CARD_HEIGHT); 
                    }

                }


            } else if(isLeftMouseButtonReleased())
            {

                if(cardBeingInteractedWith == card)
                {

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
                    card.setCardMoving(false);

                }

            } else {

                cardBeingInteractedWith = null;
                card.setCardMoving(false);

            }

        }

    }

}