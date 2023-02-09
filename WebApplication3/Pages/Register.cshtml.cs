using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Web;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private UserManager<Register> userManager { get; }
        private SignInManager<Register> signInManager { get; }


        [BindProperty]
        public Register RModel { get; set; }

        [BindProperty]
        public IFormFile? Upload { get; set; }

        public string[] Genders = new[] { "Male", "Female" };

        public RegisterModel(UserManager<Register> userManager,SignInManager<Register> signInManager, IWebHostEnvironment environment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _environment = environment;
        }



        public void OnGet()
        {
            
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptNRIC");
                var protector = dataProtectionProvider.CreateProtector("NRICKey");
                if (Upload != null)
                {
                    if (Upload.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                        return Page();
                    }
                    var uploadsFolder = "uploads";
                    var resumeFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                    var resumePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, resumeFile);
                    using var fileStream = new FileStream(resumePath, FileMode.Create);
                    await Upload.CopyToAsync(fileStream);
                    RModel.FileURL = string.Format("/{0}/{1}", uploadsFolder, resumeFile);
                }
                var user = new Register()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    Gender = RModel.Gender,
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    NRIC = protector.Protect(RModel.NRIC),
                    DOB= RModel.DOB,
                    WhoamI = HttpUtility.HtmlEncode(RModel.WhoamI),
                    FileURL = RModel.FileURL
                    
                };
                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
               
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }  

            }
            return Page();
        }







    }
}
