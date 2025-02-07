using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Solitaire_Cracked_;

public class InputManager(DeckManager dm)
{

    public DeckManager deckManager = dm;

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

    public void Update(GameTime gameTime)
    {

        checkForRestarts();

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

                    var topMostcard = deckManager.getTopmostCardAtMousePos(mousePos);

                    cardBeingInteractedWith = topMostcard;

                    if(cardBeingInteractedWith != null && cardBeingInteractedWith.isShowingFace)
                    {

                        int cardOffsetX, cardOffsetY;

                        if(topMostcard != null)
                        {
                            cardOffsetY = currMouseState.Y - topMostcard.cardPos.Y;
                            cardOffsetX = currMouseState.X - topMostcard.cardPos.X;
                            mouseOffsetOnClick = new Vector2(cardOffsetX, cardOffsetY);
                            Console.WriteLine($"Mouse offset set to x = {cardOffsetX}, y = {cardOffsetY}");

                        }

                        //process moving in deckmanager

                    } else {

                        if(!cardBeingInteractedWith.isTopmostCard)
                            cardBeingInteractedWith = null;

                    }

                } else {

                    if(cardBeingInteractedWith != null)
                        Console.WriteLine($"Moving card {cardBeingInteractedWith.cardInfo}");

                }

            }

        }

        if(isLeftMouseButtonReleased())
        {

            var mousePos = new Vector2(currMouseState.X, currMouseState.Y);

            //Check if draw pile got clicked
            var stackUnderMouse = deckManager.getStackAtMousePos(mousePos);

            if(stackUnderMouse != null && stackUnderMouse.stackType == stackType.drawPile)
            {

                if(cardBeingInteractedWith == null)
                    deckManager.onDrawPileClicked();

            }

            if(cardBeingInteractedWith != null)
            {
                //check for double click
                if(cardBeingInteractedWith.isShowingFace)
                {

                    if(lastCardInteractedWith == cardBeingInteractedWith)
                    {

                        if(clickIsWithinDoubleClickTimeframe())
                        {

                            Console.WriteLine($"{cardBeingInteractedWith.cardInfo} was double clicked");
                            
                            setClickCooldown();

                            deckManager.sendCardToFoundation(cardBeingInteractedWith);

                        } else {

                            lastCardInteractedWith = cardBeingInteractedWith;

                        }

                    } else {

                        lastCardInteractedWith = cardBeingInteractedWith;

                    }

                } else {

                    Console.WriteLine($"Turned over card {cardBeingInteractedWith.cardInfo} clicked");

                    cardBeingInteractedWith.flipCard(true);

                    setClickCooldown();

                }

                lastCardInteractedWith = cardBeingInteractedWith;
                cardBeingInteractedWith = null;
                lastClickTime = gameTime.TotalGameTime;
            }

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

        return currMouseState.LeftButton == ButtonState.Pressed;
        
    }

    public bool isLeftMouseButtonReleased()
    {

        return currMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed;

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

                var topMostcard = deckManager.getTopmostCardAtMousePos(mousePos);

                //mouse was clicked this frame
                if(prevMouseState.LeftButton != ButtonState.Pressed)
                {

                    if(topMostcard == card)
                    {

                        //topmost card check for flipped cards
                        if(card.isShowingFace == true || card.isTopmostCard)
                        {

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

                                //card.callDoubleClickCallback();

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