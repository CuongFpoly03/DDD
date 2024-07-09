using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Repositories;
using DomainDrivenDesign.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DomainDrivenDesign.Infrastructure.Persistence
{
    public class StudentRepository : IStudentRepositoty
    {
        private readonly ApplicationDbContext _context;
        public StudentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAll()
        {
            return await _context.Students.Include(s => s.Category).ToListAsync();
        }

        public async Task<Student?> GetById(int id)
        {
             var result =  await _context.Students.Include(s => s.Category).FirstOrDefaultAsync(s => s.StudentId == id);
             return result;
        }

        public async Task Add(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}