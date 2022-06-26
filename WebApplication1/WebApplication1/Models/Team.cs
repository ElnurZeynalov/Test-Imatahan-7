using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Role { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
        public string PhotoUrl { get; set; }
    }
}
