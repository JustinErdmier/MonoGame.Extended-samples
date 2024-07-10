using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos;

public class CameraDemo : DemoBase
{
    private const float _cloudsRepeatWidth = 800;

    private Texture2D _backgroundClouds;

    private Texture2D[] _backgroundHills;

    private Texture2D _backgroundSky;

    private BitmapFont _bitmapFont;

    private OrthographicCamera _camera;

    private float _cloudsOffset;

    private SpriteBatch _spriteBatch;

    private Vector2 _worldPosition;

    public CameraDemo(GameMain game)
        : base(game)
    { }

    public override string Name => "Camera";

    protected override void LoadContent()
    {
        BoxingViewportAdapter viewportAdapter = new(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480);
        _camera = new OrthographicCamera(viewportAdapter);

        _bitmapFont       = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");
        _backgroundSky    = Content.Load<Texture2D>(assetName: "Textures/hills-sky");
        _backgroundClouds = Content.Load<Texture2D>(assetName: "Textures/hills-clouds");

        _backgroundHills    = new Texture2D[4];
        _backgroundHills[0] = Content.Load<Texture2D>(assetName: "Textures/hills-1");
        _backgroundHills[1] = Content.Load<Texture2D>(assetName: "Textures/hills-2");
        _backgroundHills[2] = Content.Load<Texture2D>(assetName: "Textures/hills-3");
        _backgroundHills[3] = Content.Load<Texture2D>(assetName: "Textures/hills-4");

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    protected override void UnloadContent()
    { }

    protected override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _cloudsOffset -= deltaTime * 5;

        if (_cloudsOffset < -_cloudsRepeatWidth)
        {
            _cloudsOffset = _cloudsRepeatWidth;
        }

        KeyboardState keyboardState = Keyboard.GetState();
        MouseState    mouseState    = Mouse.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // the camera properties of the camera can be conrolled to move, zoom and rotate
        const float movementSpeed = 200;
        const float rotationSpeed = 0.5f;
        const float zoomSpeed     = 0.5f;

        if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
        {
            _camera.Move(direction: new Vector2(x: 0, -movementSpeed) * deltaTime);
        }

        if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
        {
            _camera.Move(direction: new Vector2(-movementSpeed, y: 0) * deltaTime);
        }

        if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
        {
            _camera.Move(direction: new Vector2(x: 0, movementSpeed) * deltaTime);
        }

        if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
        {
            _camera.Move(direction: new Vector2(movementSpeed, y: 0) * deltaTime);
        }

        if (keyboardState.IsKeyDown(Keys.E))
        {
            _camera.Rotation += rotationSpeed * deltaTime;
        }

        if (keyboardState.IsKeyDown(Keys.Q))
        {
            _camera.Rotation -= rotationSpeed * deltaTime;
        }

        if (keyboardState.IsKeyDown(Keys.R))
        {
            _camera.ZoomIn(deltaZoom: zoomSpeed * deltaTime);
        }

        if (keyboardState.IsKeyDown(Keys.F))
        {
            _camera.ZoomOut(deltaZoom: zoomSpeed * deltaTime);
        }

        _worldPosition = _camera.ScreenToWorld(screenPosition: new Vector2(mouseState.X, mouseState.Y));

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // the camera produces a view matrix that can be applied to any sprite batch
        Matrix transformMatrix = _camera.GetViewMatrix(Vector2.Zero);
        _spriteBatch.Begin(transformMatrix: transformMatrix);
        _spriteBatch.Draw(_backgroundSky, Vector2.Zero, Color.White);
        _spriteBatch.Draw(_backgroundClouds, position: new Vector2(-_cloudsOffset, y: 0), Color.White);
        _spriteBatch.Draw(_backgroundClouds, position: new Vector2(_cloudsOffset, y: 10), Color.White);
        _spriteBatch.End();

        for (int layerIndex = 0; layerIndex < 4; layerIndex++)
        {
            // different layers can have a parallax factor applied for a nice depth effect
            Vector2 parallaxFactor = Vector2.One * (0.25f * layerIndex);
            Matrix  viewMatrix     = _camera.GetViewMatrix(parallaxFactor);
            _spriteBatch.Begin(transformMatrix: viewMatrix);

            for (int repeatIndex = -3; repeatIndex <= 3; repeatIndex++)
            {
                Texture2D texture  = _backgroundHills[layerIndex];
                Vector2   position = new(x: repeatIndex * texture.Width, y: 0);
                _spriteBatch.Draw(texture, position, Color.White);
            }

            _spriteBatch.End();
        }

        // not all sprite batches need to be affected by the camera
        RectangleF    rectangle     = _camera.BoundingRectangle;
        StringBuilder stringBuilder = new();
        stringBuilder.AppendLine(handler: $"WASD: Move [{_camera.Position.X:0}, {_camera.Position.Y:0}]");
        stringBuilder.AppendLine(handler: $"EQ: Rotate [{MathHelper.ToDegrees(_camera.Rotation):0.00}]");
        stringBuilder.AppendLine(handler: $"RF: Zoom [{_camera.Zoom:0.00}]");
        stringBuilder.AppendLine(handler: $"World Pos: [{_worldPosition.X:0}, {_worldPosition.Y:0}]");
        stringBuilder.AppendLine(handler: $"Bounds: [{rectangle.X:0}, {rectangle.Y:0}, {rectangle.Width:0}, {rectangle.Height:0}]");

        _spriteBatch.Begin(blendState: BlendState.AlphaBlend);
        _spriteBatch.DrawString(_bitmapFont, text: stringBuilder.ToString(), position: new Vector2(x: 5, y: 5), Color.DarkBlue);
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
