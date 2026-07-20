using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    /// <summary>
    /// A UI Toolkit VisualElement that draws a pointy-top hexagon using Painter2D.
    /// Hit-testing (ContainsPoint) matches the hexagon shape, not the rectangle.
    /// </summary>
    public sealed class HexCellElement : VisualElement
    {
        private readonly Vector2[] _vertices = new Vector2[6];

        private Color _fillColor = new Color(0.08f, 0.12f, 0.2f);
        private Color _borderColor = new Color(0.3f, 0.5f, 0.8f);
        private float _borderWidth = 2f;

        public Color FillColor
        {
            get => _fillColor;
            set { _fillColor = value; MarkDirtyRepaint(); }
        }

        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; MarkDirtyRepaint(); }
        }

        public float BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = Mathf.Max(0f, value); MarkDirtyRepaint(); }
        }

        public HexCellElement()
        {
            generateVisualContent += DrawHexagon;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.Center;
        }

        private void DrawHexagon(MeshGenerationContext context)
        {
            UpdateVertices();

            Painter2D painter = context.painter2D;

            painter.BeginPath();
            painter.MoveTo(_vertices[0]);
            for (int i = 1; i < _vertices.Length; i++)
                painter.LineTo(_vertices[i]);
            painter.ClosePath();

            painter.fillColor = _fillColor;
            painter.Fill();

            if (_borderWidth > 0f)
            {
                painter.strokeColor = _borderColor;
                painter.lineWidth = _borderWidth;
                painter.Stroke();
            }
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            UpdateVertices();

            bool inside = false;
            for (int i = 0, j = _vertices.Length - 1; i < _vertices.Length; j = i++)
            {
                Vector2 a = _vertices[i];
                Vector2 b = _vertices[j];
                bool intersects = ((a.y > localPoint.y) != (b.y > localPoint.y)) &&
                    localPoint.x < (b.x - a.x) * (localPoint.y - a.y) / (b.y - a.y) + a.x;
                if (intersects) inside = !inside;
            }
            return inside;
        }

        private void UpdateVertices()
        {
            Rect rect = contentRect;
            Vector2 center = rect.center;
            float availableWidth = Mathf.Max(0f, rect.width - _borderWidth * 2f);
            float availableHeight = Mathf.Max(0f, rect.height - _borderWidth * 2f);
            float radius = Mathf.Min(availableHeight * 0.5f, availableWidth / Mathf.Sqrt(3f));

            for (int i = 0; i < 6; i++)
            {
                float angle = Mathf.Deg2Rad * (-90f + i * 60f);
                _vertices[i] = center + new Vector2(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius);
            }
        }
    }
}
