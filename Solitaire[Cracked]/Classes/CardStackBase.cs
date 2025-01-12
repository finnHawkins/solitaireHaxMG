
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

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
    {

        for (int i = 0; i < cardPile.Count; i++)
        {
            cardPile[i].Update(gameTime);
        }
        // foreach(var card in cardPile)
        // {
        //     card.Update(gameTime);
        // }
    }

    public virtual void setCardPositions()
    {

        int cardIndex = 0;

        foreach (var card in cardPile)
        {

            int cardXpos = cardPile.Count;

            if(stackType == stackType.depot)
            {
                int cardYPos = Constants.DEPOT_Y_POS;

                //TODO - make it so that face down cards have a lower margin

                cardXpos = stackCounter + (Constants.CARD_WIDTH * (stackCounter - 1));

                card.cardPos = new Rectangle(cardXpos, cardYPos, Constants.CARD_WIDTH, Constants.CARD_HEIGHT);

                //TODO - Add margins based off resolution

                cardYPos += Constants.DEPOT_CARD_MARGIN;

            } else {

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
                }
            
                card.cardPos = new Rectangle(cardXpos,Constants.TOP_MARGIN,Constants.CARD_WIDTH, Constants.CARD_HEIGHT);

            }
        
            card.cardLayer = cardIndex;
            cardIndex--;

            Console.WriteLine($"Setting {card.cardInfo} layer to {cardIndex} for stack {stackID}");

        }
    }

    public Rectangle getCardRectangle()
    {

        int cardXpos = 0;

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
        }
        
        return new Rectangle(cardXpos,Constants.TOP_MARGIN,Constants.CARD_WIDTH, Constants.CARD_HEIGHT);
    }
    
}