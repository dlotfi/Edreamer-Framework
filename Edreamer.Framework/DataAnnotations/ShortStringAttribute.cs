using System.ComponentModel.DataAnnotations;

namespace Edreamer.Framework.DataAnnotations
{
    public class ShortStringAttribute : StringLengthAttribute
    {
        public const int Length = 255;
        
        public ShortStringAttribute()
            : base(Length)
        {
            ErrorMessage = "The length of field {0} should not exceed 255 characters.";
        }
    }
}
