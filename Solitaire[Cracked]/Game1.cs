using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Solitaire_Cracked_;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    public static ContentManager content;
    public GraphicsDevice device;

    private List<Card> deck;
    private List<Depot> depots = [];
    private List<Foundation> foundations = [];
    private Foundation drawPile = new(false, true, false, 1);
    private Foundation discardPile = new(false, false, true, 2);

    private SettingsManager settings = new();
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        content = Content;

        IsMouseVisible = true;

        changeResolution(640, 360);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        //create depots
        for (int i = 1; i < 8; i++)
        {
            depots.Add(new Depot(i));
        }

        int slotCounter = 3; //one for draw, one for discard, so starts at 3

        //create foundations
        for (int i = 1; i < 5; i++)
        {
            foundations.Add(new Foundation(true, false, false, slotCounter));
            slotCounter++;
        }

        drawPile.Callback += new Foundation.CallbackEventHandler(onDrawPileClicked);

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
        

        base.Initialize();
    }

    protected override void LoadContent()
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

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGreen);

        // TODO: Add your drawing code here

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

        base.Draw(gameTime);
    }

    /// <summary>
    /// Modifies the Graphics Manager's Preferred Back Buffer width and height to given values.
    /// </summary>
    /// <param name="width">PreferredBackBufferWidth</param>
    /// <param name="height">PreferredBackBufferHeight</param>
    private void changeResolution(int width, int height)
    {
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.ApplyChanges();
    }

    /// <summary>
    /// https://community.monogame.net/t/passing-the-contentmanager-to-every-class-feels-wrong-is-it/10470/9
    /// Code copied from the above link for more finessed content managers
    /// </summary>
    /// <returns></returns>
    public static ContentManager GetNewContentManagerInstance()
    {
      // create a new content manager instance
      ContentManager temp = new ContentManager(content.ServiceProvider, content.RootDirectory);
      temp.RootDirectory = "Content";
      return temp;
    }

    public GraphicsDevice GetGraphicsDevice()
    {
        return device;
    }

    #region Deck

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
				var card = new Card((Suit)i, j, GraphicsDevice);
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
		Random rng = new Random();
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

            Console.WriteLine($"Depot {depot.depotNo} cards: {depotCards}");

        }

        resetDepotTopmostCardFlags();

        drawPile.cardPile = deck;

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

        //find card - TODO: refactor
        



    }

    public void moveCards(Card highestCard)
    {

    }

    #endregion

}
