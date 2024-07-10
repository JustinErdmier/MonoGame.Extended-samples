using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

using Sandbox.Components;

namespace Sandbox.Systems;

public sealed class RenderSystem : EntityDrawSystem
{
    private readonly GraphicsDevice _graphicsDevice;

    private readonly SpriteBatch _spriteBatch;

    private ComponentMapper<Raindrop> _raindropMapper;

    private ComponentMapper<Transform2> _transformMapper;

    public RenderSystem(GraphicsDevice graphicsDevice)
        : base(aspect: Aspect.All(typeof(Transform2), typeof(Raindrop)))
    {
        _graphicsDevice = graphicsDevice;
        _spriteBatch    = new SpriteBatch(graphicsDevice);
    }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _raindropMapper  = mapperService.GetMapper<Raindrop>();
    }

    public override void Draw(GameTime gameTime)
    {
        _graphicsDevice.Clear(color: Color.DarkBlue * 0.2f);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        foreach (int entity in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entity);
            Raindrop   raindrop  = _raindropMapper.Get(entity);

            _spriteBatch.FillRectangle(transform.Position, size: new Size2(raindrop.Size, raindrop.Size), Color.LightBlue);
        }

        _spriteBatch.End();
    }
}
