using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Depot : CardStackBase
{
	
	public Depot(stackType sType, int stackCount)
	{

        stackType = sType;
        stackCounter = stackCount;

		cardPile = new List<Card>();
	}

    public void setCardPositions()
    {
        int cardYPos = Constants.DEPOT_Y_POS;

        int cardLayer = cardPile.Count;

        //TODO - make it so that face down cards have a lower margin

        foreach(var card in cardPile)
        {

            int cardXpos = stackCounter + (Constants.CARD_WIDTH * (stackCounter - 1));

            card.cardPos = new Rectangle(cardXpos, cardYPos, Constants.CARD_WIDTH, Constants.CARD_HEIGHT);

            //TODO - Add margins based off resolution

            cardYPos += Constants.DEPOT_CARD_MARGIN;
            
            card.cardLayer = cardLayer;
            cardLayer --;
        }
    }
	
}
