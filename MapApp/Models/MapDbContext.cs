using MapApp.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MapApp.Models
{
    public class MapDbContext : DbContext
    {
        public MapDbContext(DbContextOptions<MapDbContext> options) : base(options) 
        {
            
        }
        public DbSet<PointDto> PointDtos { get; set; }
    }
}
