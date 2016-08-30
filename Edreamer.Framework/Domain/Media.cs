using System.ComponentModel.DataAnnotations;

namespace Edreamer.Framework.Domain
{
    public class Media
    {
        public int Id { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Path { get; set; }
    }
}
