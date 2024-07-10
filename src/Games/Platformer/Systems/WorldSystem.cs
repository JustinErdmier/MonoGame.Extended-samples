using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

using Platformer.Collisions;

using World = Platformer.Collisions.World;

namespace Platformer.Systems;

public class WorldSystem : EntityProcessingSystem
{
    private readonly World _world;

    private ComponentMapper<Body> _bodyMapper;

    private ComponentMapper<Transform2> _transformMapper;

    public WorldSystem()
        : base(aspectBuilder: Aspect.All(typeof(Body), typeof(Transform2))) =>
        _world = new World(gravity: new Vector2(x: 0, y: 60));

    public override void Initialize(IComponentMapperService mapperService)
    {
        _transformMapper = mapperService.GetMapper<Transform2>();
        _bodyMapper      = mapperService.GetMapper<Body>();
    }

    protected override void OnEntityAdded(int entityId)
    {
        Body body = _bodyMapper.Get(entityId);
        _world.AddBody(body);
    }

    protected override void OnEntityRemoved(int entityId)
    {
        Body body = _bodyMapper.Get(entityId);
        _world.RemoveBody(body);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _world.Update(deltaTime: gameTime.GetElapsedSeconds());
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        Transform2 transform = _transformMapper.Get(entityId);
        Body       body      = _bodyMapper.Get(entityId);
        transform.Position = body.Position;
    }
}
