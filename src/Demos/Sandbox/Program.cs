using System;

namespace Sandbox;

public static class Program
{
    [ STAThread ]
    public static void Main()
    {
        using MainGame game = new();

        game.Run();
    }
}
