namespace MySchool.Models
{

    public class Student
    {
        public int StudentId { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }

}
