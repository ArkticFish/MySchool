using MySchool.App.Services;
using MySchool.Models;

namespace MySchool.App.Pages
{
    public partial class PageRegisteredCourses
    {

        private List<Course> courses = new List<Course>();

        private Course? selectedCourse;

        protected override async Task OnInitializedAsync()
        {
            await StudentService.InitializeAsync();
            courses = await StudentService.GetRegisteredCourses();
        }

        private void SelectCourse(Course course)
        {
            selectedCourse = course;
        }

        private async Task DeregisterCourse()
        {
            await StudentService.DeregisterCourse(selectedCourse.CourseId);
            courses = await StudentService.GetRegisteredCourses();
            selectedCourse = null;
        }

    }
}