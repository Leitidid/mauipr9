using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace mauipr9;

public partial class PatternLockView : ContentPage
{
    private readonly PatternLockDrawable _drawable = new();
    private readonly List<Point> _dots = new();
    private readonly List<int> _selected = new();
    private bool _isDrawing = false;

    public string ResultPattern => string.Join(",", _selected);

    public PatternLockView()
    {
        InitializeComponent();
        InitializeDots();
        _drawable.Dots = _dots;
        GraphicsView.Drawable = _drawable;

        // Добавляем жесты ПОСЛЕ загрузки страницы
        this.Loaded += (s, e) =>
        {
            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
            GraphicsView.GestureRecognizers.Add(pan);
        };
    }

    private void InitializeDots()
    {
        _dots.Clear();
        double size = 300;
        double spacing = size / 3;
        double offset = spacing / 2;

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                _dots.Add(new Point(offset + col * spacing, offset + row * spacing));
            }
        }
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _isDrawing = true;
                _selected.Clear();
                _drawable.SelectedIndices = _selected;
                break;

            case GestureStatus.Running:
                if (_isDrawing)
                {
                    var touch = new Point(
                        GraphicsView.Width / 2 + e.TotalX,
                        GraphicsView.Height / 2 + e.TotalY
                    );

                    int nearestIndex = GetNearestDot(touch);
                    if (nearestIndex >= 0 && !_selected.Contains(nearestIndex))
                    {
                        if (_selected.Count > 0)
                        {
                            int last = _selected.Last();
                            AddIntermediateDots(last, nearestIndex);
                        }
                        _selected.Add(nearestIndex);
                        _drawable.SelectedIndices = new List<int>(_selected);
                    }

                    _drawable.CurrentTouch = touch;
                    GraphicsView.Invalidate();
                }
                break;

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                FinishDrawing();
                break;
        }
    }

    private int GetNearestDot(Point touch)
    {
        const double threshold = 60;
        for (int i = 0; i < _dots.Count; i++)
        {
            // Ручной расчёт расстояния (Point.DistanceTo не существует!)
            double dx = touch.X - _dots[i].X;
            double dy = touch.Y - _dots[i].Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);

            if (distance < threshold)
                return i;
        }
        return -1;
    }

    private void AddIntermediateDots(int from, int to)
    {
        var intermediates = new Dictionary<(int, int), int>
        {
            {(0, 2), 1}, {(2, 0), 1},
            {(0, 6), 3}, {(6, 0), 3},
            {(2, 8), 5}, {(8, 2), 5},
            {(6, 8), 7}, {(8, 6), 7},
            {(0, 8), 4}, {(8, 0), 4},
            {(2, 6), 4}, {(6, 2), 4}
        };

        if (intermediates.TryGetValue((from, to), out int mid) && !_selected.Contains(mid))
        {
            _selected.Add(mid);
        }
    }

    private void FinishDrawing()
    {
        if (_isDrawing)
        {
            _isDrawing = false;
            _drawable.CurrentTouch = null;
            GraphicsView.Invalidate();
            ConfirmButton.IsEnabled = _selected.Count >= 4;
        }
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}