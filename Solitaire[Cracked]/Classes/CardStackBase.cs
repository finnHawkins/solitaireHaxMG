
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Solitaire_Cracked_;

public enum stackType {
    drawPile,
    discardPile,
    foundation,
    depot
}

public class CardStackBase
{

    public List<Card> cardPile;

    public stackType stackType;

    public Rectangle baseCardPosition;

    public int stackCounter = 0;

    public string stackID {
        get {
            return stackType.ToString() + (stackCounter.ToString() == "0" ? "" : stackCounter);
        }
    }

    public CardStackBase(stackType sType, int stackCount)
	{

		stackType = sType;
		stackCounter = stackCount;

		baseCardPosition = getCardRectangle();

		cardPile = new List<Card>();
		
	}

    public virtual void LoadContent()
    {

        foreach(var card in cardPile)
        {
            card.LoadContent();
        }

    }

    public virtual void Draw()
    {

        foreach(var card in cardPile)
        {
            card.Draw();
        }

    }

    public virtual void Update(GameTime gameTime)
    {}

    public virtual Rectangle getStackArea()
    {

        var stackBottom = cardPile.Count == 0 ? baseCardPosition.Bottom : cardPile.Last().cardPos.Bottom;

        var stackHeight = stackBottom - baseCardPosition.Y;

        var stackArea = new Rectangle(baseCardPosition.X, baseCardPosition.Y, baseCardPosition.Height, stackHeight);

        Console.WriteLine($"{stackID} has a stackArea of: {stackArea}");

        return stackArea;

    }

    public virtual void setCardPositions()
    {

        int cardLayer = cardPile.Count;

        if(stackType != stackType.depot)
        {

            foreach(var card in cardPile)
            {

                card.cardPos = baseCardPosition;

                if(stackType == stackType.foundation)
                {
                    card.cardLayer = cardLayer;
                    cardLayer --;

                }

            }

        } else {

            int cardYPos = Constants.DEPOT_Y_POS;

            //TODO - make it so that face down cards have a lower margin

            foreach(var card in cardPile)
            {

                card.cardPos = new Rectangle(baseCardPosition.X, cardYPos, Constants.CARD_WIDTH, Constants.CARD_HEIGHT);

                //TODO - Add margins based off resolution

                cardYPos += Constants.DEPOT_CARD_MARGIN;
                
                card.cardLayer = cardLayer;
                cardLayer --;

            }

        }

    }

    public void updateCardLayers()
    {

        int cardLayer = cardPile.Count - 1;

        foreach (var card in cardPile)
        {
            if(stackType == stackType.foundation || stackType == stackType.depot)
            {
                card.cardLayer = cardLayer;
                cardLayer--;
            } else {
                card.cardLayer = 0;
            }
            card.isTopmostCard = false;
        }

        if(cardPile.Count > 0)
            cardPile.Last().isTopmostCard = true;

    }

    public Rectangle getCardRectangle()
    {

        int cardXpos = 0;
        int cardYPos = Constants.TOP_MARGIN;

        switch (stackType)
        {
            case stackType.drawPile:
                cardXpos = 1;
                break;
            case stackType.discardPile:
                cardXpos = 2 + Constants.CARD_WIDTH;
                break;
            case stackType.foundation:
                cardXpos = stackCounter + (Constants.CARD_WIDTH * (stackCounter + 1)) + Constants.CARD_WIDTH + 1;
                break;
            case stackType.depot:

                //use the first card place
                cardYPos = Constants.DEPOT_Y_POS;
                cardXpos = stackCounter + (Constants.CARD_WIDTH * (stackCounter - 1));
                break;
        }
        
        return new Rectangle(cardXpos,cardYPos,Constants.CARD_WIDTH, Constants.CARD_HEIGHT);
    }
    
}