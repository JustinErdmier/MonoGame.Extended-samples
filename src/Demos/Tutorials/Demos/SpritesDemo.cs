using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Tutorials.Demos;

public class SpritesDemo : DemoBase
{
    private Sprite _apple;

    private Sprite _axeSprite;

    private Texture2D _backgroundTexture;

    private Rectangle _clippingRectangle = new(x: 50 + 32, y: 250 - 32, width: 64, height: 128 + 64);

    private TextureRegion2D _clippingTextureRegion;

    private float _particleOpacity;

    private Sprite _particleSprite0;

    private Sprite _particleSprite1;

    private MouseState _previousMouseState;

    private Sprite _spikeyBallSprite;

    private SpriteBatch _spriteBatch;

    public SpritesDemo(GameMain game)
        : base(game)
    { }

    public override string Name => "Sprites";

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _backgroundTexture = Content.Load<Texture2D>(assetName: "Textures/bg_sharbi");

        TextureRegion2D testRegion = new(texture: Content.Load<Texture2D>(assetName: "Textures/clipping-test"));
        _clippingTextureRegion = new NinePatchRegion2D(testRegion, padding: 16);

        Texture2D appleTexture = Content.Load<Texture2D>(assetName: "Sprites/apple");
        _apple = new Sprite(appleTexture);

        Texture2D axeTexture = Content.Load<Texture2D>(assetName: "Textures/axe");

        _axeSprite = new Sprite(axeTexture)
        {
            Origin = new Vector2(x: 243, y: 679)
            //Position = new Vector2(400, 0),
            //Scale = Vector2.One * 0.5f
        };

        Texture2D spikeyBallTexture = Content.Load<Texture2D>(assetName: "Textures/spike_ball");

        _spikeyBallSprite = new Sprite(spikeyBallTexture)
        {
            //Position = new Vector2(400, 340)
        };

        Texture2D particleTexture = Content.Load<Texture2D>(assetName: "Textures/particle");

        _particleSprite0 = new Sprite(particleTexture)
        {
            //Position = new Vector2(600, 340)
        };

        _particleSprite1 = new Sprite(particleTexture)
        {
            //Position = new Vector2(200, 340)
        };

        _particleOpacity = 0.0f;
    }

    protected override void UnloadContent()
    { }

    protected override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

        KeyboardState keyboardState = Keyboard.GetState();
        MouseState    mouseState    = Mouse.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        //_axeSprite.Rotation = MathHelper.ToRadians(180) + MathHelper.PiOver2 * 0.8f * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);

        //_spikeyBallSprite.Rotation -= deltaTime * 2.5f;
        //_spikeyBallSprite.Position = new Vector2(mouseState.X, mouseState.Y);

        _particleOpacity       = 0.5f + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds);
        _particleSprite0.Color = Color.White * _particleOpacity;
        _particleSprite1.Color = Color.White * (1.0f - _particleOpacity);

        int dx = mouseState.X - _previousMouseState.X;
        int dy = mouseState.Y - _previousMouseState.Y;

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _clippingRectangle.X += dx;
            _clippingRectangle.Y += dy;
        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            _clippingRectangle.Width  += dx;
            _clippingRectangle.Height += dy;
        }

        _previousMouseState = mouseState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        _spriteBatch.Draw(_backgroundTexture,
                          destinationRectangle: new Rectangle(x: 0, y: 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                          Color.White);
        //_spriteBatch.Draw(_axeSprite);
        //_spriteBatch.Draw(_spikeyBallSprite);
        //_spriteBatch.Draw(_particleSprite0);
        //_spriteBatch.Draw(_particleSprite1);

        // clipping test
        _spriteBatch.Draw(_clippingTextureRegion,
                          destinationRectangle: new Rectangle(x: 50, y: 50, width: 128, height: 128),
                          Color.White,
                          clippingRectangle: null);

        _spriteBatch.Draw(_clippingTextureRegion, destinationRectangle: new Rectangle(x: 50, y: 250, width: 512, height: 512), Color.White, _clippingRectangle);
        _spriteBatch.DrawRectangle(rectangle: _clippingRectangle.ToRectangleF(), Color.White);

        _spriteBatch.Draw(_apple, position: new Vector2(x: 100, y: 100));

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
