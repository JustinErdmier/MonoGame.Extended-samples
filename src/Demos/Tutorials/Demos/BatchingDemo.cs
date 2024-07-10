using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Graphics.Effects;

namespace Tutorials.Demos;

public struct SpriteInfo
{
    public Vector2 Position;

    public float Rotation;

    public Color Color;

    public Texture2D Texture;

    public Matrix2 TransformMatrix;
}

public class BatchingDemo : DemoBase
{
    private readonly Random _random = new();

    private readonly SpriteInfo[] _sprites = new SpriteInfo[2048];

    private Batcher2D _batcher;

    private BitmapFont _bitmapFont;

    private DefaultEffect _effect;

    private Matrix _projectionMatrix;

    private SpriteBatch _spriteBatch;

    private Vector2 _spriteOrigin;

    private Vector2 _spriteScale;

    private Texture2D _spriteTexture1;

    private Texture2D _spriteTexture2;

    private Matrix _viewMatrix;

    private Matrix _worldMatrix;

    public BatchingDemo(GameMain game)
        : base(game)
    {
        // disable fixed time step so max frames can be measured otherwise the update & draw frames would be capped to the default 60 fps timestep
        //game.IsFixedTimeStep = false;

        //_graphicsDeviceManager = new GraphicsDeviceManager(this)
        //{
        //    // also disable v-sync so max frames can be measured otherwise draw frames would be capped to the screen's refresh rate 
        //    SynchronizeWithVerticalRetrace = false,
        //    PreferredBackBufferWidth = 800,
        //    PreferredBackBufferHeight = 600
        //};
    }

    public override string Name => "Batching";

    protected override void LoadContent()
    {
        GraphicsDevice graphicsDevice = GraphicsDevice;

        _effect      = new DefaultEffect(graphicsDevice) { TextureEnabled = true, VertexColorEnabled = true };
        _batcher     = new Batcher2D(graphicsDevice);
        _spriteBatch = new SpriteBatch(graphicsDevice);
        _bitmapFont  = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");

        // load the texture for the sprites
        _spriteTexture1 = Content.Load<Texture2D>(assetName: "Textures/logo-square-128");
        _spriteTexture2 = Content.Load<Texture2D>(assetName: "Textures/logo-square-512");
        _spriteOrigin   = new Vector2(x: _spriteTexture1.Width * 0.5f, y: _spriteTexture1.Height * 0.5f);
        _spriteScale    = new Vector2(value: 0.5f);

        Viewport viewport = GraphicsDevice.Viewport;

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int index = 0; index < _sprites.Length; index++)
        {
            SpriteInfo sprite = _sprites[index];
            sprite.Position = new Vector2(x: _random.Next(viewport.X, viewport.Width), y: _random.Next(viewport.Y, viewport.Height));
            sprite.Rotation = MathHelper.ToRadians(degrees: _random.Next(minValue: 0, maxValue: 360));
            sprite.Texture  = index % 2 == 0 ? _spriteTexture1 : _spriteTexture2;
            _sprites[index] = sprite;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard     = Keyboard.GetState();
        GamePadState  gamePadState = GamePad.GetState(PlayerIndex.One);

        if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int index = 0; index < _sprites.Length; index++)
        {
            SpriteInfo sprite = _sprites[index];

            if (index % 2 == 0)
            {
                sprite.Rotation = (sprite.Rotation + MathHelper.ToRadians(degrees: 0.5f)) % MathHelper.TwoPi;
            }
            else
            {
                sprite.Rotation = (sprite.Rotation - MathHelper.ToRadians(degrees: 0.5f) + MathHelper.TwoPi) % MathHelper.TwoPi;
            }

            sprite.Color = ColorHelper.FromHsl(hue: sprite.Rotation / MathHelper.TwoPi, saturation: 0.5f, lightness: 0.3f);

            sprite.TransformMatrix = Matrix2.CreateFrom(sprite.Position, sprite.Rotation, _spriteScale, _spriteOrigin);

            _sprites[index] = sprite;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice graphicsDevice = GraphicsDevice;

        graphicsDevice.Clear(Color.CornflowerBlue);

        // update the matrices
        _worldMatrix = Matrix.Identity;
        _viewMatrix  = _effect.View = Matrix.Identity;

        _projectionMatrix = _effect.Projection =
            Matrix.CreateOrthographicOffCenter(left: 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, top: 0, zNearPlane: 0, zFarPlane: -1);

        // comment and uncomment either of the two below lines to compare

        DrawSpritesWithBatcher2D();
        //DrawSpritesWithSpriteBatch();

        base.Draw(gameTime);
    }

    private void DrawSpritesWithBatcher2D()
    {
        _batcher.Begin(_viewMatrix, _projectionMatrix, effect: _effect);

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int index = 0; index < _sprites.Length; index++)
        {
            SpriteInfo sprite = _sprites[index];
            _batcher.DrawTexture(sprite.Texture, ref sprite.TransformMatrix, sprite.Color);
        }

        _batcher.End();
    }

    private void DrawSpritesWithSpriteBatch()
    {
        _effect.Projection = _projectionMatrix;
        _effect.View       = _viewMatrix;
        _spriteBatch.Begin(SpriteSortMode.Texture, effect: _effect);

        // ReSharper disable once ForCanBeConvertedToForeach
        for (int index = 0; index < _sprites.Length; index++)
        {
            SpriteInfo sprite = _sprites[index];

            _spriteBatch.Draw(sprite.Texture,
                              sprite.Position,
                              sourceRectangle: null,
                              sprite.Color,
                              sprite.Rotation,
                              _spriteOrigin,
                              _spriteScale,
                              SpriteEffects.None,
                              layerDepth: 0);
        }

        _spriteBatch.End();
    }
}
