using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CsvUpdateDemo.Infrastructure.Database
{
    [Table("Products")]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(64)]
        [Index("IX_Products_Sku", IsUnique = true)]
        public string Sku { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public DateTime UpdatedAtUtc { get; set; }
    }
}
