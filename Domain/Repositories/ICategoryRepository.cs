using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAll();
        Task<Category?> GetById(int id);
        Task  Add(Category category);
        Task Update(Category category);
        Task Delete(int id);
    }
}