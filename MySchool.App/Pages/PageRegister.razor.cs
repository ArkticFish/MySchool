using Microsoft.AspNetCore.Components;
using MySchool.App.Services;
using MySchool.Models;

namespace MySchool.App.Pages
{

    public partial class PageRegister
    {

        private StudentRegister register = new StudentRegister();

        protected override async Task OnInitializedAsync()
        {
            await StudentService.InitializeAsync();
        }

        private async Task StudentRegister()
        {
            var result = await StudentService.Register(register);
            if (result)
            {
                NavigationManager.NavigateTo("login");
            }
        }

    }

}
