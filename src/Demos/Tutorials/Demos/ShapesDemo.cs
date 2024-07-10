using Microsoft.Xna.Framework;

using MonoGame.Extended.Shapes;
using MonoGame.Extended.VectorDraw;

namespace Tutorials.Demos;

public class ShapesDemo : DemoBase
{
    private readonly Polygon _polygon = new(vertices: new[]
    {
        new Vector2(x: 0, y: 0), new Vector2(x: 40, y: 0), new Vector2(x: 40, y: 40), new Vector2(x: 60, y: 40),
        new Vector2(x: 60, y: 60), new Vector2(x: 0, y: 60)
    });

    private Matrix _localProjection;

    private Matrix _localView;

    private PrimitiveBatch _primitiveBatch;

    private PrimitiveDrawing _primitiveDrawing;

    public ShapesDemo(GameMain game)
        : base(game)
    { }

    public override string Name => "Shapes";

    protected override void LoadContent()
    {
        _primitiveBatch   = new PrimitiveBatch(GraphicsDevice);
        _primitiveDrawing = new PrimitiveDrawing(_primitiveBatch);

        _localProjection = Matrix.CreateOrthographicOffCenter(left: 0f,
                                                              GraphicsDevice.Viewport.Width,
                                                              GraphicsDevice.Viewport.Height,
                                                              top: 0f,
                                                              zNearPlane: 0f,
                                                              zFarPlane: 1f);

        _localView = Matrix.Identity;
    }

    protected override void Draw(GameTime gameTime)
    {
        _localProjection = Matrix.CreateOrthographicOffCenter(left: 0f,
                                                              GraphicsDevice.Viewport.Width,
                                                              GraphicsDevice.Viewport.Height,
                                                              top: 0f,
                                                              zNearPlane: 0f,
                                                              zFarPlane: 1f);

        _localView = Matrix.Identity;

        _primitiveBatch.Begin(ref _localProjection, ref _localView);

        _primitiveDrawing.DrawPoint(center: new Vector2(x: 10, y: 10), Color.Brown);

        _primitiveDrawing.DrawRectangle(location: new Vector2(x: 20, y: 20), width: 50, height: 50, Color.Yellow);
        _primitiveDrawing.DrawSolidRectangle(location: new Vector2(x: 20, y: 120), width: 50, height: 50, Color.Yellow);

        _primitiveDrawing.DrawCircle(center: new Vector2(x: 120, y: 45), radius: 25, Color.Green);
        _primitiveDrawing.DrawSolidCircle(center: new Vector2(x: 120, y: 145), radius: 25, Color.Green);

        _primitiveDrawing.DrawEllipse(center: new Vector2(x: 220, y: 45), radius: new Vector2(x: 50, y: 25), sides: 32, Color.Orange);
        _primitiveDrawing.DrawSolidEllipse(center: new Vector2(x: 220, y: 145), radius: new Vector2(x: 50, y: 25), sides: 32, Color.Orange);

        _primitiveDrawing.DrawSegment(start: new Vector2(x: 320, y: 20), end: new Vector2(x: 370, y: 170), Color.White);

        _primitiveDrawing.DrawPolygon(position: new Vector2(x: 420, y: 20), _polygon.Vertices, Color.Aqua);
        _primitiveDrawing.DrawSolidPolygon(position: new Vector2(x: 420, y: 120), _polygon.Vertices, Color.Aqua);

        _primitiveBatch.End();
    }
}
