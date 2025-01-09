
using System;
using System.Collections.Generic;
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

        foreach (var card in cardPile)
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
                case stackType.depot:
                    break;
            }
        
            card.cardPos = new Rectangle(cardXpos,Constants.TOP_MARGIN,Constants.CARD_WIDTH, Constants.CARD_HEIGHT);
        
        }
    }
    
}