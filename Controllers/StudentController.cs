using DomainDrivenDesign.Domain.Entities;
using DomainDrivenDesign.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepositoty _studentRepository;

        public StudentController(IStudentRepositoty studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _studentRepository.GetAll();
            var result = students.Select(s => new
            {
                s.StudentId,
                s.StudentFullName,
                s.StudentClass,
                s.StudentAge,
                s.StudentAddress,
                s.CategoryId,
                CategoryName = s.Category?.CategoryName
            });
            return Ok(new { message = "Success", data = result });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudentId(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                return NotFound(new { message = "Student not found" });
            }
            var result = new
            {
                student.StudentId,
                student.StudentFullName,
                student.StudentClass,
                student.StudentAge,
                student.StudentAddress,
                student.CategoryId,
                CategoryName = student.Category?.CategoryName
            };
            return Ok(new { message = "Success", data = result });
        }
        [HttpPost]
        public async Task<ActionResult> CreateStudent(Student student)
        {
            await _studentRepository.Add(student);
            return Ok(new { message = "Success", data = student });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return BadRequest();
            }
            await _studentRepository.Update(student);
            return Ok(new { message = "Update Success", data = student });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _studentRepository.Delete(id);
            return Ok(new { message = "Delete Success" });
        }
    }
}