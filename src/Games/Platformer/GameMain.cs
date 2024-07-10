using Autofac;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

using Platformer.Systems;

namespace Platformer;

public class GameMain : GameBase
{
    private OrthographicCamera _camera;

    private EntityFactory _entityFactory;

    private TiledMap _map;

    private TiledMapRenderer _renderer;

    private World _world;

    protected override void RegisterDependencies(ContainerBuilder builder)
    {
        _camera = new OrthographicCamera(GraphicsDevice);

        builder.RegisterInstance(instance: new SpriteBatch(GraphicsDevice));
        builder.RegisterInstance(_camera);
    }

    protected override void LoadContent()
    {
        _world = new WorldBuilder().AddSystem(system: new WorldSystem())
                                   .AddSystem(system: new PlayerSystem())
                                   .AddSystem(system: new EnemySystem())
                                   .AddSystem(system: new RenderSystem(spriteBatch: new SpriteBatch(GraphicsDevice), _camera))
                                   .Build();

        Components.Add(_world);

        _entityFactory = new EntityFactory(_world, Content);

        // TOOD: Load maps and collision data more nicely :)
        _map      = Content.Load<TiledMap>(assetName: "test-map");
        _renderer = new TiledMapRenderer(GraphicsDevice, _map);

        foreach (TiledMapTileLayer tileLayer in _map.TileLayers)
        {
            for (int x = 0; x < tileLayer.Width; x++)
            {
                for (int y = 0; y < tileLayer.Height; y++)
                {
                    TiledMapTile tile = tileLayer.GetTile(x: (ushort)x, y: (ushort)y);

                    if (tile.GlobalIdentifier == 1)
                    {
                        int tileWidth  = _map.TileWidth;
                        int tileHeight = _map.TileHeight;
                        _entityFactory.CreateTile(x, y, tileWidth, tileHeight);
                    }
                }
            }
        }

        _entityFactory.CreateBlue(position: new Vector2(x: 600, y: 240));
        _entityFactory.CreateBlue(position: new Vector2(x: 700, y: 100));
        _entityFactory.CreatePlayer(position: new Vector2(x: 100, y: 240));
    }

    protected override void Update(GameTime gameTime)
    {
        // TODO: Using global shared input state is really bad!

        KeyboardExtended.Refresh();
        MouseExtended.Refresh();

        //var keyboardState = KeyboardExtended.GetState();

        //if (keyboardState.IsKeyDown(Keys.Escape))
        //    Exit();

        _renderer.Update(gameTime);
        //_camera.LookAt(_playerEntity.Get<Transform2>().Position);

        //_world.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _renderer.Draw(viewMatrix: _camera.GetViewMatrix());
        //_world.Draw(gameTime);

        base.Draw(gameTime);
    }
}
