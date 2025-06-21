using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Auth;
using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Auth;
using MinimalAPI.Services.Database;

namespace MinimalAPI.Services.Auth;

public class AuthRepository : IAuthRepository
{
	private readonly ApiContext _context;

	public AuthRepository(ApiContext context)
	{
		_context = context;
	}
	public async Task<IEnumerable<WebUser>?> GetUsersAsync()
	{
		return await _context.Users.ToListAsync();
	}

	public async Task<WebUser?> GetUserByEmailAsync(string email)
	{
		return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
	}

	public async Task<WebUser?> AddUserAsync(WebUser user)
	{
		var entry = await _context.Users.AddAsync(user);
		//await _context.SaveChangesAsync();
		return entry.Entity;
	}

	public async Task<bool> DeleteUserAsync(string email)
	{
		var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
		if (user == null)
		{
			return false;
		}
		_context.Users.Remove(user);
		//await _context.SaveChangesAsync();
		return true;
	}

	public void Dispose()
	{
		//Dispose(true);
		GC.SuppressFinalize(this);
	}
}
