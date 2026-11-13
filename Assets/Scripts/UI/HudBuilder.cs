using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Whiteout.Core.Unity;

namespace Whiteout.UI
{
    /// <summary>Small helpers for HUD parts that are built in code (rectangles, images, labels).</summary>
    internal static class HudBuilder
    {
        /// <summary>Creates an empty rectangle under a parent.</summary>
        internal static RectTransform NewRect(string name, Transform parent)
        {
            var rect = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            return rect;
        }

        /// <summary>Creates an image under a parent; it never blocks the mouse.</summary>
        internal static Image NewImage(string name, Transform parent, Sprite sprite, Color color)
        {
            var rect = NewRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        /// <summary>Creates a label under a parent with the shared font; it never blocks the mouse.</summary>
        internal static TMP_Text NewLabel(string name, Transform parent, UiTextTable texts, float fontSize, Color color, TextAlignmentOptions alignment)
        {
            var rect = NewRect(name, parent);
            var label = rect.gameObject.AddComponent<TextMeshProUGUI>();
            texts.ApplyFont(label);
            label.fontSize = fontSize;
            label.color = color;
            label.alignment = alignment;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.raycastTarget = false;
            return label;
        }

        /// <summary>Anchors a rectangle to the top center of its parent with the given pivot, position and size.</summary>
        internal static void PlaceTopCenter(RectTransform rect, Vector2 pivot, Vector2 position, Vector2 size)
        {
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        /// <summary>Makes a rectangle fill its parent.</summary>
        internal static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>Destroys a sprite and its texture.</summary>
        internal static void DestroySprite(Sprite sprite)
        {
            if (sprite == null)
            {
                return;
            }

            var texture = sprite.texture;
            Object.Destroy(sprite);
            Object.Destroy(texture);
        }
    }
}
