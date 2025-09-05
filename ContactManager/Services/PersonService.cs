using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using ContactManager.Models;
using ContactManager.Repositories;

namespace ContactManager.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _personRepository;

        public PersonService(IPersonRepository personRepository)
        {
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<Person>> GetAllPeopleAsync()
        {
            return await _personRepository.GetAllAsync();
        }

        public async Task<Person?> GetPersonByIdAsync(int id)
        {
            return await _personRepository.GetByIdAsync(id);
        }

        public async Task<Person> CreatePersonAsync(Person person)
        {
            return await _personRepository.AddAsync(person);
        }

        public async Task<Person> UpdatePersonAsync(Person person)
        {
            return await _personRepository.UpdateAsync(person);
        }

        public async Task DeletePersonAsync(int id)
        {
            await _personRepository.DeleteAsync(id);
        }

        public async Task<(bool Success, string Message, List<Person> People)> ProcessCsvFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (false, "No file was uploaded.", new List<Person>());

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                return (false, "Only CSV files are allowed.", new List<Person>());

            try
            {
                var people = new List<Person>();
                var errors = new List<string>();

                using var reader = new StreamReader(file.OpenReadStream());
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    Delimiter = ","
                });

                var records = csv.GetRecords<dynamic>();
                int rowNumber = 1;

                foreach (var record in records)
                {
                    rowNumber++;
                    try
                    {
                        var person = new Person
                        {
                            Name = record.Name?.ToString() ?? "",
                            DateOfBirth = ParseDate(record.DateOfBirth?.ToString()),
                            Married = ParseBool(record.Married?.ToString()),
                            Phone = record.Phone?.ToString() ?? "",
                            Salary = ParseDecimal(record.Salary?.ToString())
                        };

                        if (ValidatePerson(person, out var validationErrors))
                        {
                            people.Add(person);
                        }
                        else
                        {
                            errors.Add($"Row {rowNumber}: {string.Join(", ", validationErrors)}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Row {rowNumber}: Error processing row - {ex.Message}");
                    }
                }

                if (errors.Any())
                {
                    return (false, $"CSV processing completed with errors:\n{string.Join("\n", errors)}", people);
                }

                // Save all valid records to database
                foreach (var person in people)
                {
                    await _personRepository.AddAsync(person);
                }

                return (true, $"Successfully processed {people.Count} records from CSV file.", people);
            }
            catch (Exception ex)
            {
                return (false, $"Error processing CSV file: {ex.Message}", new List<Person>());
            }
        }

        private DateTime ParseDate(string? dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return DateTime.MinValue;

            if (DateTime.TryParse(dateString, out var result))
                return result;

            return DateTime.MinValue;
        }

        private bool ParseBool(string? boolString)
        {
            if (string.IsNullOrWhiteSpace(boolString))
                return false;

            return boolString.ToLower() switch
            {
                "true" or "1" or "yes" or "y" => true,
                "false" or "0" or "no" or "n" => false,
                _ => false
            };
        }

        private decimal ParseDecimal(string? decimalString)
        {
            if (string.IsNullOrWhiteSpace(decimalString))
                return 0;

            // Parse using invariant culture to avoid locale-specific decimal separator issues
            if (decimal.TryParse(decimalString, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;

            return 0;
        }

        private bool ValidatePerson(Person person, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(person.Name))
                errors.Add("Name is required");

            if (person.DateOfBirth == DateTime.MinValue)
                errors.Add("Valid date of birth is required");

            if (string.IsNullOrWhiteSpace(person.Phone))
                errors.Add("Phone is required");

            if (person.Salary < 0)
                errors.Add("Salary must be non-negative");

            return !errors.Any();
        }
    }
}
