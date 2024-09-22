using Microsoft.AspNetCore.Components;
using MySchool.Models;

namespace MySchool.App.Pages
{

    public partial class PageLogin
    {

        private StudentLogin login = new();
        private string errorMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await StudentService.InitializeAsync();
        }

        private async Task StudentLogin()
        {
            try
            {
                // Call the StudentService Login method.
                var token = await StudentService.Login(login);

                // Set the token in the AuthenticationStateProvider.
                await AuthenticationStateProvider.Login(token);

                // Navigate to a protected page after login.
                NavigationManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

    }

}