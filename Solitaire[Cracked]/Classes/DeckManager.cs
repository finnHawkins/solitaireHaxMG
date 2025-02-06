using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

public class DeckManager() {

    GraphicsDevice graphics;

    List<Card> deck;
    
    private List<Depot> depots = [];
    private List<Foundation> foundations = [];
    private Foundation drawPile = new(stackType.drawPile, 0);
    private Foundation discardPile = new(stackType.discardPile, 0);

    private List<Card> movingCards = new();

    private Dictionary<Card, CardStackBase> lookupTable = new();

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

        foreach (KeyValuePair<Card, CardStackBase> entry in lookupTable)
        {
            entry.Key.LoadContent();
        }
        
    }

    public void Update(GameTime gameTime)
    {

        // foreach (KeyValuePair<Card, CardStackBase> entry in lookupTable)
        // {
        //     entry.Key.Update(gameTime);
        // }

        foreach (var f in foundations)
        {
            f.Update(gameTime);
        }

        drawPile.Update(gameTime);
        discardPile.Update(gameTime);

        // if(movingCards.Count > 0)
        //     movingCards.ForEach(card => card.Update(gameTime));

    }

    public void Draw()
    {

        foreach (KeyValuePair<Card, CardStackBase> entry in lookupTable)
        {
            entry.Key.Draw();
        }

    }

    public void restartGame()
    {

        Console.WriteLine("Restarting game...");

        deck.Clear();

        foreach (KeyValuePair<Card, CardStackBase> cardEntry in lookupTable)
        {
            cardEntry.Key.flipCard(false);
            cardEntry.Key.cardLayer = 0;
            deck.Add(cardEntry.Key);
        }

        Console.WriteLine("Clearing all previous piles...");
        
        drawPile.cardPile.Clear();
        discardPile.cardPile.Clear();

        foreach (var f in foundations)
        {
            f.cardPile.Clear();
        }
        
        foreach (var d in depots)
        {
            d.cardPile.Clear();
        }

        lookupTable.Clear();

        Console.WriteLine("Shuffling deck...");

        shuffleDeck();

        Console.WriteLine("Dealing deck...");

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
				deck.Add(card);
			}
		}
		
	}

	/// <summary>
	/// Shuffles the deck.
	/// TODO - https://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle -- use this maybe
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

                lookupTable.Add(deck[0], depots[i]);

                deck.RemoveAt(0);
            }

            depots[startingDepot].cardPile.Last().flipCard(true);
            depots[startingDepot].cardPile.Last().isTopmostCard = true;

            startingDepot++;
        }

        logDepotCards();

        resetDepotTopmostCardFlags();

        foreach (var card in deck)
        {
            drawPile.cardPile.Add(card);
            lookupTable.Add(card, drawPile);

        }

    }

    public CardStackBase getParentStack(Card card)
    {

        var stack = lookupTable[card];

        CardStackBase returnedPile;

        if(stack.stackType == stackType.drawPile)
        {
            returnedPile = drawPile;

        } else if(stack.stackType == stackType.discardPile) {
            
            returnedPile = discardPile;

        } else if(stack.stackType == stackType.foundation) {

            returnedPile = foundations.FirstOrDefault(x => x.stackID == stack.stackID);

        } else {

            returnedPile = depots.FirstOrDefault(x => x.stackID == stack.stackID);

        }

        return returnedPile;

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

    #region Logging

    public void logDepotCards()
    {
        foreach(var depot in depots)
        {
            string depotCards = "";

            foreach(var card in depot.cardPile)
            {
                depotCards += card.cardInfo + ", ";
            }

            Console.WriteLine($"Depot {depot.stackCounter} cards: {depotCards}");

        }
    }

    public void logDict()
    {

        foreach (KeyValuePair<Card, CardStackBase> entry in lookupTable)
        {
            Console.WriteLine($"{entry.Key.cardInfo} belongs to stack {entry.Value.stackID}");
        }

    }

    public void logCards(CardStackBase cardStack)
    {
        Console.Write($"{cardStack.stackID} cards: ");
        foreach (var card in cardStack.cardPile)
        {
            Console.Write(card.cardInfo + ", ");
        }
        Console.WriteLine("---------");
    }

    public void logDeck()
    {

        Console.Write("Deck cards: ");
        foreach (var card in deck)
        {
            Console.Write($"{card.cardInfo},");
        }
        Console.Write("\n");
    }

    #endregion
    
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
                
                lookupTable[card] = drawPile;
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

            lookupTable[card] = discardPile;
        }

    }

    public void sendCardToFoundation(Card card)
    {

        var parentStack = getParentStack(card);

        Console.WriteLine($"{card.cardInfo} belongs to stack {parentStack.stackID}");

        if(parentStack.stackType == stackType.foundation)
        {
            //do nothing
        } else {

            foreach (var f in foundations)
            {
                
                if(f.cardPile.Count > 0)
                {

                    if(f.cardPile[0].suit == card.suit)
                    {
                        if(f.cardPile.Last().rank + 1 == card.rank)
                        {

                            parentStack.cardPile.Remove(card);

                            Console.WriteLine($"Removed {card.cardInfo} from {parentStack.stackID} and moved it to {f.stackID}");

                            if(parentStack.cardPile.Count > 0)
                                parentStack.cardPile.Last().isTopmostCard = true;


                            f.cardPile.Add(card);
                            f.setCardPositions();

                            lookupTable[card] = f;

                            break;
                        }

                    }

                } else {

                    if(card.rank == 1)
                    {
                        Console.WriteLine($"empty foundation found, adding card to {f.stackID}");

                        parentStack.cardPile.Remove(card);
                        
                        if(parentStack.cardPile.Count > 0)
                            parentStack.cardPile.Last().isTopmostCard = true;

                        f.cardPile.Add(card);
                        f.setCardPositions();

                        lookupTable[card] = f;

                        break;

                    }

                }

            }

        }

        foreach (var f in foundations)
        {
            f.updateCardLayers();
        }

        foreach (var d in depots)
        {
            d.updateCardLayers();
        }

        drawPile.updateCardLayers();
        discardPile.updateCardLayers();

    }

    public Card getTopmostCardAtMousePos(Vector2 mousePos)
    {

        CardStackBase ownerStack = null;

        //find stack mouse is over
        foreach(var f in foundations)
        {
            if(f.getCardRectangle().Contains(mousePos))
            {
                ownerStack = f;
                break;
            }
        }

        if(ownerStack == null || ownerStack == default)
        {
            foreach(var d in depots)
            {
                if(d.getCardRectangle().Contains(mousePos))
                {
                    ownerStack = d;
                    break;
                }
            }
        }

        Card topCard = default;

        //card has not been clicked
        if(ownerStack == null || ownerStack == default){
            Console.WriteLine("No cards were clicked");
            return topCard;
        }

        var applicableCards = ownerStack.cardPile.Where(card => card.isShowingFace == true && card.cardPos.Contains(mousePos.X, mousePos.Y));

        topCard = applicableCards.MaxBy(card => card.cardLayer);

        if(topCard != default)
            Console.WriteLine($"Found topmost card {topCard.cardInfo} that was clicked");
        else
            Console.WriteLine("No suitable topmost card found");
    
        return topCard;

    }

    public void SetCardStackToMoving()
    {

        


    }

    #endregion

}