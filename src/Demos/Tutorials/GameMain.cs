using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Gui;
using MonoGame.Extended.ViewportAdapters;

using Tutorials.Demos;

namespace Tutorials;

public class GameMain : Game
{
    private readonly Dictionary<string, DemoBase> _demos;

    private readonly FramesPerSecondCounter _fpsCounter = new();

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private DemoBase _currentDemo;

    private GuiSystem _guiSystem;

    private KeyboardState _previousKeyboardState;

    private SelectDemoScreen _selectDemoScreen;

    public GameMain(PlatformConfig config)
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(game: this)
        {
            IsFullScreen = config.IsFullScreen, SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight
        };

        Content.RootDirectory    = "Content";
        IsMouseVisible           = true;
        Window.AllowUserResizing = true;

        _demos = new DemoBase[]
        {
            new ShapesDemo(game: this), new ViewportAdaptersDemo(game: this), new CollisionDemo(game: this), new TiledMapsDemo(game: this),
            new AnimationsDemo(game: this), new SpritesDemo(game: this), new BatchingDemo(game: this), new InputListenersDemo(game: this),
            new ParticlesDemo(game: this), new CameraDemo(game: this), new BitmapFontsDemo(parent: this)
        }.ToDictionary(d => d.Name);
    }

    public ViewportAdapter ViewportAdapter { get; private set; }

    protected override void Dispose(bool disposing)
    {
        foreach (DemoBase demo in _demos.Values)
        {
            demo.Dispose();
        }

        base.Dispose(disposing);
    }

    protected override void Initialize()
    {
        base.Initialize();

        // TODO: Allow switching to full-screen mode from the UI
        //if (_isFullScreen)
        //{
        //    _graphicsDeviceManager.IsFullScreen = true;
        //    _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
        //    _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
        //    _graphicsDeviceManager.ApplyChanges();
        //}
    }

    protected override void LoadContent()
    {
        ViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480);

        //var skin = GuiSkin.FromFile(Content, @"Raw/adventure-gui-skin.json");
        GuiSpriteBatchRenderer guiRenderer = new(GraphicsDevice, ViewportAdapter.GetScaleMatrix);

        BitmapFont font = Content.Load<BitmapFont>(assetName: "small-font");
        BitmapFont.UseKernings = false;
        Skin.CreateDefault(font);
        _selectDemoScreen = new SelectDemoScreen(_demos, LoadDemo, Exit);

        _guiSystem = new GuiSystem(ViewportAdapter, guiRenderer) { ActiveScreen = _selectDemoScreen };
    }

    private void LoadDemo(string name)
    {
        IsMouseVisible = true;
        _currentDemo?.Unload();
        _currentDemo?.Dispose();
        _currentDemo = _demos[name];
        _currentDemo.Load();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape) && _previousKeyboardState.IsKeyUp(Keys.Escape))
        {
            Back();
        }

        _fpsCounter.Update(gameTime);
        _guiSystem.Update(gameTime);
        _currentDemo?.OnUpdate(gameTime);

        _previousKeyboardState = keyboardState;
        base.Update(gameTime);
    }

    public void Back()
    {
        if (_selectDemoScreen.IsVisible)
        {
            Exit();
        }

        IsMouseVisible = true;
        _currentDemo?.Unload();
        _currentDemo?.Dispose();
        _currentDemo                = null;
        _selectDemoScreen.IsVisible = true;
        _guiSystem.ActiveScreen     = _selectDemoScreen;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _fpsCounter.Draw(gameTime);
        Window.Title = $"{_currentDemo?.Name} {_fpsCounter.FramesPerSecond}";

        base.Draw(gameTime);

        _currentDemo?.OnDraw(gameTime);

        _guiSystem.Draw(gameTime);
    }
}
