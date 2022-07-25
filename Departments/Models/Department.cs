using System.ComponentModel.DataAnnotations;

namespace Departments.Models
{
    public class Department
    {
        [Key]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Description { get; set; }

        [Required]
        public int Limit { get; set; }


    }
}
