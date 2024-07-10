using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceGame.Entities;

public interface IEntityManager
{
    T AddEntity<T>(T entity)
        where T : Entity;
}

public class EntityManager : IEntityManager
{
    private readonly List<Entity> _entities;

    public EntityManager() => _entities = new List<Entity>();

    public IEnumerable<Entity> Entities => _entities;

    public T AddEntity<T>(T entity)
        where T : Entity
    {
        _entities.Add(entity);

        return entity;
    }

    public void Update(GameTime gameTime)
    {
        foreach (Entity entity in _entities.Where(e => !e.IsDestroyed))
        {
            entity.Update(gameTime);
        }

        _entities.RemoveAll(e => e.IsDestroyed);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (Entity entity in _entities.Where(e => !e.IsDestroyed))
        {
            entity.Draw(spriteBatch);
        }
    }
}
