using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

using SpaceGame.Entities;

namespace SpaceGame;

public class Game1 : Game
{
    private readonly EntityManager _entityManager;

    // ReSharper disable once NotAccessedField.Local
    private readonly GraphicsDeviceManager _graphicsDeviceManager;

    private Texture2D _backgroundTexture;

    private BulletFactory _bulletFactory;

    private OrthographicCamera _camera;

    //private SpriteSheetAnimationFactory _explosionAnimations;
    private BitmapFont _font;

    private MeteorFactory _meteorFactory;

    private Spaceship _player;

    private MouseState _previousMouseState;

    private int _score;

    private SpriteBatch _spriteBatch;

    private ViewportAdapter _viewportAdapter;

    public Game1()
    {
        _graphicsDeviceManager = new GraphicsDeviceManager(game: this);
        _entityManager         = new EntityManager();

        Content.RootDirectory    = "Content";
        Window.AllowUserResizing = true;
        Window.Position          = Point.Zero;
        IsMouseVisible           = true;
    }

    //protected override void Initialize()
    //{
    //    base.Initialize();

    //    _graphicsDeviceManager.IsFullScreen = true;
    //    _graphicsDeviceManager.PreferredBackBufferWidth = GraphicsDevice.DisplayMode.Width;
    //    _graphicsDeviceManager.PreferredBackBufferHeight = GraphicsDevice.DisplayMode.Height;
    //    _graphicsDeviceManager.ApplyChanges();
    //}

    protected override void LoadContent()
    {
        _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480);
        _font            = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");

        _camera = new OrthographicCamera(_viewportAdapter);
        //_explosionAnimations = Content.Load<SpriteSheetAnimationFactory>("explosion-animations");

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _backgroundTexture = Content.Load<Texture2D>(assetName: "black");

        Texture2D       bulletTexture = Content.Load<Texture2D>(assetName: "laserBlue03");
        TextureRegion2D bulletRegion  = new(bulletTexture);
        _bulletFactory = new BulletFactory(_entityManager, bulletRegion);

        SpawnPlayer(_bulletFactory);

        _meteorFactory = new MeteorFactory(_entityManager, Content);

        for (int i = 0; i < 13; i++)
        {
            _meteorFactory.SpawnNewMeteor(_player.Position);
        }
    }

    private void SpawnPlayer(IBulletFactory bulletFactory)
    {
        Texture2D       spaceshipTexture = Content.Load<Texture2D>(assetName: "playerShip1_blue");
        TextureRegion2D spaceshipRegion  = new(spaceshipTexture);
        _player = _entityManager.AddEntity(entity: new Spaceship(spaceshipRegion, bulletFactory));
    }

    protected override void UnloadContent()
    { }

    protected override void Update(GameTime gameTime)
    {
        float         deltaTime     = gameTime.GetElapsedSeconds();
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState    mouseState    = Mouse.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        if (_player != null && !_player.IsDestroyed)
        {
            const float acceleration = 5f;

            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                _player.Accelerate(acceleration);
            }

            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                _player.Accelerate(-acceleration);
            }

            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                _player.Rotation += deltaTime * 3f;
            }

            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                _player.Rotation -= deltaTime * 3f;
            }

            if (keyboardState.IsKeyDown(Keys.Space) || mouseState.LeftButton == ButtonState.Pressed)
            {
                _player.Fire();
            }

            if (_previousMouseState.X != mouseState.X || _previousMouseState.Y != mouseState.Y)
            {
                _player.LookAt(point: _camera.ScreenToWorld(screenPosition: new Vector2(mouseState.X, mouseState.Y)));
            }

            _camera.LookAt(position: _player.Position + _player.Velocity * 0.2f);
            _camera.Zoom = 1.0f - _player.Velocity.Length() / 500f;
        }

        _entityManager.Update(gameTime);

        CheckCollisions();

        _previousMouseState = mouseState;

        base.Update(gameTime);
    }

    private void CheckCollisions()
    {
        Meteor[] meteors = _entityManager.Entities.Where(e => e is Meteor)
                                         .Cast<Meteor>()
                                         .ToArray();

        Laser[] lasers = _entityManager.Entities.Where(e => e is Laser)
                                       .Cast<Laser>()
                                       .ToArray();

        foreach (Meteor meteor in meteors)
        {
            if (_player != null && !_player.IsDestroyed && _player.BoundingCircle.Intersects(meteor.BoundingCircle))
            {
                Explode(meteor.Position, meteor.Size);
                Explode(_player.Position, radius: 3);

                _player.Destroy();
                _player = null;
                meteor.Destroy();
            }

            foreach (Laser laser in lasers.Where(laser => meteor.Contains(laser.Position)))
            {
                meteor.Damage(damage: 1);
                laser.Destroy();
                _score++;

                Explode(laser.Position, meteor.Size);

                if (meteor.Size >= 2)
                {
                    _meteorFactory.SplitMeteor(meteor);
                }
            }

            if (_player != null && _player.ShieldHealth > 0 && meteor.BoundingCircle.Intersects(circle: new CircleF(_player.Position, _player.ShieldRadius)))
            {
                _player.HitShield();
                Explode(meteor.Position, meteor.Size);
                meteor.Destroy();
            }
        }
    }

    private void Explode(Vector2 position, float radius)
    {
        //var explosion = new Explosion(_explosionAnimations, position, radius);
        //_entityManager.AddEntity(explosion);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // background
        Rectangle sourceRectangle = new(x: 0, y: 0, _viewportAdapter.VirtualWidth, _viewportAdapter.VirtualHeight);
        sourceRectangle.Offset(amount: _camera.Position * new Vector2(value: 0.1f));

        _spriteBatch.Begin(samplerState: SamplerState.PointWrap, transformMatrix: _viewportAdapter.GetScaleMatrix());
        _spriteBatch.Draw(_backgroundTexture, Vector2.Zero, sourceRectangle, Color.White);
        _spriteBatch.DrawString(_font, text: $"{_score}", Vector2.One, Color.White);
        _spriteBatch.End();

        // entities
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp, blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
        _entityManager.Draw(_spriteBatch);
        _spriteBatch.End();

        _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix());

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
