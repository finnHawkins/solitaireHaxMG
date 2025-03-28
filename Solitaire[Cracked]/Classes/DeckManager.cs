using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public class DeckManager()
{

    GraphicsDevice graphics;
    SpriteBatch _spriteBatch;
    SpriteBatch _movingCardSpriteBatch;

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
        for(int i = 1; i < 8; i++)
        {
            depots.Add(new Depot(stackType.depot, i));
        }

        //create foundations
        for(int i = 1; i < 5; i++)
        {
            foundations.Add(new Foundation(stackType.foundation, i));
        }

        graphics = gd;

        generateDeck();

        setupBoard();

    }

    public void setupBoard()
    {

        drawPile.cardPile.Clear();
        discardPile.cardPile.Clear();

        foreach(var f in foundations)
        {
            f.cardPile.Clear();
        }

        foreach(var d in depots)
        {
            d.cardPile.Clear();
        }

        lookupTable.Clear();

        shuffleDeck();
        dealDeck();

        drawPile.setCardPositions();
        discardPile.setCardPositions();

        foreach(var f in foundations)
        {
            f.setCardPositions();
        }

        foreach(var d in depots)
        {
            d.setCardPositions();
        }

    }

    public void LoadContent(ContentManager content)
    {

        _spriteBatch = new SpriteBatch(graphics);
        _movingCardSpriteBatch = new SpriteBatch(graphics);

        foreach(KeyValuePair<Card, CardStackBase> entry in lookupTable)
        {
            entry.Key.LoadContent(content);
        }

    }

    public void Update(GameTime gameTime) { }

    public void Draw()
    {

        _spriteBatch.Begin();
        _movingCardSpriteBatch.Begin();

        foreach(var card in discardPile.cardPile)
        {
            if(!movingCards.Contains(card))
            {
                card.Draw(_spriteBatch);
            }
        }

        foreach(var card in drawPile.cardPile)
        {
            if(!movingCards.Contains(card))
            {
                card.Draw(_spriteBatch);
            }
        }

        foreach(var f in foundations)
        {
            
            foreach(var card in f.cardPile)
            {
                if(!movingCards.Contains(card))
                {
                    card.Draw(_spriteBatch);
                }
            }
     
        }

        foreach(var d in depots)
        {
            
            foreach(var card in d.cardPile)
            {
                if(!movingCards.Contains(card))
                {
                    card.Draw(_spriteBatch);
                }
            }
     
        }

        foreach(var card in movingCards)
        {
            card.Draw(_movingCardSpriteBatch);
        }

        _spriteBatch.End();
        _movingCardSpriteBatch.End();

    }

    public void restartGame()
    {

        Console.WriteLine("Restarting game...");

        deck.Clear();

        foreach(KeyValuePair<Card, CardStackBase> cardEntry in lookupTable)
        {
            cardEntry.Key.resetCard();
            deck.Add(cardEntry.Key);
        }

        setupBoard();

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
            for(int j = 1; j < 14; j++)
            {
                //create new card using rank and suit and add it to the deck
                var card = new Card((Suit)i, j);
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

    }

    /// <summary>
    /// 
    /// </summary>
    private void dealDeck()
    {

        int startingDepot = 0;

        while(startingDepot < 7)
        {
            for(int i = startingDepot; i < 7; i++)
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

        foreach(var d in depots)
        {
            d.updateCardLayers();
        }

        foreach(var card in deck)
        {
            drawPile.cardPile.Add(card);
            lookupTable.Add(card, drawPile);

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

    #endregion

    #region Input Events

    public void onDrawPileClicked()
    {

        //need to reset the piles
        if(drawPile.cardPile.Count == 0)
        {

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

        var parentStack = getCardOwnerStack(card);

        if(parentStack.stackType == stackType.foundation)
        {
            //do nothing
        } else {

            foreach(var f in foundations)
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
                            f.updateCardLayers();

                            lookupTable[card] = f;

                            break;
                        }

                    }

                } else {

                    if(card.rank == 1)
                    {

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

        parentStack.updateCardLayers();

    }

    public CardStackBase getStackAtMousePos(Vector2 mousePos)
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

        if(ownerStack == null || ownerStack == default)
        {
            if(discardPile.getCardRectangle().Contains(mousePos))
            {
                ownerStack = discardPile;

            } else if(drawPile.getCardRectangle().Contains(mousePos)) {
                ownerStack = drawPile;
            }
        }

        return ownerStack;

    }

    public Card getTopmostCardAtMousePos(Vector2 mousePos)
    {

        CardStackBase ownerStack = getStackAtMousePos(mousePos);

        if(ownerStack?.stackType == stackType.discardPile)
        {
            return ownerStack.cardPile.Count == 0 ? default : ownerStack.cardPile.Last();
        }

        Card topCard = default;

        //card has not been clicked
        if(ownerStack == null || ownerStack == default)
        {
            return topCard;
        }

        var applicableCards = ownerStack.cardPile.Where(card => card.cardPos.Contains(mousePos.X, mousePos.Y));

        topCard = applicableCards.MinBy(card => card.cardLayer);

        return topCard;

    }

    public CardStackBase getCardOwnerStack(Card card)
    {

        return lookupTable[card];

    }

    public void processCardFlip(Card card)
    {

        if(card == getCardOwnerStack(card).cardPile.Last())
        {

            card.flipCard(true);

        }

    }

    public void setCardStackToMoving(Card parentCard)
    {

        var ownerStack = getCardOwnerStack(parentCard);

        var cardIndex = ownerStack.cardPile.FindIndex(card => card == parentCard);

        //get index of parentCard
        //add all cards after and including that index to moving
        //set them to moving

        for(int i = cardIndex; i < ownerStack.cardPile.Count; i++)
        {
            var card = ownerStack.cardPile[i];

            movingCards.Add(card);

        }

    }

    public void moveCards(Vector2 newCardPos)
    {

        Console.WriteLine("Moving card stack");

        //TODO - sort out layering

        foreach(var c in movingCards)
        {
            c.cardPos.X = (int)newCardPos.X;
            c.cardPos.Y = (int)newCardPos.Y;
        }

    }

    public void dropCardStack()
    {

        //check if move is valid
        var topCard = movingCards[0];

        var topCardRect = new Rectangle(topCard.cardPos.X, topCard.cardPos.Y, 1, Constants.CARD_WIDTH);

        List<CardStackBase> overlappingStacks = [];
        overlappingStacks.AddRange(foundations.Where(s => s.getStackArea().Intersects(topCard.cardPos)));
        overlappingStacks.AddRange(depots.Where(s => s.getStackArea().Intersects(topCard.cardPos)));

        if(overlappingStacks.Count > 0)
        {

            List<CardStackBase> validStacks = [];

            foreach(var s in overlappingStacks)
            {

                if(isValidMove(topCard, s))
                {

                    validStacks.Add(s);

                }

            }

            //logic is as follows:
            //prioritise foundation moves
            //if two are next to each other and both are depots, go with leftmost one (lower stackID)
            if(validStacks.Count(s => s.stackType == stackType.foundation) > 0)
            {

                moveCardsToStack(validStacks.First(s => s.stackType == stackType.foundation));

            } else if(validStacks.Count(s => s.stackType == stackType.depot) > 0)
            {

                moveCardsToStack(validStacks.First(s => s.stackType == stackType.depot));

            }

        }

        var ownerStack = getCardOwnerStack(topCard);
        ownerStack.setCardPositions();

        movingCards.Clear();

    }

    public bool isValidMove(Card card, CardStackBase targetStack)
    {

        bool validMove = false;

        Card stackLastCard = default;

        if(targetStack.cardPile.Count > 0)
        {
            stackLastCard = targetStack.cardPile.Last();
        }

        if(stackLastCard != null || stackLastCard != default)
        {

            if(targetStack.stackType == stackType.depot)
            {

                //alternate colours
                if((stackLastCard.isRed && !card.isRed) || (!stackLastCard.isRed && card.isRed))
                {

                    if(stackLastCard.rank == card.rank + 1 && stackLastCard.isShowingFace)
                    {
                        validMove = true;

                    }

                }

            } else if(targetStack.stackType == stackType.foundation)
            {

                if(stackLastCard.rank == card.rank - 1 && stackLastCard.suit == card.suit)
                {
                    validMove = true;

                }

            }

        } else {

            //valid move
            if(card.rank == 13 && targetStack.stackType == stackType.depot)
            {
                
                validMove = true;

            } else if(targetStack.stackType == stackType.foundation && movingCards.Count == 1 && movingCards[0].rank == 1) {

                validMove = true;

            }

        }

        return validMove;

    }

    public void moveCardsToStack(CardStackBase newOwningStack)
    {

        //remove cards from original stack
        //add cards to new stack
        //update lookup table
        var oldOwner = getCardOwnerStack(movingCards[0]);

        var cardIndex = oldOwner.cardPile.FindIndex(card => card == movingCards[0]);

        var newList = oldOwner.cardPile.GetRange(cardIndex, movingCards.Count);

        foreach(var card in newList)
        {

            card.cardPos = newOwningStack.baseCardPosition;
            newOwningStack.cardPile.Add(card);
            lookupTable[card] = newOwningStack;

        }

        oldOwner.cardPile.RemoveAll(x => newList.Contains(x));

        oldOwner.updateCardLayers();
        newOwningStack.updateCardLayers();

    }

    #endregion

}