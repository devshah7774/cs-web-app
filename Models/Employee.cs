
namespace web_app.Models
{
    public class Employee
    {
        public string FName { get; set; } = string.Empty;
        public string LName { get; set; } = string.Empty;
        public string PsNo  { get; set; } = string.Empty;   // char(8)
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;   // char(10)
        public string Department { get; set; } = string.Empty;
    }
}
