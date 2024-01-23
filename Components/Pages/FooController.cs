using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MvcRazor.Controllers;

namespace MvcRazor.Components.Pages
{
    public class FooController : Controller
    {
        public IResult Index(bool hideSidebar = false)
        {
            return this.RazorView<Foo>(new { hideSidebar });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IResult Post(FooModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.RazorView<Foo>();
            }
            
            return this.RazorView<Foo>(new { Model = model });
        }
    }

    public class FooModel
    {
        [Required]
        [MinLength(15)]
        [RegularExpression("[0-9]+")]
        public string Bar { get; set; }
    }
}
