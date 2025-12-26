
// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;    // <-- use Microsoft.Data.SqlClient

namespace web_app.Pages    // <-- adjust namespace to your project
{
    public class IndexModel : PageModel
    {
        [BindProperty] public string fName { get; set; } = string.Empty;
        [BindProperty] public string lName { get; set; } = string.Empty;
        [BindProperty] public string psNo { get; set; } = string.Empty;
        [BindProperty] public string email { get; set; } = string.Empty;
        [BindProperty] public string phone { get; set; } = string.Empty;
        [BindProperty] public string department { get; set; } = string.Empty;

        private readonly IConfiguration _config;
        public IndexModel(IConfiguration config) => _config = config;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            // optional server-side validation guardrails
            if (!System.Text.RegularExpressions.Regex.IsMatch(psNo, @"^\d{8}$"))
            {
                ModelState.AddModelError(nameof(psNo), "PS No must be 8 digits.");
                return Page();
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone, @"^\d{10}$"))
            {
                ModelState.AddModelError(nameof(phone), "Phone must be 10 digits.");
                return Page();
            }
            var allowedDepts = new HashSet<string> { "Marketing","Sales","Operations","HR","R&D","Finance","Legal","Management" };
            if (!allowedDepts.Contains(department))
            {
                ModelState.AddModelError(nameof(department), "Invalid department.");
                return Page();
            }

            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            conn.Open();

            using var cmd = new SqlCommand(@"
                INSERT INTO dbo.employees (fName, lName, psNo, email, phone, department)
                VALUES (@fName, @lName, @psNo, @email, @phone, @department)
            ", conn);

            cmd.Parameters.Add("@fName", System.Data.SqlDbType.VarChar, 50).Value = fName;
            cmd.Parameters.Add("@lName", System.Data.SqlDbType.VarChar, 50).Value = lName;
            cmd.Parameters.Add("@psNo", System.Data.SqlDbType.Char, 8).Value = psNo;
            cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 254).Value = email;
            cmd.Parameters.Add("@phone", System.Data.SqlDbType.Char, 10).Value = phone;
            cmd.Parameters.Add("@department", System.Data.SqlDbType.VarChar, 20).Value = department;

            cmd.ExecuteNonQuery();

            // Redirect to a page or same page with a success message
            return RedirectToPage("/Index");
        }
    }
}
