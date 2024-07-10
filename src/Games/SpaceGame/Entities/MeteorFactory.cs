using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;

namespace SpaceGame.Entities;

public class MeteorFactory
{
    private readonly EntityManager _entityManager;

    private readonly Dictionary<int, TextureRegion2D[]> _meteorRegions;

    private readonly Random _random = new();

    public MeteorFactory(EntityManager entityManager, ContentManager contentManager)
    {
        _entityManager = entityManager;

        _meteorRegions = new Dictionary<int, TextureRegion2D[]>
        {
            {
                4,
                new[]
                {
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_big1")),
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_big2")),
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_big3")),
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_big4"))
                }
            },
            {
                3,
                new[]
                {
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_med1")),
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_med3"))
                }
            },
            {
                2,
                new[]
                {
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_small1")),
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_small2"))
                }
            },
            {
                1,
                new[]
                {
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_tiny1")),
                    new TextureRegion2D(texture: contentManager.Load<Texture2D>(assetName: "meteorBrown_tiny2"))
                }
            }
        };
    }

    private TextureRegion2D GetMeteorRegion(int size)
    {
        TextureRegion2D[] regions = _meteorRegions[size];
        int               index   = _random.Next(minValue: 0, regions.Length);

        return regions[index];
    }

    public void SplitMeteor(Meteor meteor)
    {
        if (meteor.Size <= 1)
        {
            throw new InvalidOperationException(message: "Meteor size less than 2 can't be split");
        }

        for (int i = 0; i < 2; i++)
        {
            int     size          = meteor.Size - 1;
            float   rotationSpeed = _random.Next(minValue: -10, maxValue: 10) * 0.1f;
            Vector2 spawnPosition = meteor.Position;

            Vector2 velocity = i == 0 ?
                meteor.Velocity.PerpendicularClockwise() + meteor.Velocity :
                meteor.Velocity.PerpendicularCounterClockwise() + meteor.Velocity;

            TextureRegion2D textureRegion = GetMeteorRegion(size);
            Meteor          newMeteor     = new(textureRegion, spawnPosition, velocity, rotationSpeed, size);

            _entityManager.AddEntity(newMeteor);
        }
    }

    public void SpawnNewMeteor(Point2 playerPosition)
    {
        float   rotationSpeed = _random.Next(minValue: -10, maxValue: 10) * 0.1f;
        CircleF spawnCircle   = new(playerPosition, radius: 630);
        float   spawnAngle    = MathHelper.ToRadians(degrees: _random.Next(minValue: 0, maxValue: 360));
        Point2  spawnPosition = spawnCircle.BoundaryPointAt(spawnAngle);

        Vector2 velocity = (playerPosition - spawnPosition).Rotate(radians: MathHelper.ToRadians(degrees: _random.Next(minValue: -15, maxValue: 15))) *
                           _random.Next(minValue: 3, maxValue: 10) *
                           0.01f;

        TextureRegion2D textureRegion = GetMeteorRegion(size: 4);
        Meteor          meteor        = new(textureRegion, spawnPosition, velocity, rotationSpeed, size: 3);

        _entityManager.AddEntity(meteor);
    }
}
