using System;

namespace Pong;

public static class Program
{
    [ STAThread ]
    private static void Main()
    {
        using (GameMain game = new())
            game.Run();
    }
}
