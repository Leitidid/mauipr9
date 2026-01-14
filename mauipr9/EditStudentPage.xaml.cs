using Microsoft.Maui.Storage;
using System.Text.Json;
using System.IO;

namespace mauipr9;

public partial class EditStudentPage : ContentPage
{
    private Student? _editingStudent;
    private string _photoPath = "";

    public EditStudentPage(Student? student)
    {
        InitializeComponent();
        _editingStudent = student;

        if (student != null)
        {
            // Заполняем форму данными
            FullNameEntry.Text = student.FullName;
            GenderPicker.SelectedItem = student.Gender;
            AgeStepper.Value = student.Age;
            GradesSlider.Value = student.Grades;
            DormSwitch.IsToggled = student.NeedsDorm;
            LeaderSwitch.IsToggled = student.IsLeader;
            _photoPath = student.PhotoPath;
            if (!string.IsNullOrEmpty(_photoPath) && File.Exists(_photoPath))
                UserPhoto.Source = ImageSource.FromFile(_photoPath);
        }
    }

    private async void OnPickPhotoClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions { FileTypes = FilePickerFileType.Images });
            if (result != null)
            {
                _photoPath = result.FullPath;
                UserPhoto.Source = ImageSource.FromFile(_photoPath);
            }
        }
        catch { }
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
            Gender = GenderPicker.SelectedItem.ToString()!,
            Age = (int)AgeStepper.Value,
            Grades = Math.Round(GradesSlider.Value, 1),
            NeedsDorm = DormSwitch.IsToggled,
            IsLeader = LeaderSwitch.IsToggled,
            PhotoPath = _photoPath
        };

        // Сохраняем в файл
        string path = Path.Combine(FileSystem.Current.CacheDirectory, "students.json");
        var students = new List<Student>();

        if (File.Exists(path))
        {
            try
            {
                string json = await File.ReadAllTextAsync(path);
                students = JsonSerializer.Deserialize<List<Student>>(json) ?? new();
            }
            catch { }
        }

        if (_editingStudent != null)
        {
            // Редактируем существующего
            var index = students.FindIndex(s => s.FullName == _editingStudent.FullName); // упрощённо
            if (index >= 0) students[index] = student;
        }
        else
        {
            // Добавляем нового
            students.Add(student);
        }

        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(students, new JsonSerializerOptions { WriteIndented = true }));

        await Navigation.PopAsync(); // Возврат к списку
    }
}