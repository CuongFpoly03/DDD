using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Repositories
{
    public interface IStudentRepositoty
    {
        Task<IEnumerable<Student>> GetAll();
        Task<Student?> GetById(int id);
        Task Add(Student student);
        Task Update(Student student);
        Task Delete(int id);
    }
}