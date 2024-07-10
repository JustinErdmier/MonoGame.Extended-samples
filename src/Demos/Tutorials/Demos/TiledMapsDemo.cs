using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics.Effects;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos;

public class TiledMapsDemo : DemoBase
{
    private Queue<string> _availableMaps;

    private BitmapFont _bitmapFont;

    private OrthographicCamera _camera;

    private Effect _customEffect;

    private TiledMap _map;

    private TiledMapRenderer _mapRenderer;

    private KeyboardState _previousKeyboardState;

    private bool _showHelp;

    private SpriteBatch _spriteBatch;

    private ViewportAdapter _viewportAdapter;

    public TiledMapsDemo(GameMain game)
        : base(game)
    { }

    public override string Name => "Tiled Maps";

    public override void Dispose()
    {
        _mapRenderer?.Dispose();
        base.Dispose();
    }

    protected override void Initialize()
    {
        _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, virtualWidth: 1024, virtualHeight: 768);
        _camera          = new OrthographicCamera(_viewportAdapter);

        Window.AllowUserResizing = true;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _bitmapFont  = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");

        _availableMaps = new Queue<string>(collection: new[] { "level01", "level02", "level03", "level04", "level05", "level06", "level07", "level08" });

        _map = LoadNextMap();
        _camera.LookAt(position: new Vector2(_map.WidthInPixels, _map.HeightInPixels) * 0.5f);
        _camera.Position = new Vector2(x: -104, y: -92);

        CustomEffect effect = new(GraphicsDevice) { Alpha = 0.5f, TextureEnabled = true, VertexColorEnabled = false };

        _customEffect = effect;
    }

    private TiledMap LoadNextMap()
    {
        string name = _availableMaps.Dequeue();
        _map = Content.Load<TiledMap>(assetName: $"TiledMaps/{name}");
        _availableMaps.Enqueue(name);
        _mapRenderer = new TiledMapRenderer(GraphicsDevice, _map);

        return _map;
    }

    protected override void UnloadContent()
    { }

    protected override void Update(GameTime gameTime)
    {
        float         deltaSeconds  = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState keyboardState = Keyboard.GetState();

        _mapRenderer.Update(gameTime);

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        const float cameraSpeed = 500f;
        const float zoomSpeed   = 0.3f;

        Vector2 moveDirection = Vector2.Zero;

        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
        {
            moveDirection -= Vector2.UnitY;
        }

        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
        {
            moveDirection -= Vector2.UnitX;
        }

        if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
        {
            moveDirection += Vector2.UnitY;
        }

        if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
        {
            moveDirection += Vector2.UnitX;
        }

        // need to normalize the direction vector incase moving diagonally, but can't normalize the zero vector
        // however, the zero vector means we didn't want to move this frame anyways so all good
        bool isCameraMoving = moveDirection != Vector2.Zero;

        if (isCameraMoving)
        {
            moveDirection.Normalize();
            _camera.Move(direction: moveDirection * cameraSpeed * deltaSeconds);
        }

        if (keyboardState.IsKeyDown(Keys.R))
        {
            _camera.ZoomIn(deltaZoom: zoomSpeed * deltaSeconds);
        }

        if (keyboardState.IsKeyDown(Keys.F))
        {
            _camera.ZoomOut(deltaZoom: zoomSpeed * deltaSeconds);
        }

        if (_previousKeyboardState.IsKeyDown(Keys.Tab) && keyboardState.IsKeyUp(Keys.Tab))
        {
            _map = LoadNextMap();
            LookAtMapCenter();
        }

        if (_previousKeyboardState.IsKeyDown(Keys.H) && keyboardState.IsKeyUp(Keys.H))
        {
            _showHelp = !_showHelp;
        }

        if (keyboardState.IsKeyDown(Keys.Z))
        {
            _camera.Position = Vector2.Zero;
        }

        if (keyboardState.IsKeyDown(Keys.X))
        {
            _camera.LookAt(Vector2.Zero);
        }

        if (keyboardState.IsKeyDown(Keys.C))
        {
            LookAtMapCenter();
        }

        _previousKeyboardState = keyboardState;

        base.Update(gameTime);
    }

    private void LookAtMapCenter()
    {
        switch (_map.Orientation)
        {
            case TiledMapOrientation.Orthogonal:
                _camera.LookAt(position: new Vector2(_map.WidthInPixels, _map.HeightInPixels) * 0.5f);

                break;

            case TiledMapOrientation.Isometric:
                _camera.LookAt(position: new Vector2(x: 0, y: _map.HeightInPixels + _map.TileHeight) * 0.5f);

                break;

            case TiledMapOrientation.Staggered:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.BlendState              = BlendState.AlphaBlend;
        GraphicsDevice.SamplerStates[index: 0] = SamplerState.PointClamp;
        GraphicsDevice.RasterizerState         = RasterizerState.CullNone;

        Matrix viewMatrix = _camera.GetViewMatrix();

        Matrix projectionMatrix =
            Matrix.CreateOrthographicOffCenter(left: 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, top: 0, zNearPlane: 0f, zFarPlane: -1f);

        _mapRenderer.Draw(ref viewMatrix, ref projectionMatrix, _customEffect);

        DrawText();

        base.Draw(gameTime);
    }

    private void DrawText()
    {
        Color textColor = Color.Black;
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend);

        Point2 baseTextPosition = new(x: 5, y: 0);
        Point2 textPosition     = baseTextPosition;

        _spriteBatch.DrawString(_bitmapFont,
                                text:
                                $"Map: {_map.Name}; {_map.TileLayers.Count} tile layer(s) @ {_map.Width}x{_map.Height} tiles, {_map.ImageLayers.Count} image layer(s)",
                                textPosition,
                                textColor);

        // we can safely get the metrics without worrying about spritebatch interfering because spritebatch submits on End()
        textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 1);
        _spriteBatch.DrawString(_bitmapFont, text: $"Camera Position: (x={_camera.Position.X}, y={_camera.Position.Y})", textPosition, textColor);

        if (!_showHelp)
        {
            _spriteBatch.DrawString(_bitmapFont, text: "H: Show help", position: new Vector2(x: 5, y: _bitmapFont.LineHeight * 2), textColor);
        }
        else
        {
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 2);
            _spriteBatch.DrawString(_bitmapFont, text: "H: Hide help", textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 3);
            _spriteBatch.DrawString(_bitmapFont, text: "WASD/Arrows: Pan camera", textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 4);
            _spriteBatch.DrawString(_bitmapFont, text: "RF: Zoom camera in / out", textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 5);
            _spriteBatch.DrawString(_bitmapFont, text: "Z: Move camera to the origin", textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 6);
            _spriteBatch.DrawString(_bitmapFont, text: "X: Move camera to look at the origin", textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 7);
            _spriteBatch.DrawString(_bitmapFont, text: "C: Move camera to look at center of the map", textPosition, textColor);
            textPosition = baseTextPosition + new Vector2(x: 0, y: _bitmapFont.LineHeight * 8);
            _spriteBatch.DrawString(_bitmapFont, text: "Tab: Cycle through maps", textPosition, textColor);
        }

        _spriteBatch.End();
    }
}

public class CustomEffect : DefaultEffect, ITiledMapEffect
{
    public CustomEffect(GraphicsDevice graphicsDevice)
        : base(graphicsDevice)
    { }

    public CustomEffect(GraphicsDevice graphicsDevice, byte[] byteCode)
        : base(graphicsDevice, byteCode)
    { }

    public CustomEffect(Effect cloneSource)
        : base(cloneSource)
    { }
}
