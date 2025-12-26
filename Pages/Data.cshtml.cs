
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

        // This is what the view will render
        public List<Employee> Employees { get; private set; } = new();

        public async Task OnGetAsync()
        {
            var connStr = _config.GetConnectionString("DefaultConnection");
            using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            // Keep the projection explicit to avoid surprises
            var sql = @"
                SELECT fName, lName, psNo, email, phone, department
                FROM dbo.employees
            ";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                Employees.Add(new Employee
                {
                    FName      = reader.GetString(0),
                    LName      = reader.GetString(1),
                    PsNo       = reader.GetString(2),
                    Email      = reader.GetString(3),
                    Phone      = reader.GetString(4),
                    Department = reader.GetString(5)
                });
            }
        }
    }
}
