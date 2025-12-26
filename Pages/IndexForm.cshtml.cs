
// Pages/IndexForm.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace web_app.Pages
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

        public async Task<IActionResult> OnGetAsync(string? psNo)
        {
            // Prefill when psNo is provided (Edit scenario)
            if (string.IsNullOrWhiteSpace(psNo))
                return Page();

            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = @"
                SELECT fName, lName, psNo, email, phone, department
                FROM dbo.employees
                WHERE psNo = @psNo;
            ";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@psNo", System.Data.SqlDbType.Char, 8).Value = psNo;

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                fName      = reader.IsDBNull(0) ? "" : reader.GetString(0);
                lName      = reader.IsDBNull(1) ? "" : reader.GetString(1);
                this.psNo  = reader.GetString(2);
                email      = reader.GetString(3);
                phone      = reader.GetString(4);
                department = reader.GetString(5);
            }
            else
            {
                ViewData["SuccessMessage"] = $"No employee found for PS No {psNo}. You can add a new one.";
                // keep psNo prefilled to allow adding new record with this psNo
                this.psNo = psNo;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Server-side validations (optional but recommended)
            if (!System.Text.RegularExpressions.Regex.IsMatch(psNo ?? "", @"^\d{8}$"))
            {
                ModelState.AddModelError(nameof(psNo), "PS No must be 8 digits.");
                return Page();
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(phone ?? "", @"^\d{10}$"))
            {
                ModelState.AddModelError(nameof(phone), "Phone must be 10 digits.");
                return Page();
            }
            var allowedDepts = new HashSet<string>
            { "Marketing","Sales","Operations","HR","R&D","Finance","Legal","Management" };
            if (string.IsNullOrWhiteSpace(department) || !allowedDepts.Contains(department))
            {
                ModelState.AddModelError(nameof(department), "Invalid department.");
                return Page();
            }

            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            // ✅ Upsert pattern: Update; if no rows affected, Insert
            var sql = @"
                UPDATE dbo.employees
                SET fName = @fName,
                    lName = @lName,
                    email = @email,
                    phone = @phone,
                    department = @department
                WHERE psNo = @psNo;

                IF @@ROWCOUNT = 0
                BEGIN
                    INSERT INTO dbo.employees (fName, lName, psNo, email, phone, department)
                    VALUES (@fName, @lName, @psNo, @email, @phone, @department);
                END
            ";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@fName", System.Data.SqlDbType.VarChar, 100).Value = fName ?? "";
            cmd.Parameters.Add("@lName", System.Data.SqlDbType.VarChar, 100).Value = lName ?? "";
            cmd.Parameters.Add("@psNo", System.Data.SqlDbType.Char, 8).Value = psNo;
            cmd.Parameters.Add("@email", System.Data.SqlDbType.VarChar, 320).Value = email ?? "";
            cmd.Parameters.Add("@phone", System.Data.SqlDbType.Char, 10).Value = phone ?? "";
            cmd.Parameters.Add("@department", System.Data.SqlDbType.VarChar, 20).Value = department;

            await cmd.ExecuteNonQueryAsync();

            // Show success and reset the form for the next entry
            ViewData["SuccessMessage"] = "Employee details saved successfully (inserted/updated).";
            ModelState.Clear();
            fName = lName = psNo = email = phone = department = string.Empty;

            return Page(); // stay on the same page
        }
    }
}
