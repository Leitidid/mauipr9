using Microsoft.Maui.Storage;
using System.Text.Json;
using System.IO;

namespace mauipr9;

public partial class AddStudentPage : ContentPage
{
    private string _photoPath = string.Empty;

    public AddStudentPage()
    {
        InitializeComponent();
    }

    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Выберите фото"
            });

            if (result != null)
            {
                _photoPath = result.FullPath;
                UserPhoto.Source = ImageSource.FromFile(_photoPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось выбрать фото: {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FullNameEntry.Text))
        {
            await DisplayAlert("Ошибка", "Введите ФИО", "OK");
            return;
        }

        if (GenderPicker.SelectedItem == null)
        {
            await DisplayAlert("Ошибка", "Выберите пол", "OK");
            return;
        }

        var student = new Student
        {
            FullName = FullNameEntry.Text.Trim(),
            Gender = GenderPicker.SelectedItem.ToString(),
            Age = (int)AgeStepper.Value,
            PhotoPath = _photoPath,
            NeedsDorm = DormSwitch.IsToggled,
            IsLeader = LeaderSwitch.IsToggled,
            Grades = Math.Round(GradesSlider.Value, 1)
        };

        // Сохраняем в файл
        await SaveStudentToFile(student);

        // Очистка формы
        ClearForm();
        await DisplayAlert("Успех", "Студент сохранён!", "OK");

        // Возвращаемся на главную страницу
        await Navigation.PopToRootAsync(); // или Navigation.PopAsync();
    }

    private void ClearForm()
    {
        FullNameEntry.Text = "";
        GenderPicker.SelectedItem = null;
        AgeStepper.Value = 18;
        GradesSlider.Value = 4.0;
        DormSwitch.IsToggled = false;
        LeaderSwitch.IsToggled = false;
        _photoPath = "";
        UserPhoto.Source = "default_avatar.png";
    }

    private async Task SaveStudentToFile(Student student)
    {
        string cacheDir = FileSystem.Current.CacheDirectory;
        string filePath = Path.Combine(cacheDir, "students.json");

        List<Student> students = [];

        if (File.Exists(filePath))
        {
            string json = await File.ReadAllTextAsync(filePath);
            if (!string.IsNullOrWhiteSpace(json))
            {
                students = JsonSerializer.Deserialize<List<Student>>(json) ?? [];
            }
        }

        students.Add(student);

        string newJson = JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, newJson);
    }
}