using Microsoft.EntityFrameworkCore;
using MySchool.API.DatabaseContext;
using MySchool.Models;

namespace StudentRegistry.Api.Services
{
    public class StudentService
    {

        private readonly StudentDbContext _context;

        public StudentService(StudentDbContext context)
        {
            _context = context;
        }

        public void RegisterStudent(StudentRegister register)
        {
            // Check if student exists.
            if (_context.Students.Any(s => s.Email == register.Email))
            {
                throw new InvalidOperationException("Student already exists.");
            }

            // Create new student with encrypted password.
            var student = new Student
            {
                Name = register.Name,
                Email = register.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(register.Password)
            };

            // Add student to database.
            _context.Students.Add(student);
            _context.SaveChanges();
        }

        public Student Login(StudentLogin login)
        {
            // Check if student exists and password is correct.
            var student = _context.Students.SingleOrDefault(s => s.Email == login.Email);
            if (student == null || !BCrypt.Net.BCrypt.Verify(login.Password, student.Password))
            {
                throw new UnauthorizedAccessException();
            }

            // Return student.
            return student;
        }

        public Student GetStudent(int studentId)
        {
            // Get student.
            var student = _context.Students.Find(studentId);

            // Check if student exists.
            if (student == null)
                throw new InvalidOperationException("Student does not exist.");

            // Remove password from student object.
            student.Password = null;

            // Return student.
            return student;
        }

        public List<Course> GetAvailableCourses()
        {
            // Get all courses.
            return [.. _context.Courses];
        }

        public void RegisterCourse(int studentId, int courseId)
        {
            // Check if student is already enrolled in course.
            if (_context.StudentCourses.Any(sc => sc.StudentId == studentId && sc.CourseId == courseId))
            {
                throw new InvalidOperationException("Student is already enrolled in course.");
            }

            // Get student and course.
            var student = _context.Students.Find(studentId);
            var course = _context.Courses.Find(courseId);

            // Check if student and course exist.
            if (student == null)
            {
                throw new InvalidOperationException("Student does not exist.");
            }
            if (course == null)
            {
                throw new InvalidOperationException("Course does not exist.");
            }

            // Create enrollment.
            var enrollment = new StudentCourse { StudentId = studentId, CourseId = courseId, Student = student, Course = course };

            // Add enrollment to database.
            _context.StudentCourses.Add(enrollment);
            _context.SaveChanges();
        }

        public List<Course> GetRegisteredCourses(int studentId)
        {
            // Get courses for student.
            return _context.StudentCourses
                .Where(sc => sc.StudentId == studentId)
                .Select(sc => sc.Course)
                .ToList();
        }

        public void DeregisterFromCourse(int studentId, int courseId)
        {
            // Remove enrollment from database.
            var enrollment = _context.StudentCourses.FirstOrDefault(sc => sc.StudentId == studentId && sc.CourseId == courseId);
            if (enrollment != null)
            {
                _context.StudentCourses.Remove(enrollment);
                _context.SaveChanges();
            }
        }

    }

}
