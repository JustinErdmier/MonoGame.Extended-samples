using System;

namespace Platformer;

public static class Program
{
    [ STAThread ]
    private static void Main()
    {
        using (GameMain game = new())
            game.Run();
    }
}
