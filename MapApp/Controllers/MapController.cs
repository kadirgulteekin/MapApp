using MapApp.Dtos;
using MapApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MapApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly MapDbContext _context;

        public MapController(MapDbContext context)
        {
            _context = context;
        }

        private const string PointsFilePath = "points.json";
        [HttpPost("save")]
        public async Task<IActionResult> SavePoint(PointDto point)
        {
            try
            {
                var points = await ReadPointsFromFile();
                point.Id = points.Count;
                points.Add(point);

                await WritePointsToFile(points);

                //_context.PointDtos.Add(point);
                //await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet("points")]
        public async Task<IActionResult> LoadFromDatabaseAsync()
        {
            try
            {
                var points = await _context.PointDtos.ToListAsync();
                return Ok(points);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePoint(int id)
        {
            try
            {
                var point = _context.PointDtos.FirstOrDefault(p => p.Id == id);
                if(point != null)
                {
                    return BadRequest();
                }

                _context.PointDtos.Remove(point);
                _context.SaveChanges();

                return NoContent();
            }
            catch(Exception ex) 
            {
                throw new Exception("Data silinemedi", ex);

            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadPoints()
        {
            try
            {
                var points = await ReadPointsFromFile();
                return File(JsonSerializer.SerializeToUtf8Bytes(points), "application/json", "points.json");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private async Task<List<PointDto>> ReadPointsFromFile()
        {
            if (System.IO.File.Exists(PointsFilePath))
            {
                var jsonContent = await System.IO.File.ReadAllTextAsync(PointsFilePath);
                return JsonSerializer.Deserialize<List<PointDto>>(jsonContent) ?? new List<PointDto>();
            }
            return new List<PointDto>();
        }

        private async Task WritePointsToFile(List<PointDto> points)
        {
            var jsonContent = JsonSerializer.Serialize(points, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(PointsFilePath, jsonContent);
        }
    }

   
}

