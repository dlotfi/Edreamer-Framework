using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.DataAnnotations;

namespace Edreamer.Framework.Domain
{
    public class Setting
    {
        public int Id { get; set; }
        [Required, ShortString]
        public string Category { get; set; }
        [Required, ShortString]
        public string Name { get; set; }
        public string Value { get; set; }

        public string ModuleName { get; set; }
    }
}