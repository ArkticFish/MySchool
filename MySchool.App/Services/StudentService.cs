using Blazored.LocalStorage;
using MySchool.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MySchool.App.Services
{
    public class StudentService
    {

        private HttpClient _httpClient;
        ILocalStorageService LocalStorage;

        public StudentService(ILocalStorageService LocalStorage)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7000") };
            this.LocalStorage = LocalStorage;
        }


        // Initialize the service and restore the token from storage
        public async Task InitializeAsync()
        {
            // Load token from local storage.
            var token = await LocalStorage.GetItemAsync<string>("token");

            if (!string.IsNullOrEmpty(token))
            {
                // Set bearer token in the HttpClient headers
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Student Login Method.
        public async Task<string> Login(StudentLogin login)
        {
            var response = await _httpClient.PostAsJsonAsync("student/login", login);
            if (response.IsSuccessStatusCode)
            {
                // Read the token from the response.
                var token = await response.Content.ReadAsStringAsync();
                // Set bearer token in the HttpClient.
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                // Return the token.
                return token;
            }
            else
            {
                throw new Exception("Invalid Login Attempt");
            }
        }

        // Student Logout Method.
        public void Logout()
        {
            // Remove the token from the HttpClient.
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        // Student Register Method.
        public async Task<bool> Register(StudentRegister register)
        {
            var response = await _httpClient.PostAsJsonAsync("student/register", register);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid Registration Attempt");
            }
            return true;
        }

        // Get All Courses Method.
        public async Task<List<Course>> GetAllCourses()
        {
            return await _httpClient.GetFromJsonAsync<List<Course>>("student/getCourses");
        }

        // Get Registered Courses Method.
        public async Task<List<Course>> GetRegisteredCourses()
        {
            return await _httpClient.GetFromJsonAsync<List<Course>>("student/getRegisteredCourses");
        }

        // Register Course Method.
        public async Task<bool> RegisterCourse(int courseId)
        {
            var response = await _httpClient.PostAsJsonAsync($"student/registerCourse", courseId);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid Registration Attempt");
            }
            return true;
        }

        // Deregister Course Method.
        public async Task<bool> DeregisterCourse(int courseId)
        {
            var response = await _httpClient.PostAsJsonAsync($"student/deregisterCourse", courseId);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid Deregistration Attempt");
            }
            return true;
        }

    }

}
