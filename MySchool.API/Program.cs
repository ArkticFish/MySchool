using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySchool.API.DatabaseContext;
using MySchool.Models;
using StudentRegistry.Api.Services;
using System.Text;

namespace MySchool.API
{

    public class Program
    {

        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext.
            builder.Services.AddDbContext<StudentDbContext>(options => options.UseInMemoryDatabase("MyInMemoryDb"));

            // Add StudentService.
            builder.Services.AddScoped<StudentService>();

            // Get Jwt settings from appsettings.json.
            var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
            var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

            // Add JwtBearer authentication.
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

            builder.Services.AddControllers();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Add CORS.
            app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            // Add course data.
            AddCourses(app);

            app.Run();

        }

        private static void AddCourses(IHost app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StudentDbContext>();

                // Check if the database has been seeded already.
                if (!context.Courses.Any())
                {
                    context.Courses.AddRange(
                        new Course { CourseName = "Mathematics 101" },
                        new Course { CourseName = "History 101" },
                        new Course { CourseName = "Science 101" },
                        new Course { CourseName = "Programming 101" }
                    );

                    context.SaveChanges();
                }
            }
        }

    }

}
