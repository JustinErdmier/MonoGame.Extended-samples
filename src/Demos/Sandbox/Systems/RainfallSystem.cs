using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

using Sandbox.Components;

namespace Sandbox.Systems;

public sealed class RainfallSystem : EntityUpdateSystem
{
    private const float MinSpawnDelay = 0.0f;

    private const float MaxSpawnDelay = 0.0f;

    private readonly FastRandom _random = new();

    private ComponentMapper<Expiry> _expiryMapper;

    private ComponentMapper<Raindrop> _raindropMapper;

    private float _spawnDelay = MaxSpawnDelay;

    private ComponentMapper<Transform2> _transformMapper;

    public RainfallSystem()
        : base(aspectBuilder: Aspect.All(typeof(Transform2), typeof(Raindrop)))
    { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _raindropMapper  = mapperService.GetMapper<Raindrop>();
        _expiryMapper    = mapperService.GetMapper<Expiry>();
    }

    public override void Update(GameTime gameTime)
    {
        float elapsedSeconds = gameTime.GetElapsedSeconds();

        foreach (int entityId in ActiveEntities)
        {
            Transform2 transform = _transformMapper.Get(entityId);
            Raindrop   raindrop  = _raindropMapper.Get(entityId);

            raindrop.Velocity  += new Vector2(x: 0, y: 500) * elapsedSeconds;
            transform.Position += raindrop.Velocity * elapsedSeconds;

            if (!(transform.Position.Y >= 480) || _expiryMapper.Has(entityId))
            {
                continue;
            }

            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = new(x: _random.NextSingle(min: -100, max: 100), y: -raindrop.Velocity.Y * _random.NextSingle(min: 0.1f, max: 0.2f));

                int id = CreateRaindrop(position: transform.Position.SetY(y: 479), velocity, size: (i + 1) * 0.5f);
                _expiryMapper.Put(id, component: new Expiry(timeRemaining: 1f));
            }

            DestroyEntity(entityId);
        }

        _spawnDelay -= gameTime.GetElapsedSeconds();

        if (!(_spawnDelay <= 0))
        {
            return;
        }

        for (int q = 0; q < 50; q++)
        {
            Vector2 position = new(x: _random.NextSingle(min: 0, max: 800), y: _random.NextSingle(min: -240, max: -480));
            CreateRaindrop(position);
        }

        _spawnDelay = _random.NextSingle(MinSpawnDelay, MaxSpawnDelay);
    }

    private int CreateRaindrop(Vector2 position, Vector2 velocity = default, float size = 3)
    {
        // TODO: entity templates
        // TODO: component pooling
        Entity entity = CreateEntity();
        entity.Attach(component: new Transform2(position));
        entity.Attach(component: new Raindrop { Velocity = velocity, Size = size });

        return entity.Id;
    }
}
