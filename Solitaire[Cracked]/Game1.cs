using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Solitaire_Cracked_;

public class Game1 : Game
{
    private static GraphicsDeviceManager _graphics;
    public static ContentManager content;
    public GraphicsDevice device;

    private static SettingsManager settings;
    private DeckManager deckManager = new();
    private static InputManager inputManager;
    
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        content = Content;

        IsMouseVisible = true;

        settings = new SettingsManager(_graphics);

        inputManager = new InputManager(deckManager);

        settings.changeResolution(Constants.BASE_WIDTH, Constants.BASE_HEIGHT);

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

        deckManager.LoadContent(content);

        base.LoadContent();

    }

    protected override void Update(GameTime gameTime)
    {
        if(inputManager.isExitGameButtonDown())
            Exit();

        inputManager.Update(gameTime);

        if(inputManager.shouldRestartGame)
        {
            deckManager.restartGame();
        }

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
    /// https://community.monogame.net/t/passing-the-contentmanager-to-every-class-feels-wrong-is-it/10470/9
    /// Code copied from the above link for more finessed content managers
    /// </summary>
    /// <returns></returns>
    // public static ContentManager GetNewContentManagerInstance()
    // {
    //   // create a new content manager instance
    //   ContentManager temp = new ContentManager(content.ServiceProvider, content.RootDirectory);
    //   temp.RootDirectory = "Content";
    //   return temp;
    // }

    public static InputManager GetInputManager()
    {
        return inputManager;
    }

    public static SettingsManager GetSettingsManager()
    {
        return settings;
    }

    // may not be needed
    public GraphicsDevice GetGraphicsDevice()
    {
        return device;
    }

}
