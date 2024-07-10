using Microsoft.Xna.Framework;

namespace Platformer.Collisions;

public enum BodyType { Static, Dynamic }

public class Body
{
    public BodyType BodyType = BodyType.Static;

    public Vector2 Position;

    public Vector2 Size;

    public Vector2 Velocity;

    public AABB BoundingBox => new(min: Position - Size / 2f, max: Position + Size / 2f);
}
