//using Microsoft.Build.Framework;

using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Models
{
    public class Category:BaseEntity
    {
        [Required (ErrorMessage ="Don't be empty")]
        [StringLength(10,ErrorMessage ="Max 50 characters")]
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
