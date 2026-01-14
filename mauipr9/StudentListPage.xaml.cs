using System.Text.Json;
using System.IO;

namespace mauipr9;

public partial class StudentListPage : ContentPage
{
    private List<Student> _students = new();

    public StudentListPage()
    {
        InitializeComponent();
        LoadStudents();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStudents(); // Обновляем при возврате
    }

    private async void LoadStudents()
    {
        StudentsStack.Children.Clear();
        string path = Path.Combine(FileSystem.Current.CacheDirectory, "students.json");

        if (File.Exists(path))
        {
            try
            {
                string json = await File.ReadAllTextAsync(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    _students = JsonSerializer.Deserialize<List<Student>>(json) ?? new();
                }
            }
            catch { /* игнор */ }
        }

        if (_students.Count == 0)
        {
            EmptyLabel.IsVisible = true;
            return;
        }

        EmptyLabel.IsVisible = false;
        foreach (var student in _students)
        {
            var frame = new Frame { Padding = 12, Margin = new Thickness(0, 0, 0, 10) };
            var layout = new StackLayout();

            layout.Children.Add(new Label { Text = student.FullName, FontAttributes = FontAttributes.Bold });
            layout.Children.Add(new Label { Text = $"Балл: {student.Grades:F1}" });
            layout.Children.Add(new Label { Text = $"Возраст: {student.Age}" });

            var tap = new TapGestureRecognizer();
            tap.Tapped += (s, e) => OnStudentTapped(student);
            frame.GestureRecognizers.Add(tap);

            frame.Content = layout;
            StudentsStack.Children.Add(frame);
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditStudentPage(null)); // null = новый студент
    }

    private async void OnStudentTapped(Student student)
    {
        await Navigation.PushAsync(new EditStudentPage(student));
    }
}