using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Sprites;

namespace Pong.GameObjects;

public class GameObject
{
    public Vector2 Position;

    public float Rotation;

    public Vector2 Scale = Vector2.One;

    public Sprite Sprite;

    public RectangleF BoundingRectangle => Sprite.GetBoundingRectangle(Position, rotation: 0, Vector2.One);
}
