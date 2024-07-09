using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Repositories;
using DomainDrivenDesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesign.Infrastructure.Persistence
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context; 
        }

        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _context.Categorys.ToListAsync();
        }

        public async Task<Category?> GetById(int id)
        {
            return await _context.Categorys.FindAsync(id);
        }

        public async Task Add(Category category)
        {
            await _context.Categorys.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Category category)
        {
            _context.Categorys.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var category = await _context.Categorys.FindAsync(id);
            if (category != null)
            {
                _context.Categorys.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
