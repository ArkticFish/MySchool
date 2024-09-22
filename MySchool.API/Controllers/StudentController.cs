using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MySchool.API.Extensions;
using MySchool.Models;
using StudentRegistry.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MySchool.API.Controllers
{

    [ApiController]
    [Route("student")]
    public class StudentController : ControllerBase
    {

        private readonly IConfiguration _config;
        private readonly StudentService _studentService;

        public StudentController(IConfiguration config, StudentService studentService)
        {
            _config = config;
            _studentService = studentService;
        }

        [HttpPost("register")]
        public IActionResult RegisterStudent([FromBody] StudentRegister student)
        {
            _studentService.RegisterStudent(student);
            return Ok();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] StudentLogin loginRequest)
        {

            // Get student from student service.
            var student = _studentService.Login(loginRequest);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var Sectoken = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                [ 
                    new Claim("StudentId", student.StudentId.ToString()),
                    new Claim("StudentEmail", student.Email),
                    new Claim(ClaimTypes.Role, "student")
                ],
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

            return Ok(token);
        }

        [Authorize(Roles = "student")]
        [HttpGet("getStudent")]
        public IActionResult GetStudent()
        {
            return Ok(_studentService.GetStudent(this.GetStudentId()));
        }

        [Authorize(Roles = "student")]
        [HttpGet("getCourses")]
        public IActionResult GetAvailableCourses()
        {
            return Ok(_studentService.GetAvailableCourses());
        }

        [Authorize(Roles = "student")]
        [HttpPost("registerCourse")]
        public IActionResult RegisterCourse([FromBody] int courseId)
        {
            _studentService.RegisterCourse(this.GetStudentId(), courseId);
            return Ok();
        }

        [Authorize(Roles = "student")]
        [HttpPost("deregisterCourse")]
        public IActionResult DeregisterFromCourse([FromBody] int courseId)
        {
            _studentService.DeregisterFromCourse(this.GetStudentId(), courseId);
            return Ok();
        }

        [Authorize(Roles = "student")]
        [HttpGet("getRegisteredCourses")]
        public IActionResult GetRegisteredCourses()
        {
            return Ok(_studentService.GetRegisteredCourses(this.GetStudentId()));
        }

    }

}
