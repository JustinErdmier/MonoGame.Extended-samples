using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos;

public class InputListenersDemo : DemoBase
{
    private const float _cursorBlinkDelay = 0.5f;

    private const int _maxLogLines = 13;

    private readonly List<string> _logLines = new();

    private readonly GameMain _this;

    private Texture2D _backgroundTexture;

    private BitmapFont _bitmapFont;

    private OrthographicCamera _camera;

    private float _cursorBlinkDelta = _cursorBlinkDelay;

    private bool _isCursorVisible = true;

    private SpriteBatch _spriteBatch;

    private string _typedString = string.Empty;

    public InputListenersDemo(GameMain game)
        : base(game) =>
        _this = game;

    public override string Name => "Input Listeners";

    protected override void Initialize()
    {
        MouseListener    mouseListener    = new(settings: new MouseListenerSettings());
        KeyboardListener keyboardListener = new(settings: new KeyboardListenerSettings());

        Components.Add(item: new InputListenerComponent(_this, mouseListener, keyboardListener));

        keyboardListener.KeyPressed += (sender, args) =>
        {
            if (args.Key == Keys.Escape)
            {
                Exit();
            }
        };

        mouseListener.MouseClicked       += (sender, args) => LogMessage(messageFormat: "{0} mouse button clicked", args.Button);
        mouseListener.MouseDoubleClicked += (sender, args) => LogMessage(messageFormat: "{0} mouse button double-clicked", args.Button);
        mouseListener.MouseDown          += (sender, args) => LogMessage(messageFormat: "{0} mouse button down", args.Button);
        mouseListener.MouseUp            += (sender, args) => LogMessage(messageFormat: "{0} mouse button up", args.Button);
        mouseListener.MouseDrag          += (sender, args) => LogMessage(messageFormat: "Mouse dragged");
        mouseListener.MouseWheelMoved    += (sender, args) => LogMessage(messageFormat: "Mouse scroll wheel value {0}", args.ScrollWheelValue);

        keyboardListener.KeyPressed  += (sender, args) => LogMessage(messageFormat: "{0} key pressed", args.Key);
        keyboardListener.KeyReleased += (sender, args) => LogMessage(messageFormat: "{0} key released", args.Key);

        keyboardListener.KeyTyped += (sender, args) =>
        {
            if (args.Key == Keys.Back && _typedString.Length > 0)
            {
                _typedString = _typedString.Substring(startIndex: 0, length: _typedString.Length - 1);
            }
            else if (args.Key == Keys.Enter)
            {
                LogMessage(_typedString);
                _typedString = string.Empty;
            }
            else
            {
                _typedString += args.Character?.ToString() ?? "";
            }
        };

        LogMessage(messageFormat: "Do something with the mouse or keyboard...");

        base.Initialize();
    }

    private void LogMessage(string messageFormat, params object[] args)
    {
        string message = string.Format(messageFormat, args);

        if (_logLines.Count == _maxLogLines)
        {
            _logLines.RemoveAt(index: 0);
        }

        _logLines.Add(message);
    }

    protected override void LoadContent()
    {
        BoxingViewportAdapter viewportAdapter = new(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480);
        _camera      = new OrthographicCamera(viewportAdapter);
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _backgroundTexture = Content.Load<Texture2D>(assetName: "Textures/vignette");
        _bitmapFont        = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");
    }

    protected override void UnloadContent()
    { }

    protected override void Update(GameTime gameTime)
    {
        _cursorBlinkDelta -= (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (_cursorBlinkDelta <= 0)
        {
            _isCursorVisible  = !_isCursorVisible;
            _cursorBlinkDelta = _cursorBlinkDelay;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());

        _spriteBatch.Draw(_backgroundTexture,
                          destinationRectangle: new Rectangle(x: 0, y: 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                          Color.DarkSlateGray);

        for (int i = 0; i < _logLines.Count; i++)
        {
            string logLine = _logLines[i];
            _spriteBatch.DrawString(_bitmapFont, logLine, position: new Vector2(x: 4, y: i * _bitmapFont.LineHeight), color: Color.LightGray * 0.2f);
        }

        int        textInputY      = 14 * _bitmapFont.LineHeight - 2;
        Point2     position        = new(x: 4, textInputY);
        RectangleF stringRectangle = _bitmapFont.GetStringRectangle(_typedString, position);

        _spriteBatch.DrawString(_bitmapFont, _typedString, position, Color.White);

        if (_isCursorVisible)
        {
            _spriteBatch.DrawString(_bitmapFont, text: "_", position: new Vector2(stringRectangle.Width, textInputY), Color.White);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
