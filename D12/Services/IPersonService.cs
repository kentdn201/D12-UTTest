using D7.Models;

namespace D7.Services;

public interface IPersonService
{
    List<Person> GetAll();

    Person GetOne(int index);
    void Create(Person person);
    void Update(int index, Person person);
    void Delete(int index);
}
