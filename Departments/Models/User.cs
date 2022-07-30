using System.ComponentModel;
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

        [Required]
        [DisplayName("User Name")]
        public string userName { get; set; }

        [Required]
        public string Password { get; set; }
        public string? ProfilePicture { get; set; }
        public int DepartmentId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public bool IsActivated { get; set; } = false;

        [Required]
        [ForeignKey("Departments")]
        public int DepId { get; set; }

        public virtual Department Department { get; set; }
    }
}
