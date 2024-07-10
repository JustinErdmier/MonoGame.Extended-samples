using System;

using MonoGame.Extended;
using MonoGame.Extended.Entities;

using StarWarrior.Components;

namespace StarWarrior;

public class EntityFactory
{
    // TODO: Remove this property injection.
    public World World { get; set; }

    public Entity CreateMissile()
    {
        Entity entity = World.CreateEntity();
        entity.Attach(component: new Transform2());
        entity.Attach(component: new SpatialFormComponent { SpatialFormFile = "Missile" });
        entity.Attach(component: new PhysicsComponent());
        entity.Attach(component: new ExpiresComponent { LifeTime = TimeSpan.FromMilliseconds(value: 2000) });

        return entity;
    }

    public Entity CreateShipExplosion()
    {
        Entity entity = World.CreateEntity();
        entity.Attach(component: new Transform2());
        entity.Attach(component: new SpatialFormComponent { SpatialFormFile = "ShipExplosion" });
        entity.Attach(component: new ExpiresComponent { LifeTime            = TimeSpan.FromMilliseconds(value: 1000) });

        return entity;
    }

    public Entity CreateBulletExplosion()
    {
        Entity entity = World.CreateEntity();
        entity.Attach(component: new Transform2());
        entity.Attach(component: new SpatialFormComponent { SpatialFormFile = "BulletExplosion" });
        entity.Attach(component: new ExpiresComponent { LifeTime            = TimeSpan.FromMilliseconds(value: 1000) });

        return entity;
    }

    public Entity CreateEnemyShip()
    {
        Entity entity = World.CreateEntity();
        entity.Attach(component: new Transform2());
        entity.Attach(component: new SpatialFormComponent { SpatialFormFile = "EnemyShip" });
        entity.Attach(component: new HealthComponent { Points               = 10, MaximumPoints = 10 });
        entity.Attach(component: new WeaponComponent { ShootDelay           = TimeSpan.FromSeconds(value: 2) });
        entity.Attach(component: new PhysicsComponent());
        entity.Attach(component: new EnemyComponent());

        return entity;
    }
}
