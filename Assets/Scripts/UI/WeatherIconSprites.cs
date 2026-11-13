using System;
using UnityEngine;

namespace Whiteout.UI
{
    /// <summary>
    /// Draws the weather icons as white anti-aliased shapes into small textures, so the HUD needs no image assets
    /// and no emoji font. The caller tints the sprite and owns (destroys) the returned texture and sprite.
    /// </summary>
    /// <example>
    /// <code>
    /// Sprite sun = WeatherIconSprites.Create(WeatherIconShape.Sun);
    /// image.sprite = sun;
    /// </code>
    /// </example>
    public static class WeatherIconSprites
    {
        private const int Size = 32;
        private const float Center = 15.5f;

        /// <summary>Creates the sprite of a shape.</summary>
        /// <param name="shape">Which icon.</param>
        public static Sprite Create(WeatherIconShape shape)
        {
            var texture = new Texture2D(Size, Size, TextureFormat.RGBA32, false)
            {
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear,
                hideFlags = HideFlags.HideAndDontSave
            };

            var pixels = new Color32[Size * Size];
            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    var coverage = Mathf.Clamp01(0.5f - Distance(shape, x + 0.5f, y + 0.5f));
                    pixels[y * Size + x] = new Color32(255, 255, 255, (byte)Mathf.RoundToInt(coverage * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, Size, Size), new Vector2(0.5f, 0.5f), Size, 0, SpriteMeshType.FullRect);
        }

        private static float Distance(WeatherIconShape shape, float x, float y)
        {
            switch (shape)
            {
                case WeatherIconShape.Sun:
                {
                    var d = Circle(x, y, Center, Center, 7f);
                    for (var i = 0; i < 8; i++)
                    {
                        var angle = i * Mathf.PI / 4f;
                        var cos = Mathf.Cos(angle);
                        var sin = Mathf.Sin(angle);
                        d = Mathf.Min(d, Segment(x, y, Center + cos * 10f, Center + sin * 10f, Center + cos * 14f, Center + sin * 14f, 1.3f));
                    }

                    return d;
                }

                case WeatherIconShape.Snowflake:
                {
                    var d = float.MaxValue;
                    for (var i = 0; i < 3; i++)
                    {
                        var angle = i * Mathf.PI / 3f;
                        var cos = Mathf.Cos(angle) * 13f;
                        var sin = Mathf.Sin(angle) * 13f;
                        d = Mathf.Min(d, Segment(x, y, Center - cos, Center - sin, Center + cos, Center + sin, 1.3f));
                    }

                    return d;
                }

                case WeatherIconShape.Wind:
                    return Mathf.Min(
                        Segment(x, y, 4f, 10f, 24f, 10f, 1.6f),
                        Mathf.Min(Segment(x, y, 4f, 16f, 28f, 16f, 1.6f), Segment(x, y, 4f, 22f, 20f, 22f, 1.6f)));

                case WeatherIconShape.Thermometer:
                    return Mathf.Min(Segment(x, y, 16f, 6f, 16f, 20f, 2.6f), Circle(x, y, 16f, 23f, 5f));

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }

        private static float Circle(float x, float y, float cx, float cy, float radius)
        {
            return Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) - radius;
        }

        private static float Segment(float x, float y, float ax, float ay, float bx, float by, float halfWidth)
        {
            var abx = bx - ax;
            var aby = by - ay;
            var t = Mathf.Clamp01(((x - ax) * abx + (y - ay) * aby) / (abx * abx + aby * aby));
            var dx = x - (ax + abx * t);
            var dy = y - (ay + aby * t);
            return Mathf.Sqrt(dx * dx + dy * dy) - halfWidth;
        }
    }
}
