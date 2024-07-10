using Autofac;

using Microsoft.Xna.Framework;

namespace Platformer;

public abstract class GameBase : Game
{
    protected GameBase(int width = 800, int height = 480)
    {
        Width                    = width;
        Height                   = height;
        GraphicsDeviceManager    = new GraphicsDeviceManager(game: this) { PreferredBackBufferWidth = width, PreferredBackBufferHeight = height };
        IsMouseVisible           = true;
        Window.AllowUserResizing = true;
        Content.RootDirectory    = "Content";
    }

    // ReSharper disable once NotAccessedField.Local
    protected GraphicsDeviceManager GraphicsDeviceManager { get; }

    protected IContainer Container { get; private set; }

    public int Width { get; }

    public int Height { get; }

    protected override void Initialize()
    {
        ContainerBuilder containerBuilder = new();

        RegisterDependencies(containerBuilder);
        Container = containerBuilder.Build();

        base.Initialize();
    }

    protected abstract void RegisterDependencies(ContainerBuilder builder);
}
