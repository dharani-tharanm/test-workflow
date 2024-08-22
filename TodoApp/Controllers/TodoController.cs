using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TodoApp.Models;

namespace TodoApp.Controllers
{
    public class TodoController : Controller
    {
        private readonly HttpClient _httpClient;

        public TodoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            List<Todo>? tasks;
            try
            {
                tasks = await _httpClient.GetFromJsonAsync<List<Todo>>("https://todoapi-backend.azurewebsites.net/api/Tasks");
                if (tasks == null)
                {
                    tasks = new List<Todo>();
                    ModelState.AddModelError(string.Empty, "No tasks available.");
                }
            }
            catch (HttpRequestException)
            {
                tasks = new List<Todo>();
                ModelState.AddModelError(string.Empty, "Error fetching tasks. Please try again later.");
            }
            return View(tasks);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Todo todo)
        {
            if (!ModelState.IsValid)
            {
                return View(todo);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://todoapi-backend.azurewebsites.net/api/Tasks", todo);
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error creating task. Please try again later.");
                return View(todo);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"https://todoapi-backend.azurewebsites.net/api/Tasks/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "Error deleting task. Please try again later.");
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
