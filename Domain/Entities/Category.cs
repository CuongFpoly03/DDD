
using System.ComponentModel.DataAnnotations;

namespace DomainDrivenDesign.Domain.Entities
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }

         // Đảm bảo ICollection<Student> được khởi tạo
        public virtual ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}