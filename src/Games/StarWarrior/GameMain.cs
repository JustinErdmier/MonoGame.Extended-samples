using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;

using StarWarrior.Components;
using StarWarrior.Systems;

namespace StarWarrior;

public class GameMain : Game
{
    private readonly FramesPerSecondCounter _fpsCounter = new();

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private readonly Random _random = new();

    private EntityFactory _entityFactory;

    private BitmapFont _font;

    private SpriteBatch _spriteBatch;

    private World _world;

    public GameMain()
    {
        Content.RootDirectory    = "Content";
        IsMouseVisible           = true;
        Window.AllowUserResizing = false;
        IsFixedTimeStep          = true;

        _graphicsDeviceManager = new GraphicsDeviceManager(game: this)
        {
            IsFullScreen                   = false,
            PreferredBackBufferWidth       = 800,
            PreferredBackBufferHeight      = 600,
            PreferredBackBufferFormat      = SurfaceFormat.Color,
            PreferMultiSampling            = false,
            PreferredDepthStencilFormat    = DepthFormat.None,
            SynchronizeWithVerticalRetrace = true
        };
    }

    protected override void LoadContent()
    {
        _entityFactory = new EntityFactory();
        _spriteBatch   = new SpriteBatch(GraphicsDevice);
        _font          = Content.Load<BitmapFont>(assetName: "montserrat-32");

        _world = new WorldBuilder().AddSystem(system: new CollisionSystem())
                                   .AddSystem(system: new EnemyShipMovementSystem(GraphicsDevice))
                                   .AddSystem(system: new EnemyShooterSystem(_entityFactory))
                                   .AddSystem(system: new EnemySpawnSystem(GraphicsDevice, _entityFactory))
                                   .AddSystem(system: new ExpirationSystem())
                                   .AddSystem(system: new HealthBarRenderSystem(_spriteBatch, _font))
                                   .AddSystem(system: new HudRenderSystem(GraphicsDevice, _spriteBatch, _font))
                                   .AddSystem(system: new MovementSystem())
                                   .AddSystem(system: new PlayerShipControlSystem(_entityFactory))
                                   .AddSystem(system: new RenderSystem(_spriteBatch, Content))
                                   .Build();

        _entityFactory.World = _world;

        InitializePlayerShip();
        InitializeEnemyShips();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboard     = Keyboard.GetState();
        GamePadState  gamePadState = GamePad.GetState(PlayerIndex.One);

        if (gamePadState.Buttons.Back == ButtonState.Pressed || keyboard.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _fpsCounter.Update(gameTime);
        _world.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _fpsCounter.Draw(gameTime);
        string fps = $"FPS: {_fpsCounter.FramesPerSecond}";

        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        _world.Draw(gameTime);

        _spriteBatch.DrawString(_font, fps, position: new Vector2(x: 16, y: 16), Color.White);

        //#if DEBUG
        //            var entityCount = $"Active Entities Count: {_entityManager.ActiveEntitiesCount}";
        //            //var removedEntityCount = $"Removed Entities TotalCount: {_ecs.TotalEntitiesRemovedCount}";
        //            var totalEntityCount = $"Allocated Entities Count: {_entityManager.TotalEntitiesCount}";

        //            _spriteBatch.DrawString(_font, entityCount, new Vector2(16, 62), Color.White);
        //            _spriteBatch.DrawString(_font, totalEntityCount, new Vector2(16, 92), Color.White);
        //            //_spriteBatch.DrawString(_font, removedEntityCount, new Vector2(32, 122), Color.Yellow);
        //#endif

        _spriteBatch.End();
    }

    private void InitializeEnemyShips()
    {
        Viewport viewport = GraphicsDevice.Viewport;

        Random random = new();

        for (int index = 0; 2 > index; ++index)
        {
            Entity entity = _entityFactory.CreateEnemyShip();

            Transform2 transform = entity.Get<Transform2>();

            Vector2 position = new()
            {
                X = random.Next(maxValue: viewport.Width - 100) + 50, Y = random.Next(maxValue: (int)(viewport.Height * 0.75 + 0.5)) + 50
            };

            transform.Position = position;

            PhysicsComponent physics = entity.Get<PhysicsComponent>();
            physics.Speed = 0.05f;
            physics.Angle = random.Next() % 2 == 0 ? 0 : 180;
        }
    }

    private void InitializePlayerShip()
    {
        Viewport viewport = GraphicsDevice.Viewport;

        Entity entity = _world.CreateEntity();
        entity.Attach(component: new Transform2(x: viewport.Width * 0.5f, y: viewport.Height - 50f));
        entity.Attach(component: new SpatialFormComponent { SpatialFormFile = "PlayerShip" });
        entity.Attach(component: new HealthComponent { Points               = 30, MaximumPoints = 30 });
        entity.Attach(component: new PlayerComponent());
    }
}
