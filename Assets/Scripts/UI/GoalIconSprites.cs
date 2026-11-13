using System;
using UnityEngine;

namespace Whiteout.UI
{
    /// <summary>
    /// Draws the goal icons (beacon, its three parts, rescue, arrow, check mark) as white anti-aliased shapes into small textures, like
    /// <see cref="WeatherIconSprites"/>, so the HUD needs no image assets. The caller tints the sprite and owns
    /// (destroys) the returned texture and sprite.
    /// </summary>
    /// <example>
    /// <code>
    /// image.sprite = GoalIconSprites.Create(GoalIconShape.Antenna);
    /// </code>
    /// </example>
    public static class GoalIconSprites
    {
        private const int Size = 64;

        /// <summary>Creates the sprite of a shape.</summary>
        /// <param name="shape">Which icon.</param>
        public static Sprite Create(GoalIconShape shape)
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

        private static float Distance(GoalIconShape shape, float x, float y)
        {
            switch (shape)
            {
                case GoalIconShape.Antenna:
                    return Mathf.Min(
                        Mathf.Min(Segment(x, y, 32f, 8f, 32f, 46f, 2.6f), Circle(x, y, 32f, 52f, 5f)),
                        Mathf.Min(Segment(x, y, 32f, 38f, 18f, 50f, 2.4f), Segment(x, y, 32f, 38f, 46f, 50f, 2.4f)));

                case GoalIconShape.Battery:
                {
                    var outline = Mathf.Abs(Box(x, y, 32f, 28f, 15f, 21f)) - 2.2f;
                    var nub = Box(x, y, 32f, 54f, 6f, 4f);
                    var bolt = Mathf.Min(
                        Segment(x, y, 36f, 40f, 27f, 29f, 2f),
                        Mathf.Min(Segment(x, y, 27f, 29f, 37f, 29f, 2f), Segment(x, y, 37f, 29f, 28f, 16f, 2f)));
                    return Mathf.Min(outline, Mathf.Min(nub, bolt));
                }

                case GoalIconShape.Transmitter:
                {
                    var body = Mathf.Abs(Box(x, y, 28f, 20f, 21f, 12f)) - 2.2f;
                    var stub = Segment(x, y, 42f, 32f, 42f, 46f, 2.4f);
                    var knob = Circle(x, y, 17f, 20f, 4f);
                    var wave1 = Mathf.Max(Mathf.Abs(Circle(x, y, 42f, 46f, 8f)) - 1.7f, 44f - y);
                    var wave2 = Mathf.Max(Mathf.Abs(Circle(x, y, 42f, 46f, 15f)) - 1.7f, 44f - y);
                    return Mathf.Min(Mathf.Min(body, stub), Mathf.Min(knob, Mathf.Min(wave1, wave2)));
                }

                case GoalIconShape.Beacon:
                {
                    var legs = Mathf.Min(Segment(x, y, 17f, 6f, 30f, 40f, 2.4f), Segment(x, y, 47f, 6f, 34f, 40f, 2.4f));
                    var bars = Mathf.Min(Segment(x, y, 22f, 20f, 42f, 20f, 2f), Segment(x, y, 26f, 31f, 38f, 31f, 2f));
                    var top = Circle(x, y, 32f, 45f, 4.5f);
                    var wave1 = Mathf.Max(Mathf.Abs(Circle(x, y, 32f, 45f, 10f)) - 1.6f, 44f - y);
                    var wave2 = Mathf.Max(Mathf.Abs(Circle(x, y, 32f, 45f, 17f)) - 1.6f, 44f - y);
                    return Mathf.Min(Mathf.Min(legs, bars), Mathf.Min(top, Mathf.Min(wave1, wave2)));
                }

                case GoalIconShape.Rescue:
                {
                    var body = Circle(x, y, 28f, 28f, 12f);
                    var tail = Segment(x, y, 36f, 31f, 56f, 36f, 2.6f);
                    var mast = Segment(x, y, 28f, 38f, 28f, 48f, 2f);
                    var rotor = Segment(x, y, 8f, 48f, 50f, 48f, 2.2f);
                    var skid = Segment(x, y, 14f, 11f, 42f, 11f, 2.2f);
                    var legs = Mathf.Min(Segment(x, y, 20f, 11f, 22f, 18f, 2f), Segment(x, y, 36f, 11f, 34f, 18f, 2f));
                    return Mathf.Min(Mathf.Min(body, tail), Mathf.Min(Mathf.Min(mast, rotor), Mathf.Min(skid, legs)));
                }

                case GoalIconShape.Arrow:
                    return Mathf.Min(
                        Segment(x, y, 8f, 32f, 54f, 32f, 3.2f),
                        Mathf.Min(Segment(x, y, 38f, 16f, 55f, 32f, 3.2f), Segment(x, y, 38f, 48f, 55f, 32f, 3.2f)));

                case GoalIconShape.Check:
                    return Mathf.Min(Segment(x, y, 12f, 32f, 26f, 16f, 5f), Segment(x, y, 26f, 16f, 52f, 48f, 5f));

                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }

        private static float Circle(float x, float y, float cx, float cy, float radius)
        {
            return Mathf.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) - radius;
        }

        private static float Box(float x, float y, float cx, float cy, float halfWidth, float halfHeight)
        {
            var dx = Mathf.Abs(x - cx) - halfWidth;
            var dy = Mathf.Abs(y - cy) - halfHeight;
            var outside = Mathf.Sqrt(Mathf.Max(dx, 0f) * Mathf.Max(dx, 0f) + Mathf.Max(dy, 0f) * Mathf.Max(dy, 0f));
            return outside + Mathf.Min(Mathf.Max(dx, dy), 0f);
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
