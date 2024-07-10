using System;

namespace Tutorials;

public static class Program
{
    [ STAThread ]
    public static void Main()
    {
        using (GameMain game = new(config: new PlatformConfig { IsFullScreen = false }))
            game.Run();
    }
}
