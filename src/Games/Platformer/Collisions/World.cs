using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Platformer.Collisions;

public class World
{
    private readonly List<Body> _dynamicBodies = new();

    private readonly List<Body> _staticBodies = new();

    public World(Vector2 gravity) => Gravity = gravity;

    public Vector2 Gravity { get; set; }

    public void AddBody(Body body)
    {
        if (body.BodyType == BodyType.Dynamic)
        {
            _dynamicBodies.Add(body);
        }
        else
        {
            _staticBodies.Add(body);
        }
    }

    public void RemoveBody(Body body)
    {
        if (body.BodyType == BodyType.Dynamic)
        {
            _dynamicBodies.Remove(body);
        }
        else
        {
            _staticBodies.Remove(body);
        }
    }

    public void Update(float deltaTime)
    {
        // apply gravity (and other forces?)
        foreach (Body body in _dynamicBodies)
        {
            body.Velocity += Gravity;
        }

        foreach (Body dynamicBody in _dynamicBodies)
        {
            dynamicBody.Position.X += dynamicBody.Velocity.X * deltaTime;
            ResolveCollisions(dynamicBody);

            dynamicBody.Position.Y += dynamicBody.Velocity.Y * deltaTime;
            ResolveCollisions(dynamicBody);
        }
    }

    private void ResolveCollisions(Body dynamicBody)
    {
        foreach (Body staticBody in _staticBodies)
        {
            Vector2 vector = staticBody.Position - dynamicBody.Position;

            if (CollisionTester.AabbAabb(dynamicBody.BoundingBox, staticBody.BoundingBox, vector, manifold: out Manifold manifold))
            {
                dynamicBody.Position -= manifold.Normal * manifold.Penetration;
                dynamicBody.Velocity =  dynamicBody.Velocity * new Vector2(x: Math.Abs(manifold.Normal.Y), y: Math.Abs(manifold.Normal.X));
            }
        }

        foreach (Body otherDynamicBody in _dynamicBodies)
        {
            if (dynamicBody != otherDynamicBody) //TODO: Equality should be implemented better than this
            {
                Vector2 vector = otherDynamicBody.Position - dynamicBody.Position;

                if (CollisionTester.AabbAabb(dynamicBody.BoundingBox, otherDynamicBody.BoundingBox, vector, manifold: out Manifold manifold))
                {
                    //Invert how the manifold is applied from how this is done with static bodyies. 
                    //This will create the effect that the dynamic body in motion is pushing the other dynamic body.
                    otherDynamicBody.Position += manifold.Normal * manifold.Penetration;
                    otherDynamicBody.Velocity =  otherDynamicBody.Velocity * new Vector2(x: Math.Abs(manifold.Normal.Y), y: Math.Abs(manifold.Normal.X));
                }
            }
        }
    }
}
