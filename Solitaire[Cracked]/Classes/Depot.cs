using System.Collections.Generic;

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
