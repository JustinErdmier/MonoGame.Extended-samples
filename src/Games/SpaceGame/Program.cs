using System;

namespace SpaceGame;

internal static class Program
{
    [ STAThread ]
    private static void Main(string[] args)
    {
        using (Game1 game = new())
            game.Run();
    }
}
