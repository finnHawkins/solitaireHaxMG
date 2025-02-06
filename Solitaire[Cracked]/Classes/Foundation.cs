using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Solitaire_Cracked_;

// use for Foundations but also use for draw/discard piles
public class Foundation : CardStackBase
{
    
    protected bool isClicked;
	
	public Foundation(stackType sType, int stackCount)
	{

        stackType = sType;
        stackCounter = stackCount;

        baseCardPosition = getCardRectangle();
		
		cardPile = new List<Card>();
		
	}
	
}
