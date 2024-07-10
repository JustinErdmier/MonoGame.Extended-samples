using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

namespace Sandbox.Systems;

public sealed class HudSystem : IDrawSystem
{
    private readonly BitmapFont _font;

    private readonly SpriteBatch _spriteBatch;

    private World _world;

    public HudSystem(GraphicsDevice graphicsDevice, BitmapFont font)
    {
        _font        = font;
        _spriteBatch = new SpriteBatch(graphicsDevice);
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    { }

    public void Initialize(World world) => _world = world;

    public void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        _spriteBatch.FillRectangle(x: 0, y: 0, width: 800, height: 20, color: Color.Black * 0.4f);
        _spriteBatch.DrawString(_font, text: $"entities: {_world.EntityCount}", Vector2.One, Color.White);
        _spriteBatch.End();
    }
}
