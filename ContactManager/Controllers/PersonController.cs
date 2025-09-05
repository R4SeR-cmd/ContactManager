using Microsoft.AspNetCore.Mvc;
using ContactManager.Models;
using ContactManager.Services;

namespace ContactManager.Controllers
{
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        public async Task<IActionResult> Index()
        {
            var people = await _personService.GetAllPeopleAsync();
            return View(people);
        }

        [HttpPost]
        public async Task<IActionResult> UploadCsv(IFormFile csvFile)
        {
            if (csvFile == null || csvFile.Length == 0)
            {
                TempData["Message"] = "Please select a CSV file to upload.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var result = await _personService.ProcessCsvFileAsync(csvFile);
            
            TempData["Message"] = result.Message;
            TempData["MessageType"] = result.Success ? "success" : "error";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPerson = await _personService.CreatePersonAsync(person);
                return Json(new { success = true, data = createdPerson });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Person person)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPerson = await _personService.UpdatePersonAsync(person);
                return Json(new { success = true, data = updatedPerson });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var people = await _personService.GetAllPeopleAsync();
            return Json(people);
        }
    }
}
