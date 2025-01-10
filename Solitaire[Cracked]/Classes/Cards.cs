using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Solitaire_Cracked_;

public enum Suit {
	H = 1,
	C,
	D,
	S
}

public class Card(Suit _suit, int _rank, GraphicsDevice gd)
{
    bool isShowingFace;
	bool isJoker {
		get {
			return ((int) suit == 0 && rank == 0);
		}
	}

	public bool isTopmostCard;

	public readonly Suit suit = _suit;
	public readonly int rank = _rank; // J = 11, Q = 12, K = 13

	Texture2D cardImg;
	Texture2D cardBack;

    private GraphicsDevice graphicsDevice = gd;
	private readonly ContentManager content = Game1.GetNewContentManagerInstance();

    private SpriteBatch _spriteBatch;

	public Rectangle cardPos;

	bool isBeingClicked;

	/// <summary>
	/// True if card was clicked within last second
	/// </summary>
	bool wasRecentlyClicked;
	public TimeSpan lastClicked;
	public TimeSpan nextClickAllowed;

    public delegate void CallbackEventHandler(Card card);
    public event CallbackEventHandler doubleClickCallback;
    public event CallbackEventHandler clickAndDragCallback;

	public string cardInfo {
		get {

			string cardRank;
			string cardSuit = "";

			switch (rank)
			{
				case 1:
					cardRank = "Ace";
					break;
				case 11:
					cardRank = "Jack";
					break;
				case 12:
					cardRank = "Queen";
					break;
				case 13:
					cardRank = "King";
					break;
				default:
					cardRank = rank.ToString();
					break;
			}

			switch (suit)
			{
				case Suit.H:
					cardSuit = "Heart";
					break;
				case Suit.C:
					cardSuit = "Club";
					break;
				case Suit.D:
					cardSuit = "Diamond";
					break;
				case Suit.S:
					cardSuit = "Spade";
					break;
			}

			return cardRank + " of " + cardSuit + "s";
		}
	}
	
	public bool isRed {
		get {
			return (int)suit%2 == 0;
		}
	}


    /// <summary>
	/// Flips a card depending on which side should be shown
	/// </summary>
	/// <param name="isFacingUp">Boolean declaring whether card should be displayed face up</param>
	public void flipCard(bool isFacingUp)
	{
		isShowingFace = isFacingUp;
	}

	public void LoadContent()
	{

		_spriteBatch = new SpriteBatch(graphicsDevice);

		cardBack = content.Load<Texture2D>("BACK_RED");

		string cardName = suit.ToString() + "_" + rank;

		cardImg = content.Load<Texture2D>(cardName);

	}

	public void Draw()
	{

		_spriteBatch.Begin();

		if(isShowingFace)
		{
			_spriteBatch.Draw(cardImg, new Vector2(cardPos.X, cardPos.Y), Color.White);
		} else {
			_spriteBatch.Draw(cardBack, new Vector2(cardPos.X, cardPos.Y), Color.White);
		}

		_spriteBatch.End();

	}

	public void Update(GameTime gameTime)
	{

		var ms = Mouse.GetState();

        if(cardPos.Contains(new Vector2(ms.X, ms.Y)))
        {
            if(isShowingFace && isTopmostCard)
			{

				if(gameTime.TotalGameTime > nextClickAllowed)
				{

					if(ms.LeftButton == ButtonState.Pressed)
					{
						isBeingClicked = true;
						Console.WriteLine("LB down registered");

					} else if (ms.LeftButton == ButtonState.Released && isBeingClicked)
					{
						Console.WriteLine($"LB released on {cardInfo}");

						if(wasRecentlyClicked)
						{

							if(gameTime.TotalGameTime < lastClicked.Add(new TimeSpan(0,0,0,0,Constants.CLICK_DELAY)))
							{
								Console.WriteLine(cardInfo + " was double clicked");
								nextClickAllowed = gameTime.TotalGameTime.Add(new TimeSpan(0,0,0,0,Constants.CLICK_DELAY));
								//lastClicked = default;

                                doubleClickCallback?.Invoke(this);

							//
                            } else {

								wasRecentlyClicked = true;
								lastClicked = gameTime.TotalGameTime;
								Console.WriteLine("First LB click " + cardInfo);
							}
						} else {
							
							wasRecentlyClicked = true;
							lastClicked = gameTime.TotalGameTime;
							Console.WriteLine("First LB click " + cardInfo);
						}

						isBeingClicked = false;

					}
				}

            }

        } else {
			isBeingClicked = false;
		}

	}

}
