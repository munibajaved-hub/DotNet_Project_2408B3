using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public partial class CategoryValidation
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Category name is required.")]
        [StringLength(20, ErrorMessage = "Category name must be at most 10 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name can only contain letters.")]

        [Display(Name = "Category Name")]
        public string CateName { get; set; } = null!;

        public string? Description { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
