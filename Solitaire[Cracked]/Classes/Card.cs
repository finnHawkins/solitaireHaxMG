using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

public enum Suit
{
	J = 0,
	H,
	C,
	D,
	S
}

public class Card(Suit _suit, int _rank)
{
	public bool isShowingFace { get; private set; }

	bool isJoker
	{
		get
		{
			return suit == Suit.J;
		}
	}

	public bool isTopmostCard;

	public int cardLayer;

	public readonly Suit suit = _suit;
	public readonly int rank = _rank; // J = 11, Q = 12, K = 13

	Texture2D cardImg;
	Texture2D cardBack;

	public Rectangle cardPos;

	public string cardInfo
	{
		get
		{

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

	public bool isRed
	{
		get
		{
			return (int)suit % 2 == 0;
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

	public void LoadContent(ContentManager content)
	{

		cardBack = content.Load<Texture2D>("BACK_RED");

		string cardName = suit.ToString() + "_" + rank;

		cardImg = content.Load<Texture2D>(cardName);

	}

	public void resetCard()
	{
		isShowingFace = false;
		cardLayer = 0;
		isTopmostCard = false;
	}

	public void Draw(SpriteBatch spriteBatch)
	{

		try
		{
			
			if (isShowingFace)
			{
				// spriteBatch.Draw(texture: cardImg,
				// 					destinationRectangle: cardPos,
				// 					color: Color.White,
				// 					layerDepth: cardLayer,
				// 					rotation: 0,
				// 					origin: new Vector2(0, 0),
				// 					effects: SpriteEffects.None,
				// 					sourceRectangle: null);
				spriteBatch.Draw(cardImg, new Vector2(cardPos.X, cardPos.Y), Color.White);
			} else {
				spriteBatch.Draw(cardBack, new Vector2(cardPos.X, cardPos.Y), Color.White);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error: {ex.Message}");
			throw;
		}

	}

}
