using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Particles.Modifiers;
using MonoGame.Extended.Particles.Modifiers.Containers;
using MonoGame.Extended.Particles.Modifiers.Interpolators;
using MonoGame.Extended.Particles.Profiles;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos;

public class ParticlesDemo : DemoBase
{
    private OrthographicCamera _camera;

    private ParticleEffect _particleEffect;

    private Texture2D _particleTexture;

    private Sprite _sprite;

    private SpriteBatch _spriteBatch;

    private Transform2 _transform;

    public ParticlesDemo(GameMain game)
        : base(game)
    { }

    public override string Name => "Particles";

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        BoxingViewportAdapter viewportAdapter = new(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480);
        _camera = new OrthographicCamera(viewportAdapter);

        Texture2D logoTexture = Content.Load<Texture2D>(assetName: "Textures/logo-square-128");
        _sprite    = new Sprite(logoTexture);
        _transform = new Transform2 { Position = viewportAdapter.Center.ToVector2() };

        _particleTexture = new Texture2D(GraphicsDevice, width: 1, height: 1);
        _particleTexture.SetData(data: new[] { Color.White });

        ParticleInit(textureRegion: new TextureRegion2D(_particleTexture));
    }

    protected override void UnloadContent()
    {
        // any content not loaded with the content manager should be disposed
        _particleTexture.Dispose();
        _particleEffect.Dispose();
    }

    protected override void Update(GameTime gameTime)
    {
        float         deltaTime     = (float)gameTime.ElapsedGameTime.TotalSeconds;
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState    mouseState    = Mouse.GetState();
        Vector2       p             = _camera.ScreenToWorld(mouseState.X, mouseState.Y);

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        _transform.Rotation += deltaTime;

        _particleEffect.Update(deltaTime);

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _particleEffect.Trigger(position: new Vector2(p.X, p.Y));
        }

        //_particleEffect.Position = new Vector2(400, 240);
        //_particleEffect.Trigger(new Vector2(400, 240));

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(blendState: BlendState.AlphaBlend, transformMatrix: _camera.GetViewMatrix());
        _spriteBatch.Draw(_particleEffect);
        _spriteBatch.Draw(_sprite, _transform.Position, _transform.Rotation, _transform.Scale);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void ParticleInit(TextureRegion2D textureRegion)
    {
        _particleEffect = new ParticleEffect
        {
            Position = new Vector2(x: 400, y: 240),
            Emitters = new List<ParticleEmitter>
            {
                new(textureRegion,
                    capacity: 500,
                    lifeSpan: TimeSpan.FromSeconds(value: 2.5),
                    profile: Profile.Ring(radius: 150f, Profile.CircleRadiation.In))
                {
                    Parameters =
                        new ParticleReleaseParameters
                        {
                            Speed    = new Range<float>(min: 0f, max: 50f),
                            Quantity = 3,
                            Rotation = new Range<float>(min: -1f, max: 1f),
                            Scale    = new Range<float>(min: 3.0f, max: 4.0f)
                        },
                    Modifiers =
                    {
                        new AgeModifier
                        {
                            Interpolators =
                            {
                                new ColorInterpolator
                                {
                                    StartValue = new HslColor(h: 0.33f, s: 0.5f, l: 0.5f),
                                    EndValue   = new HslColor(h: 0.5f, s: 0.9f, l: 1.0f)
                                }
                            }
                        },
                        new RotationModifier { RotationRate    = -2.1f },
                        new RectangleContainerModifier { Width = 800, Height              = 480 },
                        new LinearGravityModifier { Direction  = -Vector2.UnitY, Strength = 30f }
                    }
                }
            }
        };
    }
}
