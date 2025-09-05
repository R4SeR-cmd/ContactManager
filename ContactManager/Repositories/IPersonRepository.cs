using ContactManager.Models;

namespace ContactManager.Repositories
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();
        Task<Person?> GetByIdAsync(int id);
        Task<Person> AddAsync(Person person);
        Task<Person> UpdateAsync(Person person);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> GetCountAsync();
    }
}
