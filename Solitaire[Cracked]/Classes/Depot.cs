using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Depot
{
	
	public int depotNo;
	
	public List<Card> cardPile;

	public Depot(int _depotNo)
	{

        depotNo = _depotNo;

		cardPile = new List<Card>();
	}

    public void LoadContent()
    {

        foreach(var card in cardPile)
        {
            card.LoadContent();
        }

    }

    public void Draw()
    {

        foreach(var card in cardPile)
        {

            //TODO - Add margins based off resolution
            card.Draw();
        }

    }

    public void setCardPositions()
    {
        int cardYPos = Constants.DEPOT_Y_POS;

        //TODO - make it so that face down cards have a lower margin

        foreach(var card in cardPile)
        {

            int cardXpos = depotNo + (Constants.CARD_WIDTH * (depotNo - 1));

            card.cardPos = new Rectangle(cardXpos, cardYPos, Constants.CARD_WIDTH, Constants.CARD_HEIGHT);

            //TODO - Add margins based off resolution

            cardYPos += Constants.DEPOT_CARD_MARGIN;
        }
    }

    public void Update()
    {
        foreach(var card in cardPile)
        {
            card.Update();
        }
    }
	
}
