using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Solitaire_Cracked_;

// use for Foundations but also use for draw piles
public class Foundation
{
	public List<Card> cardPile;

    public Rectangle rect;

    public delegate void CallbackEventHandler();
    public event CallbackEventHandler Callback; 

	public bool isFoundation { get; set; }
	public bool isDrawPile { get; set; }
	public bool isDiscardPile { get; set; }

    public int slotPosition;
    protected bool isClicked;
	
	public Foundation(bool _isFoundation = true, bool _isDrawPile = false, bool _isDiscardPile = false, int _slotPos = 0)
	{

        isFoundation = _isFoundation;
        isDiscardPile = _isDiscardPile;
        isDrawPile = _isDrawPile;
        slotPosition = _slotPos;

        rect = getCardRectangle();
		
		cardPile = new List<Card>();
		
	}

    public void LoadContent()
    {

        foreach(var card in cardPile)
        {
            card.LoadContent();
        }

    }

    public void Update() {

        var ms = Mouse.GetState();

        if(rect.Contains(new Vector2(ms.X, ms.Y)))
        {
            
            if(ms.LeftButton == ButtonState.Pressed)
            {
                isClicked = true;

            } else if (ms.LeftButton == ButtonState.Released && isClicked)
            {

                if(isDrawPile)
                {
                    if (Callback != null)
                        Callback();
                }

                isClicked = false;
            }

        }

        foreach (var  card in cardPile)
        {
            card.Update();
        }

    }

    public void Draw()
    {

        foreach(var card in cardPile)
        {
            card.Draw();
        }

    }

    public Rectangle getCardRectangle()
    {
        int offset = 0;

        //is foundation and as such, needs moving over by one card width
        if(isFoundation)
        {
            offset = Constants.CARD_WIDTH + 1;
        }
        
        int cardXpos = slotPosition + (Constants.CARD_WIDTH * (slotPosition - 1)) + offset;

        return new Rectangle(cardXpos,Constants.TOP_MARGIN,Constants.CARD_WIDTH, Constants.CARD_HEIGHT);
    }

    public void setCardPositions()
    {

        foreach(var card in cardPile)
        {

            card.cardPos = rect;

        }
    }
	
}
