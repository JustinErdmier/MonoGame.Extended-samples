using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace SpaceGame.Entities;

public class Laser : Entity
{
    private readonly Sprite _sprite;

    private readonly Transform2 _transform;

    private float _timeToLive;

    public Laser(TextureRegion2D textureRegion, Vector2 velocity)
    {
        _timeToLive = 1.0f;
        _sprite     = new Sprite(textureRegion);
        _transform  = new Transform2 { Scale = Vector2.One * 0.5f };

        Velocity = velocity;
    }

    public Vector2 Position
    {
        get => _transform.Position;
        set => _transform.Position = value;
    }

    public float Rotation
    {
        get => _transform.Rotation;
        set => _transform.Rotation = value;
    }

    public Vector2 Velocity { get; set; }

    public override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Position += Velocity * deltaTime;

        _timeToLive -= deltaTime;

        if (_timeToLive <= 0)
        {
            Destroy();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_sprite, _transform);
    }
}
