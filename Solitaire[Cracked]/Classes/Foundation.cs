using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Solitaire_Cracked_;

// use for Foundations but also use for draw piles
public class Foundation : CardStackBase
{
    public Rectangle rect;

    public delegate void CallbackEventHandler();
    public event CallbackEventHandler Callback; 

    protected bool isClicked;
	
	public Foundation(stackType sType, int stackCount)
	{

        stackType = sType;
        stackCounter = stackCount;

        rect = getCardRectangle();
		
		cardPile = new List<Card>();
		
	}


    public override void Update(GameTime gameTime) {

        var ms = Mouse.GetState();

        if(rect.Contains(new Vector2(ms.X, ms.Y)))
        {

            if(ms.LeftButton == ButtonState.Pressed)
            {
                isClicked = true;

            } else if (ms.LeftButton == ButtonState.Released && isClicked)
            {

                if(stackType == stackType.drawPile)
                {
                    if (Callback != null)
                        Callback();
                }

                isClicked = false;
            }

        }

    }

    public Rectangle getCardRectangle()
    {

        int cardXpos;

        switch (stackType)
        {
            case stackType.drawPile:
                cardXpos = 1;
                break;
            case stackType.discardPile:
                cardXpos = 2 + Constants.CARD_WIDTH;
                break;
            default:
                cardXpos = stackCounter + (Constants.CARD_WIDTH * (stackCounter + 1)) + Constants.CARD_WIDTH + 1;
                break;
        }
        
        return new Rectangle(cardXpos,Constants.TOP_MARGIN,Constants.CARD_WIDTH, Constants.CARD_HEIGHT);
    }

    public void setCardPositions()
    {
        
        int cardLayer = cardPile.Count;

        foreach(var card in cardPile)
        {

            card.cardPos = rect;

            if(stackType == stackType.foundation)
            {
                card.cardLayer = cardLayer;
                cardLayer --;
            }

        }
    }
	
}
