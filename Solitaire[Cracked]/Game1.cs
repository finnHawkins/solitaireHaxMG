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

    private SettingsManager settings = new();
    private DeckManager deckManager = new();
    
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
        
        deckManager.Initialize(GraphicsDevice);

        // TODO - find better cursor
		//var mouseTexture = content.Load<Texture2D>("cursor_pointer");
        //Mouse.SetCursor(MouseCursor.FromTexture2D(mouseTexture, 0, 0));
        

        base.Initialize();
    }

    protected override void LoadContent()
    {

        deckManager.LoadContent();

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        deckManager.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGreen);

        // TODO: Add your drawing code here

        deckManager.Draw();

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

}
