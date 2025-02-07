using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Solitaire_Cracked_;

public enum Suit {
	J = 0,
	H,
	C,
	D,
	S
}

public class Card(Suit _suit, int _rank, GraphicsDevice gd)
{
    public bool isShowingFace { get; private set; }

	bool isJoker {
		get {
			return suit == Suit.J;
		}
	}

	public bool isTopmostCard;

	public int cardLayer;

	public readonly Suit suit = _suit;
	public readonly int rank = _rank; // J = 11, Q = 12, K = 13

	Texture2D cardImg;
	Texture2D cardBack;

    private GraphicsDevice graphicsDevice = gd;
	private readonly ContentManager content = Game1.GetNewContentManagerInstance();

    private SpriteBatch _spriteBatch;

	public Rectangle cardPos;

	public Rectangle movingCardPos;

	public bool isMoving { get; private set; }

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
					cardSuit = "Hearts";
					break;
				case Suit.C:
					cardSuit = "Clubs";
					break;
				case Suit.D:
					cardSuit = "Diamonds";
					break;
				case Suit.S:
					cardSuit = "Spades";
					break;
			}

			return cardRank + " of " + cardSuit;
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

	public void setCardMoving(bool isMoving)
	{
		this.isMoving = isMoving;
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
			_spriteBatch.Draw(texture: cardImg, 
								destinationRectangle: isMoving ? movingCardPos : cardPos, 
								color: Color.White,
								layerDepth: cardLayer,
								rotation: 0,
								origin: new Vector2(0, 0),
								effects: SpriteEffects.None,
								sourceRectangle: null);
		} else {
			_spriteBatch.Draw(cardBack, new Vector2(cardPos.X, cardPos.Y), Color.White);
		}

		_spriteBatch.End();

	}

}
