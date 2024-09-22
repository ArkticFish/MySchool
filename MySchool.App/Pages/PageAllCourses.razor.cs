using Microsoft.AspNetCore.Components;
using MySchool.App.Services;
using MySchool.Models;

namespace MySchool.App.Pages
{
    public partial class PageAllCourses
    {

        private List<Course> courses = new List<Course>();

        private Course selectedCourse;

        protected override async Task OnInitializedAsync()
        {
            await StudentService.InitializeAsync();
            courses = await StudentService.GetAllCourses();
        }

        private void SelectCourse(Course course)
        {
            selectedCourse = course;
        }

        private async Task RegisterCourse()
        {
            await StudentService.RegisterCourse(selectedCourse.CourseId);
            NavigationManager.NavigateTo("registeredCourses");
        }

    }
}