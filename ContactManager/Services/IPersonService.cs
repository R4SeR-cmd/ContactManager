using ContactManager.Models;

namespace ContactManager.Services
{
    public interface IPersonService
    {
        Task<IEnumerable<Person>> GetAllPeopleAsync();
        Task<Person?> GetPersonByIdAsync(int id);
        Task<Person> CreatePersonAsync(Person person);
        Task<Person> UpdatePersonAsync(Person person);
        Task DeletePersonAsync(int id);
        Task<(bool Success, string Message, List<Person> People)> ProcessCsvFileAsync(IFormFile file);
    }
}
