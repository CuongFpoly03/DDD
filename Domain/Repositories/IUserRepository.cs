using DomainDrivenDesign.Domain.Entities;

namespace DomainDrivenDesign.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAll();
    }
}