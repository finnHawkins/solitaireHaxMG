
using Microsoft.Xna.Framework;

public class SettingsManager {

private static SettingsManager instance = null;
    private static readonly object padlock = new object();

    public static SettingsManager Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new SettingsManager();
                }
                return instance;
            }
        }
    }

    // 1 = 640 x 360
    // 2 = 1280 x 720
    // 3 = 1920 x 1080
    public int resolutionMultiplier = 1;

    public bool cheatsEnabled;

    // TODO - implement
    public bool timerEnabled;

    //TODO - implement
    public bool scoreEnabled;

    /// <summary>
    /// Controls whether 1 or 3 cards are drawn from the draw pile when clicked
    /// </summary>
    public bool drawThree = false;

    public bool useDarkMode = false;

    private GraphicsDeviceManager _graphics;

    public void assignGdm(GraphicsDeviceManager graphics)
    {
        _graphics = graphics;
    }

    /// <summary>
    /// Modifies the Graphics Manager's Preferred Back Buffer width and height to given values.
    /// </summary>
    /// <param name="width">PreferredBackBufferWidth</param>
    /// <param name="height">PreferredBackBufferHeight</param>
    public void changeResolution(int width, int height)
    {
        _graphics.PreferredBackBufferWidth = width * resolutionMultiplier;
        _graphics.PreferredBackBufferHeight = height * resolutionMultiplier;
        _graphics.ApplyChanges();
    }

}