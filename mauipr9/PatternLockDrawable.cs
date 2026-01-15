// PatternLockDrawable.cs
using Microsoft.Maui.Graphics;

namespace mauipr9;

public class PatternLockDrawable : IDrawable
{
    public List<Point> Dots { get; set; } = new();
    public List<int> SelectedIndices { get; set; } = new();
    public Point? CurrentTouch { get; set; }
    public Color DotColor { get; set; } = Colors.LightGray;
    public Color SelectedDotColor { get; set; } = Colors.DodgerBlue;
    public Color LineColor { get; set; } = Colors.DodgerBlue;
    public float DotRadius { get; set; } = 20f;
    public float LineWidth { get; set; } = 6f;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // Рисуем линии между выбранными точками
        if (SelectedIndices.Count > 0)
        {
            var points = SelectedIndices.Select(i => Dots[i]).ToList();
            if (CurrentTouch.HasValue)
                points.Add(CurrentTouch.Value);

            for (int i = 0; i < points.Count - 1; i++)
            {
                canvas.StrokeColor = LineColor;
                canvas.StrokeSize = LineWidth;
                canvas.DrawLine(
                    (float)points[i].X, (float)points[i].Y,
                    (float)points[i + 1].X, (float)points[i + 1].Y);
            }
        }

        // Рисуем точки
        for (int i = 0; i < Dots.Count; i++)
        {
            var dot = Dots[i];
            canvas.FillColor = SelectedIndices.Contains(i) ? SelectedDotColor : DotColor;
            canvas.FillCircle((float)dot.X, (float)dot.Y, DotRadius);
        }
    }
}