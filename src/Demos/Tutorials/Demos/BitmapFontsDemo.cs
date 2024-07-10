using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.ViewportAdapters;

namespace Tutorials.Demos;

public class BitmapFontsDemo : DemoBase
{
    private Texture2D _backgroundTexture;

    private BitmapFont _bitmapFontImpact;

    private BitmapFont _bitmapFontMonospaced;

    private BitmapFont _bitmapFontMontserrat;

    private Rectangle _clippingRectangle = new(x: 100, y: 100, width: 300, height: 300);

    private MouseState _previousMouseState;

    private SpriteBatch _spriteBatch;

    private BoxingViewportAdapter _viewportAdapter;

    public BitmapFontsDemo(GameMain parent)
        : base(parent)
    { }

    public override string Name => "Bitmap Fonts";

    protected override void LoadContent()
    {
        _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, virtualWidth: 800, virtualHeight: 480);

        _backgroundTexture    = Content.Load<Texture2D>(assetName: "Textures/vignette");
        _bitmapFontImpact     = Content.Load<BitmapFont>(assetName: "Fonts/impact-32");
        _bitmapFontMontserrat = Content.Load<BitmapFont>(assetName: "Fonts/montserrat-32");
        _bitmapFontMonospaced = LoadCustomMonospacedFont();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    private BitmapFont LoadCustomMonospacedFont()
    {
        // this is a way to create a font in pure code without a font file.
        const string characters        = @" !""#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~";
        Texture2D    monospacedTexture = Content.Load<Texture2D>(assetName: "Fonts/monospaced");
        TextureAtlas atlas             = TextureAtlas.Create(name: "monospaced-atlas", monospacedTexture, regionWidth: 16, regionHeight: 16);
        var          fontRegions       = new BitmapFontRegion[characters.Length];
        int          index             = 0;

        for (int y = 0; y < monospacedTexture.Height; y += 16)
        {
            for (int x = 0; x < monospacedTexture.Width; x += 16)
            {
                if (index < characters.Length)
                {
                    fontRegions[index] = new BitmapFontRegion(textureRegion: atlas[index], character: characters[index], xOffset: 0, yOffset: 0, xAdvance: 16);
                    index++;
                }
            }
        }

        return new BitmapFont(name: "monospaced", fontRegions, lineHeight: 16);
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState    mouseState    = Mouse.GetState();

        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        int dx = mouseState.X - _previousMouseState.X;
        int dy = mouseState.Y - _previousMouseState.Y;

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _clippingRectangle.X += dx;
            _clippingRectangle.Y += dy;
        }

        if (mouseState.RightButton == ButtonState.Pressed)
        {
            _clippingRectangle.Width  += dx;
            _clippingRectangle.Height += dy;
        }

        _previousMouseState = mouseState;
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(samplerState: SamplerState.LinearClamp, blendState: BlendState.AlphaBlend, transformMatrix: _viewportAdapter.GetScaleMatrix());
        _spriteBatch.Draw(_backgroundTexture, _viewportAdapter.BoundingRectangle, Color.DarkBlue);

        const string helloWorld = "The quick brown fox jumps over the lazy dog\nThe lazy dog jumps back over the quick brown fox";

        Point2  position = new(x: 400, y: 140);
        Vector2 offset   = new(x: 0, y: 50);
        Vector2 scale    = Vector2.One;
        Color   color    = Color.White;
        int     rotation = 0; //MathHelper.Pi/64f;

        // bitmap font
        Size2  bitmapFontSize   = _bitmapFontImpact.MeasureString(helloWorld);
        Point2 bitmapFontOrigin = bitmapFontSize / 2f;

        _spriteBatch.DrawString(_bitmapFontImpact,
                                helloWorld,
                                position: position + offset,
                                color,
                                rotation,
                                bitmapFontOrigin,
                                scale,
                                SpriteEffects.None,
                                layerDepth: 0);

        _spriteBatch.DrawRectangle(location: position - bitmapFontOrigin + offset, bitmapFontSize, Color.Red);

        Size2 bitmapFontMontserratSize   = _bitmapFontMontserrat.MeasureString(helloWorld);
        Size2 bitmapFontMontserratOrigin = bitmapFontMontserratSize / 2f;

        _spriteBatch.DrawString(_bitmapFontMontserrat,
                                helloWorld,
                                position: position + offset * 3,
                                color,
                                rotation,
                                bitmapFontMontserratOrigin,
                                scale,
                                SpriteEffects.None,
                                layerDepth: 0,
                                _clippingRectangle);

        _spriteBatch.DrawRectangle(_clippingRectangle, Color.White);
        _spriteBatch.DrawRectangle(location: position - bitmapFontMontserratOrigin + offset * 3, bitmapFontMontserratSize, Color.Green);

        _spriteBatch.DrawString(_bitmapFontMonospaced, text: "Hello Monospaced Fonts!", position: new Vector2(x: 100, y: 400), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
