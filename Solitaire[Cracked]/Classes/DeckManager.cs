using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

class DeckManager() {

    GraphicsDevice graphics;

    List<Card> deck;
    
    private List<Depot> depots = [];
    private List<Foundation> foundations = [];
    private Foundation drawPile = new(stackType.drawPile, 0);
    private Foundation discardPile = new(stackType.discardPile, 0);

    public void Initialize(GraphicsDevice gd)
    {
        //create depots
        for (int i = 1; i < 8; i++)
        {
            depots.Add(new Depot(stackType.depot, i));
        }

        //create foundations
        for (int i = 1; i < 5; i++)
        {
            foundations.Add(new Foundation(stackType.foundation, i));
        }

        drawPile.Callback += new Foundation.CallbackEventHandler(onDrawPileClicked);

        graphics = gd;

        generateDeck();
        shuffleDeck();
        dealDeck();
        
        drawPile.setCardPositions();
        discardPile.setCardPositions();

        foreach (var f in foundations)
        {
            f.setCardPositions();
        }

        foreach (var d in depots)
        {
            d.setCardPositions();
        }
    }

    public void LoadContent()
    {
        drawPile.LoadContent();
        discardPile.LoadContent();

        foreach(var f in foundations)
        {
            f.LoadContent();
        }

        foreach (var d in depots)
        {
            d.LoadContent();
        }
    }

    public void Update(GameTime gameTime)
    {
        drawPile.Update(gameTime);
        discardPile.Update(gameTime);

        foreach(var f in foundations)
        {
            f.Update(gameTime);
        }

        foreach (var d in depots)
        {
            d.Update(gameTime);
        }
    }

    public void Draw()
    {
        drawPile.Draw();
        discardPile.Draw();

        foreach(var f in foundations)
        {
            f.Draw();
        }

        foreach (var d in depots)
        {
            d.Draw();
        }
    }

    /// <summary>
    /// Generates a 52 card deck. 
    /// Adds Jokers if cheats enabled.
    /// </summary>
    private void generateDeck()
	{
		//declare new deck
		deck = new List<Card>();

		//loop through suits
		for(int i = 1; i < 5; i++)
		{
			//loops through ranks
			for (int j = 1; j < 14; j++)
			{
				//create new card using rank and suit and add it to the deck
				var card = new Card((Suit)i, j, graphics);
                card.doubleClickCallback += new Card.CallbackEventHandler(sendCardToFoundation);
                card.clickAndDragCallback += new Card.CallbackEventHandler(moveCards);
				deck.Add(card);
			}
		}
		
	}

    /// <summary>
    /// Shuffles the deck.
    /// </summary>
	private void shuffleDeck()
	{
		Random rng = new();
		deck = deck.OrderBy(_ => rng.Next()).ToList();

		//logging shuffled deck
		// foreach(Card card in deck)
		// {
		// 	Console.WriteLine(card.cardInfo);
		// }
	}

    /// <summary>
    /// 
    /// </summary>
    private void dealDeck()
    {

        int startingDepot = 0;

        while (startingDepot < 7)
        {
            for (int i = startingDepot; i < 7; i++)
            {
                depots[i].cardPile.Add(deck[0]);
                deck.RemoveAt(0);
            }

            depots[startingDepot].cardPile.Last().flipCard(true);
            depots[startingDepot].cardPile.Last().isTopmostCard = true;

            startingDepot++;
        }

        foreach(var depot in depots)
        {
            string depotCards = "";

            foreach(var card in depot.cardPile)
            {
                depotCards += card.cardInfo + ", ";
            }

            Console.WriteLine($"Depot {depot.stackCounter} cards: {depotCards}");

        }

        resetDepotTopmostCardFlags();

        drawPile.cardPile = deck;

        string drawString = "Draw Pile: ";

        foreach (var card in drawPile.cardPile)
        {
            drawString += card.cardInfo + ",";
        }

        Console.WriteLine(drawString);

    }

    public void resetDepotTopmostCardFlags()
    {
        foreach (var depot in depots)
        {
            foreach (var card in depot.cardPile)
            {
                card.isTopmostCard = false;
            }

            if(depot.cardPile.Count > 0)
                depot.cardPile.Last().isTopmostCard = true;
        }
    }

    
    #region Event Handlers

    public void onDrawPileClicked()
    {

        //need to reset the piles
        if(drawPile.cardPile.Count == 0)
        {

            Console.WriteLine("resetting draw pile");

            foreach(var card in discardPile.cardPile)
            {
                card.flipCard(false);
            }

            if(discardPile.cardPile.Count > 0)
                discardPile.cardPile.Last().isTopmostCard = false;

            drawPile.cardPile.AddRange(discardPile.cardPile);
            drawPile.setCardPositions();

            discardPile.cardPile.Clear();

        } else {

            var card = drawPile.cardPile[0];

            card.flipCard(true);
            card.isTopmostCard = true;

            if(discardPile.cardPile.Count > 0)
                discardPile.cardPile.Last().isTopmostCard = false;

            discardPile.cardPile.Add(card);
            
            card.cardPos = discardPile.getCardRectangle();

            drawPile.cardPile.RemoveAt(0);
        }

    }

    public void sendCardToFoundation(Card card)
    {

        //find card in right stack
        //check if valid to move to a foundation
        //if so, move

        Console.WriteLine("attempting to move card " + card.cardInfo + " to foundation");

    }

    public void moveCards(Card highestCard)
    {

    }

    #endregion

}