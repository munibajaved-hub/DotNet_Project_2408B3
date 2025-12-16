using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Models;

public partial class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Category Name is required")]
    [StringLength(10)]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Category name can only contain letters.")]
    public string CateName { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
