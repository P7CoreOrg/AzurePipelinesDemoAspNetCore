using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DemoLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private IDog _dog;

        public IndexModel(IDog dog)
        {
            _dog = dog;
        }
        public void OnGet()
        {
            Name = _dog.Name;
        }

        public string Name { get; set; }
    }
}
