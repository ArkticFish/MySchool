namespace MySchool.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public required string CourseName { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; } = new List<StudentCourse>();
    }
}
