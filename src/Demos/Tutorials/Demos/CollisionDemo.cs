using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Collisions;
using MonoGame.Extended.Sprites;

namespace Tutorials.Demos;

public class CollisionDemo : DemoBase
{
    private List<DemoActor> _actors;

    private Texture2D _blankTexture;

    private CollisionComponent _collisionComponent;

    private DemoBall _controllableBall;

    private Texture2D _spikyBallTexture;

    private SpriteBatch _spriteBatch;

    public CollisionDemo(GameMain game)
        : base(game)
    { }

    public override string Name { get; } = "Collisions";

    protected override void LoadContent()
    {
        _collisionComponent = new CollisionComponent(boundary: new RectangleF(x: -10000, y: -5000, width: 20000, height: 10000));
        _actors             = new List<DemoActor>();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D spikeyBallTexture = Content.Load<Texture2D>(assetName: "Textures/spike_ball");
        _spikyBallTexture = spikeyBallTexture;

        _blankTexture = new Texture2D(GraphicsDevice, width: 1, height: 1);
        _blankTexture.SetData(data: new[] { Color.WhiteSmoke });

        DemoBall spikeyBallRight = new(sprite: new Sprite(_spikyBallTexture)) { Position = new Vector2(x: 600, y: 240), Velocity = new Vector2(x: 0, y: 120) };

        _actors.Add(spikeyBallRight);

        ControllableBall controllableBall = new(sprite: new Sprite(_spikyBallTexture))
        {
            Position = new Vector2(x: 400, y: 240), Velocity = new Vector2(x: 0, y: 0)
        };

        _actors.Add(controllableBall);
        _controllableBall = controllableBall;

        DemoWall topWall = new(sprite: new Sprite(_blankTexture))
        {
            Bounds = new RectangleF(x: 0, y: 0, width: 800, height: 20), Position = new Vector2(x: 0, y: 0)
        };

        _actors.Add(topWall);

        DemoWall bottomWall = new(sprite: new Sprite(_blankTexture))
        {
            Position = new Vector2(x: 0, y: 460), Bounds = new RectangleF(x: 0, y: 0, width: 800, height: 20)
        };

        _actors.Add(bottomWall);

        StationaryBall spikeyBallCenter = new(sprite: new Sprite(_spikyBallTexture)) { Position = new Vector2(x: 400, y: 240), Velocity = Vector2.Zero };

        _actors.Add(spikeyBallCenter);

        foreach (DemoActor actor in _actors)
        {
            _collisionComponent.Insert(actor);
        }

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        UpdateControlledBall(gameTime, _controllableBall);

        foreach (DemoActor actor in _actors)
        {
            actor.Update(gameTime);
        }

        _collisionComponent.Update(gameTime);
        base.Update(gameTime);
    }

    private void UpdateControlledBall(GameTime gameTime, DemoActor actor)
    {
        KeyboardState kb    = Keyboard.GetState();
        float         speed = 150.0f;

        Vector2 position = actor.Position;
        float   distance = speed * gameTime.GetElapsedSeconds();

        if (kb.IsKeyDown(Keys.W))
        {
            position.Y -= distance;
        }

        if (kb.IsKeyDown(Keys.S))
        {
            position.Y += distance;
        }

        if (kb.IsKeyDown(Keys.A))
        {
            position.X -= distance;
        }

        if (kb.IsKeyDown(Keys.D))
        {
            position.X += distance;
        }

        actor.Position = position;
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

        foreach (DemoActor actor in _actors)
        {
            actor.Draw(_spriteBatch);
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}

internal class DemoActor : ICollisionActor, IUpdate
{
    private readonly Sprite _sprite;

    private Vector2 _position;

    public DemoActor(Sprite sprite)
    {
        _sprite = sprite;
        Bounds  = sprite.GetBoundingRectangle(transform: new Transform2());
    }

    /// <summary>Gets or sets the actor's position and updates teh actor's bounds position.</summary>
    public Vector2 Position
    {
        get => _position;
        set
        {
            _position       = value;
            Bounds.Position = value + Offset;
        }
    }

    /// <summary>Gets or sets how far the actor's collision bounds are offset from the actor's position.</summary>
    public Vector2 Offset { get; set; }

    /// <summary>Gets or sets the actor's velocity.</summary>
    public Vector2 Velocity { get; set; }

    /// <summary>Gets or sets the actor's collision bounds.</summary>
    public IShapeF Bounds { get; set; }

    public virtual void OnCollision(CollisionEventArgs collisionInfo)
    { }

    public virtual void Update(GameTime gameTime)
    {
        Position += gameTime.GetElapsedSeconds() * Velocity;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _sprite.Draw(spriteBatch, Position, rotation: 0, Vector2.One);
    }
}

internal class DemoWall : DemoActor
{
    public DemoWall(Sprite sprite)
        : base(sprite)
    { }
}

/// <summary>Ball that bounces on wall</summary>
internal class DemoBall : DemoActor
{
    public DemoBall(Sprite sprite)
        : base(sprite) =>
        Bounds = new CircleF(center: Position + Offset, radius: 60);

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Velocity *= -1;
        Position -= collisionInfo.PenetrationVector;
        base.OnCollision(collisionInfo);
    }
}

/// <summary>Ball that doesn't move when collided with.</summary>
internal class StationaryBall : DemoBall
{
    public StationaryBall(Sprite sprite)
        : base(sprite)
    { }

    public override void OnCollision(CollisionEventArgs collisionInfo)
    { }
}

/// <summary>Player controlled ball.</summary>
internal class ControllableBall : DemoBall
{
    public ControllableBall(Sprite sprite)
        : base(sprite) =>
        Bounds = new CircleF(center: Position + Offset, radius: 60);

    public override void OnCollision(CollisionEventArgs collisionInfo)
    {
        Position -= collisionInfo.PenetrationVector;
    }
}
