using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

using Platformer.Collisions;
using Platformer.Components;

using World = MonoGame.Extended.Entities.World;

namespace Platformer;

public class EntityFactory
{
    private readonly ContentManager _contentManager;

    private readonly World _world;

    public EntityFactory(World world, ContentManager contentManager)
    {
        _world          = world;
        _contentManager = contentManager;
    }

    public Entity CreatePlayer(Vector2 position)
    {
        Texture2D    dudeTexture = _contentManager.Load<Texture2D>(assetName: "hero");
        TextureAtlas dudeAtlas   = TextureAtlas.Create(name: "dudeAtlas", dudeTexture, regionWidth: 16, regionHeight: 16);

        Entity      entity      = _world.CreateEntity();
        SpriteSheet spriteSheet = new() { TextureAtlas = dudeAtlas };

        AddAnimationCycle(spriteSheet, name: "idle", frames: new[] { 0, 1, 2, 1 });
        AddAnimationCycle(spriteSheet, name: "walk", frames: new[] { 6, 7, 8, 9, 10, 11 });
        AddAnimationCycle(spriteSheet, name: "jump", frames: new[] { 10, 12 }, isLooping: false);
        AddAnimationCycle(spriteSheet, name: "fall", frames: new[] { 13, 14 }, isLooping: false);
        AddAnimationCycle(spriteSheet, name: "swim", frames: new[] { 18, 19, 20, 21, 22, 23 });
        AddAnimationCycle(spriteSheet, name: "kick", frames: new[] { 15 }, isLooping: false, frameDuration: 0.3f);
        AddAnimationCycle(spriteSheet, name: "punch", frames: new[] { 26 }, isLooping: false, frameDuration: 0.3f);
        AddAnimationCycle(spriteSheet, name: "cool", frames: new[] { 17 }, isLooping: false, frameDuration: 0.3f);
        entity.Attach(component: new AnimatedSprite(spriteSheet, playAnimation: "idle"));
        entity.Attach(component: new Transform2(position, rotation: 0, scale: Vector2.One * 4));
        entity.Attach(component: new Body { Position = position, Size = new Vector2(x: 32, y: 64), BodyType = BodyType.Dynamic });
        entity.Attach(component: new Player());

        return entity;
    }

    public Entity CreateBlue(Vector2 position)
    {
        Texture2D    dudeTexture = _contentManager.Load<Texture2D>(assetName: "blueguy");
        TextureAtlas dudeAtlas   = TextureAtlas.Create(name: "blueguyAtlas", dudeTexture, regionWidth: 16, regionHeight: 16);

        Entity      entity      = _world.CreateEntity();
        SpriteSheet spriteSheet = new() { TextureAtlas = dudeAtlas };
        AddAnimationCycle(spriteSheet, name: "idle", frames: new[] { 0, 1, 2, 3, 2, 1 });
        AddAnimationCycle(spriteSheet, name: "walk", frames: new[] { 6, 7, 8, 9, 10, 11 });
        AddAnimationCycle(spriteSheet, name: "jump", frames: new[] { 10, 12 }, isLooping: false, frameDuration: 1.0f);
        entity.Attach(component: new AnimatedSprite(spriteSheet, playAnimation: "idle") { Effect = SpriteEffects.FlipHorizontally });
        entity.Attach(component: new Transform2(position, rotation: 0, scale: Vector2.One * 4));
        entity.Attach(component: new Body { Position = position, Size = new Vector2(x: 32, y: 64), BodyType = BodyType.Dynamic });
        entity.Attach(component: new Enemy());

        return entity;
    }

    private void AddAnimationCycle(SpriteSheet spriteSheet, string name, int[] frames, bool isLooping = true, float frameDuration = 0.1f)
    {
        SpriteSheetAnimationCycle cycle = new();

        foreach (int f in frames)
        {
            cycle.Frames.Add(item: new SpriteSheetAnimationFrame(f, frameDuration));
        }

        cycle.IsLooping = isLooping;

        spriteSheet.Cycles.Add(name, cycle);
    }

    public void CreateTile(int x, int y, int width, int height)
    {
        Entity entity = _world.CreateEntity();

        entity.Attach(component: new Body
        {
            Position = new Vector2(x: x * width + width * 0.5f, y: y * height + height * 0.5f),
            Size     = new Vector2(width, height),
            BodyType = BodyType.Static
        });
    }
}
