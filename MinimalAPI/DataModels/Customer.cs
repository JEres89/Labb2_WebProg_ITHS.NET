using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI.DataModels;

[Index(nameof(Email), IsUnique = true)]
public class Customer
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	public int Id { get; set; }
	public required string FirstName { get; set; }
	public required string LastName { get; set; }
	public required string Email { get; set; }
	public string? Phone { get; set; }
	public string? Address { get; set; }
	public List<Order>? Orders { get; set; }
}
