using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsvUpdateDemo.Infrastructure.Database
{
    [Table("Employees")]
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string FullName { get; set; }

        [MaxLength(120)]
        public string Title { get; set; }

        public int EmploymentType { get; set; }

        [MaxLength(120)]
        public string Location { get; set; }

        public int UserId { get; set; }
    }
}
