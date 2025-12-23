using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project.Models;

namespace Project.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        EcommerceContext dbcontext = new EcommerceContext();
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Category()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Category(CategoryValidation c)
        {
            if (!ModelState.IsValid)
            {
                return View(c);
            }
            //check duplication category
            bool checkdupRec = dbcontext.Categories.Any(x => x.CateName.ToLower() == c.CateName.Trim().ToLower());
            if (checkdupRec)
            {
                ModelState.AddModelError(nameof(c.CateName), "Category already exist");
            }
            var dc = new Category();
            dc.CateName = c.CateName;
            dbcontext.Categories.Add(dc);
            dbcontext.SaveChanges();
            TempData["Success"] = "Category Added Successfully";
            return Redirect("CatList");
        }


        public IActionResult CatList()
        {
            var catList = dbcontext.Categories.ToList();
            return View(catList);
        }

        public IActionResult Delete(int id) 

        {
            var deletetion = dbcontext.Categories.FirstOrDefault(x=>x.Id == id); //1044 ==1044
            if(deletetion != null)
            {
                dbcontext.Categories.Remove(deletetion);
                dbcontext.SaveChanges();
            }
         return RedirectToAction("CatList");
    }

        [HttpGet]
    public IActionResult Edit(int id)

        {
            var cat = dbcontext.Categories.FirstOrDefault(x => x.Id == id);//select * from categories where id = id
            if(cat == null)
            {
                NotFound();
            }
            return View(cat);
        }

        [HttpPost]
        public IActionResult Edit(Category c, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(c);
            }
            var catExisting = dbcontext.Categories.FirstOrDefault(x => x.Id == id);
            if(catExisting == null)
            {
                NotFound();
            }
             catExisting.CateName =  c.CateName; //vegetables
            dbcontext.Categories.Update(catExisting);
            dbcontext.SaveChanges();
            return RedirectToAction("CatList");

        }
        [HttpGet]
        public IActionResult CreateProd()
        {
            ViewBag.CategoryList = new SelectList(dbcontext.Categories, "Id", "CateName");
            return View();
        }


        [HttpPost]
        public IActionResult CreateProd(Product p,IFormFile ImgFile)
        {
            //step 1 Get the image name like product8.jpg
            var imageName = Path.GetFileName(ImgFile.FileName);
            //step 2 create folder path where the image  will be stored e.g www.root/Image/
            var ImagePath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/Image/");
            //step 3 Combine folder Path +image
            string imageValue = Path.Combine(ImagePath, imageName);
            //step 4 Save the image into folder// Move the image from the folder
            using (var stream = new FileStream(imageValue,FileMode.Create))
            {
                ImgFile.CopyTo(stream);
            }
            var dbimage = Path.Combine("/Image/", imageName);
            p.Image = dbimage; //Image/Product8.jpg
            dbcontext.Products.Add(p);
            dbcontext.SaveChanges();
            return View();
        }
        public IActionResult ProdList()
        {
            var datashow = dbcontext.Products.Include(x => x.Category).ToList();
            return View(datashow);
        }

        public IActionResult DeleteProd(int id )
        {

            var deletetion = dbcontext.Products.FirstOrDefault(x => x.Id == id); //1044 ==1044
            if (deletetion != null)
            {
                dbcontext.Products.Remove(deletetion);
                dbcontext.SaveChanges();
            }
            return RedirectToAction("ProdList");
        }
        [HttpGet]
        public IActionResult EditProd(Product p ,int id)
        {
            var ProdExisting = dbcontext.Products.FirstOrDefault(x => x.Id == id);
            ViewBag.CategoryList = new SelectList(dbcontext.Categories, "Id", "CateName");
            if(ProdExisting == null)
            {
                NotFound();
            }
            return View(ProdExisting);

        }
        [HttpPost]
        public IActionResult EditProd(Product p,IFormFile ImgFile)
        {
            var existProd = dbcontext.Products.Find(p.Id);
            if (existProd == null)
            {
                NotFound();
            }
            else
            {
                if(ImgFile != null)
                {
                    //step 1 Get the image name like product8.jpg
                    var imageName = Path.GetFileName(ImgFile.FileName);
                    //step 2 create folder path where the image  will be stored e.g www.root/Image/
                    var ImagePath = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/Image/");
                    //step 3 Combine folder Path +image
                    string imageValue = Path.Combine(ImagePath, imageName);
                    //step 4 Save the image into folder// Move the image from the folder
                    using (var stream = new FileStream(imageValue, FileMode.Create))
                    {
                        ImgFile.CopyTo(stream);
                    }
                    var dbimage = Path.Combine("/Image/", imageName);
                    existProd.Image = dbimage; //Image/Product8.jpg
                }
                existProd.ProductName = p.ProductName;
                existProd.Description = p.Description;
                existProd.Price = p.Price;
                existProd.CategoryId = p.CategoryId;
                existProd.Quantity = p.Quantity;

                dbcontext.Products.Update(existProd);
                dbcontext.SaveChanges();
            }
            
            return RedirectToAction("ProdList");

        }





        //private static List<Student> students = new List<Student>();
        //public IActionResult CreateStud()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public IActionResult CreateStud(Student s)
        //{
        //    students.Add(s);
        //    TempData["Success"] = "Student record save";
        //    return Redirect("ListView");
        //}
        //public IActionResult ListView()
        //{
        //    return View(students);
        //}

    }
}
