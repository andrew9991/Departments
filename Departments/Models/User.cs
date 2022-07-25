using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Departments.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Departments")]
        public string DepartmentName { get; set; }

        public virtual Department Department { get; set; }
    }
}
