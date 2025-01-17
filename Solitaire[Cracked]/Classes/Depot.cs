using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Solitaire_Cracked_;

public class Depot : CardStackBase
{
	
	public Depot(stackType sType, int stackCount)
	{

        stackType = sType;
        stackCounter = stackCount;

        baseCardPosition = getCardRectangle();

		cardPile = new List<Card>();
		
	}
	
}
