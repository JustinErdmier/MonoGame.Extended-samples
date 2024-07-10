using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

using Sandbox.Components;

namespace Sandbox.Systems;

public sealed class ExpirySystem : EntityProcessingSystem
{
    private ComponentMapper<Expiry> _expiryMapper;

    public ExpirySystem()
        : base(aspectBuilder: Aspect.All(typeof(Expiry)))
    { }

    public override void Initialize(IComponentMapperService mapperService) => _expiryMapper = mapperService.GetMapper<Expiry>();

    public override void Process(GameTime gameTime, int entityId)
    {
        Expiry expiry = _expiryMapper.Get(entityId);

        expiry.TimeRemaining -= gameTime.GetElapsedSeconds();

        if (expiry.TimeRemaining <= 0)
        {
            DestroyEntity(entityId);
        }
    }
}
