using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;

using Sandbox.Systems;

namespace Sandbox;

public class MainGame : Game
{
    // ReSharper disable once NotAccessedField.Local
    private GraphicsDeviceManager _graphicsDeviceManager;

    private SpriteBatch _spriteBatch;

    private World _world;

    public MainGame()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
        Content.RootDirectory  = "Content";
        IsMouseVisible         = true;
    }

    protected override void LoadContent()
    {
        BitmapFont font = Content.Load<BitmapFont>(assetName: "Sensation");

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _world = new WorldBuilder().AddSystem(system: new RainfallSystem())
                                   .AddSystem(system: new ExpirySystem())
                                   .AddSystem(system: new RenderSystem(GraphicsDevice))
                                   .AddSystem(system: new HudSystem(GraphicsDevice, font))
                                   .Build();
    }

    protected override void UnloadContent()
    {
        _spriteBatch.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _world.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _world.Draw(gameTime);

        base.Draw(gameTime);
    }
}
