using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DomainDrivenDesign.Domain.Entities
{
    public class Student
    {
        public int StudentId { get; set; }
        public string? StudentFullName { get; set; }
        public string? StudentClass { get; set; }
        public int StudentAge { get; set; }
        public string? StudentAddress { get; set; }
        public int? CategoryId { get; set; }

         // Thiết lập thuộc tính Category không bắt buộc
        public virtual Category? Category { get; set; }
    }
}