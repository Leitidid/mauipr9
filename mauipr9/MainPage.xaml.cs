using System.Text.Json;
using System.IO;

namespace mauipr9;

public partial class MainPage : ContentPage
{
    private Student? _student;

    public MainPage()
    {
        InitializeComponent();
        try
        {
            LoadStudentData();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка в MainPage: {ex}");
            // Или покажите alert:
             MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Ошибка", ex.Message, "OK"));
        }
    }

    private async void LoadStudentData()
    {
        string cacheDir = FileSystem.Current.CacheDirectory;
        string filePath = Path.Combine(cacheDir, "students.json");

        if (File.Exists(filePath))
        {
            string json = await File.ReadAllTextAsync(filePath);
            if (!string.IsNullOrWhiteSpace(json))
            {
                var students = JsonSerializer.Deserialize<List<Student>>(json);
                _student = students?.LastOrDefault(); // берём последнего добавленного
            }
        }

        if (_student != null)
        {
            FullNameLabel.Text = _student.FullName;
            GenderLabel.Text = $"Пол: {_student.Gender}";
            AgeLabel.Text = $"Возраст: {_student.Age}";
            GradesLabel.Text = $"Средний балл: {_student.Grades:F1}";
            DormLabel.Text = $"Общежитие: {(_student.NeedsDorm ? "Да" : "Нет")}";
            LeaderLabel.Text = $"Староста: {(_student.IsLeader ? "Да" : "Нет")}";

            if (!string.IsNullOrEmpty(_student.PhotoPath) && File.Exists(_student.PhotoPath))
            {
                ProfilePhoto.Source = ImageSource.FromFile(_student.PhotoPath);
            }
        }
        else
        {
            FullNameLabel.Text = "Данные не найдены";
        }
    }

    private async void OnEditClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddStudentPage());
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStudentData(); // обновляем при возврате
    }
}