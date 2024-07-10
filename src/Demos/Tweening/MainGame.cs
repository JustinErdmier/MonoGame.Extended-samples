using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tweening;

namespace Tweening;

public class MainGame : Game
{
    private readonly Tweener _tweener = new();

    private BitmapFont _bitmapFont;

    // ReSharper disable once NotAccessedField.Local
    private GraphicsDeviceManager _graphicsDeviceManager;

    private SpriteBatch _spriteBatch;

    public Vector2 Back = new(x: 200, y: 250);

    public Vector2 Bounce = new(x: 200, y: 200);

    public Vector2 Elastic = new(x: 200, y: 300);

    public Vector2 Exponential = new(x: 200, y: 150);

    public Vector2 Linear = new(x: 200, y: 50);

    public Vector2 Quadratic = new(x: 200, y: 100);

    public Vector2 Size = new(x: 50, y: 50);

    public MainGame()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
        Content.RootDirectory  = "Content";
        IsMouseVisible         = true;
        IsFixedTimeStep        = true;
        TargetElapsedTime      = TimeSpan.FromSeconds(value: 1f / 60f);
    }

    protected override void LoadContent()
    {
        _bitmapFont = Content.Load<BitmapFont>(assetName: "kenney-rocket-square");

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _tweener.TweenTo(target: this, a => a.Linear, toValue: new Vector2(x: 550, y: 50), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.Linear);

        _tweener.TweenTo(target: this, a => a.Quadratic, toValue: new Vector2(x: 550, y: 100), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.QuadraticInOut);

        _tweener.TweenTo(target: this, a => a.Exponential, toValue: new Vector2(x: 550, y: 150), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.ExponentialInOut);

        _tweener.TweenTo(target: this, a => a.Bounce, toValue: new Vector2(x: 550, y: 200), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.BounceOut);

        _tweener.TweenTo(target: this, a => a.Back, toValue: new Vector2(x: 550, y: 250), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.BackOut);

        _tweener.TweenTo(target: this, a => a.Elastic, toValue: new Vector2(x: 550, y: 300), duration: 2, delay: 1)
                .RepeatForever(repeatDelay: 0.2f)
                .AutoReverse()
                .Easing(EasingFunctions.ElasticOut);
    }

    protected override void UnloadContent()
    {
        _spriteBatch.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardExtended.Refresh();
        MouseExtended.Refresh();

        KeyboardStateExtended keyboardState  = KeyboardExtended.GetState();
        MouseStateExtended    mouseState     = MouseExtended.GetState();
        float                 elapsedSeconds = gameTime.GetElapsedSeconds();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (keyboardState.IsKeyReleased(Keys.Space))
        {
            _tweener.CancelAll();
        }

        if (keyboardState.IsKeyReleased(Keys.Tab))
        {
            _tweener.CancelAndCompleteAll();
        }

        if (mouseState.IsButtonDown(MouseButton.Left))
        {
            _tweener.TweenTo(target: this, a => a.Linear, toValue: mouseState.Position.ToVector2(), duration: 1.0f)
                    .Easing(EasingFunctions.QuadraticOut);
        }

        _tweener.Update(elapsedSeconds);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.FillRectangle(Linear.X, Linear.Y, Size.X, Size.X, Color.Red);
        _spriteBatch.FillRectangle(Quadratic.X, Quadratic.Y, Size.X, Size.X, Color.Green);
        _spriteBatch.FillRectangle(Exponential.X, Exponential.Y, Size.X, Size.X, Color.Blue);
        _spriteBatch.FillRectangle(Bounce.X, Bounce.Y, Size.X, Size.X, Color.DarkOrange);
        _spriteBatch.FillRectangle(Back.X, Back.Y, Size.X, Size.X, Color.Purple);
        _spriteBatch.FillRectangle(Elastic.X, Elastic.Y, Size.X, Size.X, Color.Yellow);

        _spriteBatch.DrawString(_bitmapFont, text: $"{_tweener.AllocationCount}", Vector2.One, Color.WhiteSmoke);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
