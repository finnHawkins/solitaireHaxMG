using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Solitaire_Cracked_;

public class Game1 : Game
{
    private static GraphicsDeviceManager _graphics;
    public static ContentManager content;
    public GraphicsDevice device;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        content = Content;

        IsMouseVisible = true;

        SettingsManager.Instance.assignGdm(_graphics);
        SettingsManager.Instance.changeResolution(Constants.BASE_WIDTH, Constants.BASE_HEIGHT);

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        DeckManager.Instance.Initialize(GraphicsDevice);

        // TODO - find better cursor
        //var mouseTexture = content.Load<Texture2D>("cursor_pointer");
        //Mouse.SetCursor(MouseCursor.FromTexture2D(mouseTexture, 0, 0));


        base.Initialize();
    }

    protected override void LoadContent()
    {

        DeckManager.Instance.LoadContent(content);

        base.LoadContent();

    }

    protected override void Update(GameTime gameTime)
    {
        if(InputManager.Instance.isExitGameButtonDown())
            Exit();

        InputManager.Instance.Update(gameTime);

        if(InputManager.Instance.shouldRestartGame)
        {
            DeckManager.Instance.restartGame();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.DarkGreen);

        // TODO: Add your drawing code here

        DeckManager.Instance.Draw();

        base.Draw(gameTime);
    }

}
