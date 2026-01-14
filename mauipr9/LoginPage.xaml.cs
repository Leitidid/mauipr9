using Microsoft.Maui.Storage;

using System.Security.Cryptography;
using System.Text;

namespace mauipr9;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnPasswordLoginClicked(object sender, EventArgs e)
    {
        if (PasswordEntry.Text == "123")
        {
            await NavigateToMain();
        }
        else
        {
            await DisplayAlert("Ошибка", "Неверный пароль!", "OK");
        }
    }

    private async void OnSetPatternClicked(object sender, EventArgs e)
    {
        var page = new PatternLockPage();
        await Navigation.PushAsync(page);

        // Ждём, пока страница закроется
        await WaitForPageClose(page);

        if (!string.IsNullOrEmpty(page.ResultPattern))
        {
            string hash = HashPattern(page.ResultPattern);
            Preferences.Set("PatternHash", hash);
            await DisplayAlert("Успех", "Графический ключ сохранён!", "OK");
        }
    }

    private async void OnPatternLoginClicked(object sender, EventArgs e)
    {
        var page = new PatternLockPage();
        await Navigation.PushAsync(page);

        await WaitForPageClose(page);

        if (!string.IsNullOrEmpty(page.ResultPattern))
        {
            string inputHash = HashPattern(page.ResultPattern);
            string savedHash = Preferences.Get("PatternHash", "");

            if (!string.IsNullOrEmpty(savedHash) && inputHash == savedHash)
            {
                await NavigateToMain(); // ? используем общий метод
                return;
            }
        }

        await DisplayAlert("Ошибка", "Неверный графический ключ", "OK");
    }

    private async Task WaitForPageClose(ContentPage page)
    {
        while (Navigation.NavigationStack.Contains(page))
        {
            await Task.Delay(100);
        }
    }

    private async Task NavigateToMain()
    {
        await Navigation.PushAsync(new StudentListPage());
    }

    private string HashPattern(string pattern)
    {
        using var sha = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(pattern);
        byte[] hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
   
}