using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos;

public class ViewportAdaptersDemo : DemoBase
{
    private Texture2D _backgroundTexture;

    private BitmapFont _bitmapFont;

    private BoxingViewportAdapter _boxingViewportAdapter;

    private ViewportAdapter _currentViewportAdapter;

    private DefaultViewportAdapter _defaultViewportAdapter;

    private Point _mousePosition;

    private ScalingViewportAdapter _scalingViewportAdapter;

    private SpriteBatch _spriteBatch;

    public ViewportAdaptersDemo(GameMain game)
        : base(game)
    { }

    public override string Name => "Viewport Adapters";

    protected override void Initialize()
    {
        base.Initialize();

        // the default viewport adapater is the simplest, it doesn't do any scaling at all
        // but is used by a Camera2D if no other adapter is specified.
        // this is often useful if you have a game with a large map and you want the player to see 
        // more of the map on a bigger screen.
        _defaultViewportAdapter = new DefaultViewportAdapter(GraphicsDevice);

        // the scaling viewport adapter stretches the output to fit in the viewport, ignoring the aspect ratio
        // this works well if the aspect ratio doesn't change a lot between devices 
        // or you don't like the black bars of the boxing adapter
        _scalingViewportAdapter = new ScalingViewportAdapter(GraphicsDevice, virtualWidth: 800, virtualHeight: 480);

        // the boxing viewport adapter uses letterboxing or pillarboxing to maintain aspect ratio
        // it's a little more complicated and needs to listen to the window client size changing event
        _boxingViewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480); //, 88, 70);

        // typically you'll only ever want to use one viewport adapter for a game, but in this sample we'll be 
        // switching between them.
        _currentViewportAdapter = _boxingViewportAdapter;
    }

    protected override void LoadContent()
    {
        _spriteBatch       = new SpriteBatch(GraphicsDevice);
        _backgroundTexture = Content.Load<Texture2D>(assetName: "Textures/vignette");
        _bitmapFont        = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");
    }

    protected override void UnloadContent()
    { }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState   keyboardState           = Keyboard.GetState();
        MouseState      mouseState              = Mouse.GetState();
        ViewportAdapter previousViewportAdapter = _currentViewportAdapter;

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            SwitchAdapter(_defaultViewportAdapter);
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            SwitchAdapter(_scalingViewportAdapter);
        }

        if (keyboardState.IsKeyDown(Keys.B))
        {
            SwitchAdapter(_boxingViewportAdapter);
        }

        // if we've changed the viewport adapter mid game we need to reset the viewport back to the window size
        // this wouldn't normally be required if you're only ever using one viewport adapter
        if (previousViewportAdapter != _currentViewportAdapter)
        {
            GraphicsDevice.Viewport = new Viewport(x: 0, y: 0, Window.ClientBounds.Width, Window.ClientBounds.Height);
            _currentViewportAdapter.Reset();
        }

        // the viewport adapters can also scale mouse and touch input to the virtual resolution
        _mousePosition = _currentViewportAdapter.PointToScreen(mouseState.X, mouseState.Y);

        base.Update(gameTime);
    }

    private void SwitchAdapter(ViewportAdapter viewportAdapter)
    {
        _currentViewportAdapter?.Dispose();
        _currentViewportAdapter = viewportAdapter;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // when rendering sprites, you'll always work within the bounds of the virtual width and height
        // specified when setting up the viewport adapter. The default MonoGame window is 800x480.
        Rectangle destinationRectangle = new(x: 0, y: 0, width: 800, height: 480);

        _spriteBatch.Begin(transformMatrix: _currentViewportAdapter.GetScaleMatrix());
        _spriteBatch.Draw(_backgroundTexture, destinationRectangle, Color.White);
        _spriteBatch.DrawString(_bitmapFont, text: $"Press D: {typeof(DefaultViewportAdapter).Name}", position: new Vector2(x: 49, y: 40), Color.White);

        _spriteBatch.DrawString(_bitmapFont,
                                text: $"Press S: {typeof(ScalingViewportAdapter).Name}",
                                position: new Vector2(x: 49, y: 40 + _bitmapFont.LineHeight * 1),
                                Color.White);

        _spriteBatch.DrawString(_bitmapFont,
                                text: $"Press B: {typeof(BoxingViewportAdapter).Name}",
                                position: new Vector2(x: 49, y: 40 + _bitmapFont.LineHeight * 2),
                                Color.White);

        _spriteBatch.DrawString(_bitmapFont,
                                text: $"Current: {_currentViewportAdapter.GetType().Name}",
                                position: new Vector2(x: 49, y: 40 + _bitmapFont.LineHeight * 4),
                                Color.Black);

        _spriteBatch.DrawString(_bitmapFont, text: @"Try resizing the window", position: new Vector2(x: 49, y: 40 + _bitmapFont.LineHeight * 6), Color.Black);
        _spriteBatch.DrawString(_bitmapFont, text: $"Mouse: {_mousePosition}", position: new Vector2(x: 49, y: 40 + _bitmapFont.LineHeight * 8), Color.Black);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
