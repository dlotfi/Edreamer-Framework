using System.ComponentModel.DataAnnotations;
using Edreamer.Framework.DataAnnotations;

namespace Edreamer.Framework.Domain
{
    public class Module
    {
        [Required, ShortString]
        public string Name { get; set; }
    }
}
