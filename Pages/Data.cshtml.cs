
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using web_app.Models;

namespace web_app.Pages
{
    public class DataModel : PageModel
    {
        private readonly IConfiguration _config;
        public DataModel(IConfiguration config) => _config = config;

        public List<Employee> Employees { get; private set; } = new();

        public async Task OnGetAsync()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = @"
                SELECT fName, lName, psNo, email, phone, department
                FROM dbo.employees
                ORDER BY fName ASC, psNo ASC;   -- ensure alphabetical
            ";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Employees.Add(new Employee
                {
                    FName      = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    LName      = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    PsNo       = reader.GetString(2),
                    Email      = reader.GetString(3),
                    Phone      = reader.GetString(4),
                    Department = reader.GetString(5)
                });
            }
        }

        // Named handler for Delete (invoked by asp-page-handler="Delete")
        public async Task<IActionResult> OnPostDeleteAsync(string psNo)
        {
            if (string.IsNullOrWhiteSpace(psNo))
            {
                TempData["WarningMessage"] = "Invalid PS No for deletion.";
                return RedirectToPage();  // back to /data
            }

            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            using var cmd = new SqlCommand("DELETE FROM dbo.employees WHERE psNo = @psNo", conn);
            cmd.Parameters.Add("@psNo", System.Data.SqlDbType.Char, 8).Value = psNo;

            var affected = await cmd.ExecuteNonQueryAsync();
            TempData["SuccessMessage"] = affected > 0
                ? $"Employee (PS No {psNo}) deleted."
                : $"No employee found for PS No {psNo}.";

            return RedirectToPage(); // refresh /data
        }
    }
}
