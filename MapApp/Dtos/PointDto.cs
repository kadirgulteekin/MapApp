using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MapApp.Dtos
{
    public class PointDto
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName ="decimal(18,2)")]
        public double Lat { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public double Lng { get; set; }
        [Column(TypeName = "Datetime")]

        public DateTime DateTime { get; set; }
    }
}
