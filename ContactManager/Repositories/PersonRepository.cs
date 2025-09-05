using Microsoft.EntityFrameworkCore;
using ContactManager.Data;
using ContactManager.Models;

namespace ContactManager.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _context;

        public PersonRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.People
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Person?> GetByIdAsync(int id)
        {
            return await _context.People.FindAsync(id);
        }

        public async Task<Person> AddAsync(Person person)
        {
            person.CreatedAt = DateTime.UtcNow;
            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return person;
        }

        public async Task<Person> UpdateAsync(Person person)
        {
            var existingPerson = await _context.People.FindAsync(person.Id);
            if (existingPerson == null)
                throw new InvalidOperationException("Person not found");

            existingPerson.Name = person.Name;
            existingPerson.DateOfBirth = person.DateOfBirth;
            existingPerson.Married = person.Married;
            existingPerson.Phone = person.Phone;
            existingPerson.Salary = person.Salary;
            existingPerson.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPerson;
        }

        public async Task DeleteAsync(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.People.AnyAsync(p => p.Id == id);
        }

        public async Task<int> GetCountAsync()
        {
            return await _context.People.CountAsync();
        }
    }
}
