using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace SpaceGame.Entities;

public class Meteor : Entity
{
    private const float _radius = 55f / 4f;

    private readonly Sprite _sprite;

    private readonly Transform2 _transform = new();

    public CircleF BoundingCircle;

    public Meteor(TextureRegion2D textureRegion, Vector2 position, Vector2 velocity, float rotationSpeed, int size)
    {
        _sprite        = new Sprite(textureRegion);
        BoundingCircle = new CircleF(_transform.Position, radius: _radius * size);

        Position      = position;
        Velocity      = velocity;
        RotationSpeed = rotationSpeed;
        HealthPoints  = 1;
        Size          = size;
    }

    public int HealthPoints { get; private set; }

    public Vector2 Position
    {
        get => _transform.Position;
        set
        {
            _transform.Position   = value;
            BoundingCircle.Center = _transform.Position;
        }
    }

    public float Rotation
    {
        get => _transform.Rotation;
        set => _transform.Rotation = value;
    }

    public float RotationSpeed { get; }

    public int Size { get; private set; }

    public Vector2 Velocity { get; set; }

    public void Damage(int damage)
    {
        HealthPoints -= damage;

        if (HealthPoints <= 0)
        {
            Destroy();
        }
    }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * deltaTime;
        Rotation += RotationSpeed * deltaTime;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, _transform);
    }

    public bool Contains(Vector2 position) => BoundingCircle.Contains(position);
}
