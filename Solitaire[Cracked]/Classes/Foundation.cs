using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Solitaire_Cracked_;

// use for Foundations but also use for draw piles
public class Foundation : CardStackBase
{
    public delegate void CallbackEventHandler();
    public event CallbackEventHandler Callback; 

    protected bool isClicked;
	
	public Foundation(stackType sType, int stackCount)
	{

        stackType = sType;
        stackCounter = stackCount;

        baseCardPosition = getCardRectangle();
		
		cardPile = new List<Card>();
		
	}


    public override void Update(GameTime gameTime) {

        var ms = Mouse.GetState();

        if(baseCardPosition.Contains(new Vector2(ms.X, ms.Y)))
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

    public void setCardPositions()
    {
        
        int cardLayer = cardPile.Count;

        foreach(var card in cardPile)
        {

            card.cardPos = baseCardPosition;

            if(stackType == stackType.foundation)
            {
                card.cardLayer = cardLayer;
                cardLayer --;
            }

        }
    }
	
}
