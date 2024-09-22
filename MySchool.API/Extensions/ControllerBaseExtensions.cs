using Microsoft.AspNetCore.Mvc;

namespace MySchool.API.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static int GetStudentId(this ControllerBase b)
        {
            var id = b.User.Claims.FirstOrDefault((x) => x.Type.Equals("StudentId"))?.Value;
            return Convert.ToInt32(id);
        }

        public static string? GetStudentEmail(this ControllerBase b)
        {
            var email = b.User.Claims.FirstOrDefault((x) => x.Type.Equals("StudentEmail"))?.Value;
            return email;
        }

    }

}
