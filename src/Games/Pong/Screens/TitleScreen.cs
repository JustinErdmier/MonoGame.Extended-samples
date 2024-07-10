using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

namespace Pong.Screens;

public class TitleScreen : GameScreen
{
    private Texture2D _background;

    private SpriteBatch _spriteBatch;

    public TitleScreen(Game game)
        : base(game) =>
        game.IsMouseVisible = true;

    public override void LoadContent()
    {
        base.LoadContent();
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _background  = Content.Load<Texture2D>(assetName: "title-screen");
    }

    public override void Update(GameTime gameTime)
    {
        MouseStateExtended    mouseState    = MouseExtended.GetState();
        KeyboardStateExtended keyboardState = KeyboardExtended.GetState();

        if (keyboardState.IsKeyReleased(Keys.Escape))
        {
            Game.Exit();
        }

        if (mouseState.LeftButton == ButtonState.Pressed || keyboardState.WasAnyKeyJustDown())
        {
            ScreenManager.LoadScreen(screen: new PongGameScreen(Game), transition: new FadeTransition(GraphicsDevice, Color.Black, duration: 0.5f));
        }
    }

    public override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Magenta);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _spriteBatch.Draw(_background,
                          destinationRectangle: new Rectangle(x: 0, y: 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                          Color.White);

        _spriteBatch.End();
    }
}
