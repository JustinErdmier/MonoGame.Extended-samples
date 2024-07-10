using System;

using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Screens;
using MonoGame.Extended.Screens.Transitions;

using Pong.Screens;

namespace Pong;

public class GameMain : Game
{
    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphics;

    private readonly ScreenManager _screenManager;

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(game: this)
        {
            PreferredBackBufferWidth = 800, PreferredBackBufferHeight = 480, SynchronizeWithVerticalRetrace = false
        };

        Content.RootDirectory = "Content";
        IsFixedTimeStep       = true;
        TargetElapsedTime     = TimeSpan.FromSeconds(value: 1f / 60f);

        _screenManager = Components.Add<ScreenManager>();
    }

    protected override void LoadContent()
    {
        base.LoadContent();

        _screenManager.LoadScreen(screen: new TitleScreen(game: this), transition: new FadeTransition(GraphicsDevice, Color.Black, duration: 0.5f));
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardExtended.Refresh();
        MouseExtended.Refresh();
    }
}
