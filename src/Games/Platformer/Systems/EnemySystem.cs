using Microsoft.Xna.Framework;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Entities.Systems;

using Platformer.Collisions;
using Platformer.Components;

namespace Platformer.Systems;

public class EnemySystem : EntityProcessingSystem
{
    private ComponentMapper<Body> _bodyMapper;

    private ComponentMapper<Enemy> _enemyMapper;

    public EnemySystem()
        : base(aspectBuilder: Aspect.All(typeof(Body), typeof(Enemy)))
    { }

    public override void Initialize(IComponentMapperService mapperService)
    {
        _enemyMapper = mapperService.GetMapper<Enemy>();
        _bodyMapper  = mapperService.GetMapper<Body>();
    }

    public override void Process(GameTime gameTime, int entityId)
    {
        float elapsedSeconds = gameTime.GetElapsedSeconds();
        Body  body           = _bodyMapper.Get(entityId);
        Enemy enemy          = _enemyMapper.Get(entityId);

        enemy.TimeLeft -= elapsedSeconds;

        if (enemy.TimeLeft <= 0)
        {
            enemy.Speed    = -enemy.Speed;
            enemy.TimeLeft = 1.0f;
        }

        body.Position = body.Position.Translate(x: enemy.Speed * elapsedSeconds, y: 0);
    }
}
