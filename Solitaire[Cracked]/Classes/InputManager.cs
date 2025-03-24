using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class InputManager(DeckManager dm)
{

    public enum moveState
    {
        click,
        drag,
        idle
    }

    public DeckManager deckManager = dm;

    bool isRestartKeyBeingPressed;

    public bool shouldRestartGame { get; private set; }

    public TimeSpan lastClickTime { get; private set; }
    //public TimeSpan nextClickAllowedTime { get; private set; }

    MouseState prevMouseState;
    MouseState currMouseState;

    GameTime gt;

    Card lastCardInteractedWith;
    Card cardBeingInteractedWith;

    Vector2 mouseOffsetOnClick;

    Rectangle mouseDragBorderBox;
    moveState mouseMoveState;

    TimeSpan doubleClickTimeout;

    public void Update(GameTime gameTime)
    {

        checkForRestarts();

        prevMouseState = currMouseState;
        currMouseState = Mouse.GetState();

        gt = gameTime;

        // if(!isClickAllowed())
        // {
        //     return;
        // }

        if(isLeftMouseButtonDown())
        {

            //mouse was clicked this frame
            if(prevMouseState.LeftButton != ButtonState.Pressed)
            {
                mouseMoveState = moveState.click;

                var borderX = currMouseState.X - Constants.MOUSE_BOUND_BORDER_PIXEL_SIZE;
                var borderY = currMouseState.Y - Constants.MOUSE_BOUND_BORDER_PIXEL_SIZE;
                var borderSize = Constants.MOUSE_BOUND_BORDER_PIXEL_SIZE * 2;
                mouseDragBorderBox = new Rectangle(borderX, borderY, borderSize, borderSize);

                doubleClickTimeout = gt.TotalGameTime.Add(new TimeSpan(0, 0, 1));

                var mousePos = new Vector2(currMouseState.X, currMouseState.Y);

                var topMostcard = deckManager.getTopmostCardAtMousePos(mousePos);

                cardBeingInteractedWith = topMostcard;

            } else {

                if(mouseMoveState == moveState.click)
                {

                    if(mouseDragBorderBox.Contains(new Vector2(currMouseState.X, currMouseState.Y)))
                    {

                        if(gt.TotalGameTime >= doubleClickTimeout)
                        {

                            setMouseToDrag();

                        }

                    } else {

                        setMouseToDrag();
                    }

                } else if(mouseMoveState == moveState.drag && cardBeingInteractedWith != null) {

                    //move cards
                    Console.WriteLine($"Moving card {cardBeingInteractedWith.cardInfo}");

                    //TODO - add moving
                    updateCardPos();
                }

            }

        } else {

            if(isLeftMouseButtonReleased())
            {

                var mousePos = new Vector2(currMouseState.X, currMouseState.Y);

                if(mouseMoveState == moveState.click)
                {

                    //Check if draw pile got clicked
                    var stackUnderMouse = deckManager.getStackAtMousePos(mousePos);

                    if(stackUnderMouse != null && stackUnderMouse.stackType == stackType.drawPile)
                    {

                        if(stackUnderMouse.cardPile.Count == 0 || cardBeingInteractedWith == stackUnderMouse.cardPile[0])
                        {
                            deckManager.onDrawPileClicked();
                            cardBeingInteractedWith = null;
                            lastCardInteractedWith = null;
                        }

                    } else {

                        Console.WriteLine("Processing click in IM");

                        if(cardBeingInteractedWith != null)
                        {
                            if(lastCardInteractedWith == cardBeingInteractedWith)
                            {
                                if(clickIsWithinDoubleClickTimeframe())
                                {
                                    deckManager.sendCardToFoundation(cardBeingInteractedWith);
                                }

                            } else {

                                if(!cardBeingInteractedWith.isShowingFace)
                                {
                                    deckManager.processCardFlip(cardBeingInteractedWith);
                                }

                            }

                        }

                        //reset variables
                        lastCardInteractedWith = cardBeingInteractedWith;
                        cardBeingInteractedWith = null;

                    }

                } else {

                    //TODO - process mouse movement and move cards to new stack
                    if(cardBeingInteractedWith != null)
                    {

                        Console.WriteLine($"Dropping {cardBeingInteractedWith.cardInfo}");
                        deckManager.dropCardStack();

                    }


                }

                mouseMoveState = moveState.idle;
                lastClickTime = gt.TotalGameTime;

            }

        }

    }

    public void checkForRestarts()
    {

        if(shouldRestartGame)
            shouldRestartGame = false;

        if(Keyboard.GetState().IsKeyDown(Constants.RESTART_GAME_KEY))
        {
            isRestartKeyBeingPressed = true;
        }

        if(isRestartKeyBeingPressed && Keyboard.GetState().IsKeyUp(Constants.RESTART_GAME_KEY))
        {
            isRestartKeyBeingPressed = false;
            shouldRestartGame = true;
        }

    }


    public bool isLeftMouseButtonDown()
    {

        return currMouseState.LeftButton == ButtonState.Pressed;

    }

    public bool isLeftMouseButtonReleased()
    {

        return currMouseState.LeftButton == ButtonState.Released && prevMouseState.LeftButton == ButtonState.Pressed;

    }

    public void setMouseToDrag()
    {
        mouseMoveState = moveState.drag;
        deckManager.setCardStackToMoving(cardBeingInteractedWith);

        if(cardBeingInteractedWith != null)
        {
            var cardOffsetY = currMouseState.Y - cardBeingInteractedWith.cardPos.Y;
            var cardOffsetX = currMouseState.X - cardBeingInteractedWith.cardPos.X;
            mouseOffsetOnClick = new Vector2(cardOffsetX, cardOffsetY);
            Console.WriteLine($"Mouse offset set to x = {cardOffsetX}, y = {cardOffsetY}");
        }

        Console.WriteLine("Mouse state set to drag as doubleClickTimeout was met");

    }

    public void updateCardPos()
    {
        var mousePos = new Vector2(currMouseState.X, currMouseState.Y);
        var newXpos = mousePos.X - mouseOffsetOnClick.X;
        var newYpos = mousePos.Y - mouseOffsetOnClick.Y;
                        
        deckManager.moveCards(new Vector2(newXpos, newYpos));
    }

    // public bool isClickAllowed()
    // {

    //     return gt.TotalGameTime > nextClickAllowedTime;

    // }

    public bool isExitGameButtonDown()
    {

        //GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Constants.END_GAME_KEY);
        return Keyboard.GetState().IsKeyDown(Constants.END_GAME_KEY);

    }

    // public void setClickCooldown()
    // {

    // 	nextClickAllowedTime = gt.TotalGameTime.Add(new TimeSpan(0,0,0,0,Constants.CLICK_DELAY));

    // }

    public bool clickIsWithinDoubleClickTimeframe()
    {

        return gt.TotalGameTime < lastClickTime.Add(new TimeSpan(0, 0, 0, 0, Constants.DOUBLE_CLICK_TOLERANCE));

    }

}