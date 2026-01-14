using Microsoft.Maui.Controls.Shapes;
using System.Collections.Generic;

namespace mauipr9;

public partial class PatternLockPage : ContentPage
{
    private readonly Ellipse[] _dots;
    private readonly bool[] _selected;
    private readonly List<int> _pattern;
    private bool _isDrawing = false;
    public bool WasConfirmed { get; private set; } = false;
    public string ResultPattern { get; private set; } = "";
    // Добавьте это в начало класса PatternLockPage
 

    
    public PatternLockPage()
    {
        // Инициализируем массивы ДО InitializeComponent
        _dots = new Ellipse[9];
        _selected = new bool[9];
        _pattern = new List<int>();

        InitializeComponent();
        CreateDots();
    }
    private void OnConfirmClicked(object sender, EventArgs e)
    {
        ResultPattern = string.Join(",", _pattern);
        WasConfirmed = true;
        Navigation.PopAsync(); // просто закрываем страницу
    }
    private void CreateDots()
    {
        PatternGrid.RowDefinitions.Clear();
        PatternGrid.ColumnDefinitions.Clear();

        // 1. Сначала создаём строки и столбцы
        for (int i = 0; i < 3; i++)
        {
            PatternGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            PatternGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        // 2. Затем создаём и размещаем точки
        for (int i = 0; i < 9; i++)
        {
            int row = i / 3;
            int col = i % 3;

            var dot = new Ellipse
            {
                Fill = Colors.LightGray,
                WidthRequest = 60,
                HeightRequest = 60,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            int index = i;
            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => OnDotTapped(index);
            dot.GestureRecognizers.Add(tap);

            // ?? ОБЯЗАТЕЛЬНО: сначала SetRow/SetColumn, потом Add!
            Grid.SetRow(dot, row);
            Grid.SetColumn(dot, col);
            PatternGrid.Children.Add(dot);

            _dots[i] = dot;
        }
    }

    private void OnDotTapped(int index)
    {
        // Защита от некорректного индекса (на всякий случай)
        if (index < 0 || index >= 9) return;

        if (_isDrawing || _selected[index]) return;

        _selected[index] = true;
        _pattern.Add(index);
        _dots[index].Fill = Colors.DodgerBlue;

        UpdateStatus();

        if (_pattern.Count >= 4)
        {
            ConfirmButton.IsEnabled = true;
        }
    }

    private void UpdateStatus()
    {
        StatusLabel.Text = $"Выбрано: {_pattern.Count} точек (минимум 4)";
    }

    

    // Опционально: метод для сброса (если понадобится)
    public void Clear()
    {
        for (int i = 0; i < 9; i++)
        {
            _selected[i] = false;
            _dots[i].Fill = Colors.LightGray;
        }
        _pattern.Clear();
        ConfirmButton.IsEnabled = false;
        UpdateStatus();
    }
}