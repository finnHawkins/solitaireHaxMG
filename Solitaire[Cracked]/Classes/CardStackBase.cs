
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
            return stackType.ToString() + stackCounter.ToString();
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
        foreach(var card in cardPile)
        {
            card.Update(gameTime);
        }
    }
    
}