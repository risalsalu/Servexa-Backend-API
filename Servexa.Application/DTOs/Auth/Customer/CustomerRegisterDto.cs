namespace Servexa.Application.DTOs.Auth.Customer;

public class CustomerRegisterDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Role { get; set; } = "Customer";
}
