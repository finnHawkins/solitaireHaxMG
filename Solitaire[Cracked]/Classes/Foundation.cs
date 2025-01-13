using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Solitaire_Cracked_;

// use for Foundations but also use for draw/discard piles
public class Foundation : CardStackBase
{
    public delegate void CallbackEventHandler();
    public event CallbackEventHandler Callback; 

    protected bool isClicked;
	
	public Foundation(stackType sType, int stackCount)
	{

        stackType = sType;
        stackCounter = stackCount;

        baseCardPosition = getCardRectangle();
		
		cardPile = new List<Card>();
		
	}


    public override void Update(GameTime gameTime) {

        var im = Game1.GetInputManager();
        var ms = im.GetMouseState();

        if(baseCardPosition.Contains(new Vector2(ms.X, ms.Y)))
        {

            if(im.isLeftMouseButtonDown())
            {
                isClicked = true;

            } else if (im.isLeftMouseButtonReleased() && isClicked)
            {

                if(stackType == stackType.drawPile)
                {
                    if (Callback != null)
                        Callback();
                }

                isClicked = false;
            }

        }

    }
	
}
