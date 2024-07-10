using System;
using System.Collections.Generic;
using System.Linq;

using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;

using Tutorials.Demos;

namespace Tutorials;

public class SelectDemoScreen : Screen
{
    private readonly IDictionary<string, DemoBase> _demos;

    private readonly Action<string> _loadDemo;

    public SelectDemoScreen(IDictionary<string, DemoBase> demos, Action<string> loadDemo, Action exitGameAction)
    {
        _demos    = demos;
        _loadDemo = loadDemo;

        UniformGrid grid = new();

        foreach (DemoBase demo in _demos.Values.OrderBy(i => i.Name))
        {
            Button button = new() { Content = demo.Name, Margin = new Thickness(all: 4) };
            button.Clicked += (sender, args) => LoadDemo(demo);
            grid.Items.Add(button);
        }

        Button closeButton = new() { Margin = 4, Content = "Close" };
        closeButton.Clicked += (sender, args) => exitGameAction();
        grid.Items.Add(closeButton);

        Content = grid;
    }

    public override void Dispose()
    {
        foreach (DemoBase demo in _demos.Values)
        {
            demo.Dispose();
        }

        base.Dispose();
    }

    private void LoadDemo(DemoBase demo)
    {
        _loadDemo(demo.Name);
        Hide();
    }
}
