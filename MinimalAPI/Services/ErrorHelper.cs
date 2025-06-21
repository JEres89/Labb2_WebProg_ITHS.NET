using Microsoft.Data.SqlClient;
using MinimalAPI.DataModels;
using System.Collections.ObjectModel;
using static System.Net.HttpStatusCode;

namespace MinimalAPI.Services;

public static class ErrorHelper
{
	private static readonly ReadOnlyCollection<int> _duplicateSqlErrors = new([2398, 2627, 2601]);

	public static ReadOnlyCollection<int> DuplicateSqlErrors => _duplicateSqlErrors;

	public static async Task<ValidationResult<T>> RollbackOnSqlServerDuplicateError<T>(IUnitOfWork worker, SqlException e, Dictionary<string, string>? errorMatches = null)
	{
		string? message = null;
		if(errorMatches != null)
		{
			foreach(var errorMatch in errorMatches)
			{
				if(e.Message.Contains(errorMatch.Key))
					message = errorMatch.Value;
			}
		}

		await worker.RollbackAsync();
		return new ValidationResult<T> {
			ResultCode = BadRequest,
			ErrorMessage = message??$"Unique value already in use: {e.Message.Split("value is")[^1]}"
		};
	}

	public static async Task<ValidationResult<T>> RollbackOnSqlServerError<T>(IUnitOfWork worker, SqlException e, string action)
	{
		await worker.RollbackAsync();
		return new ValidationResult<T> {
			ResultCode = BadRequest,
			ErrorMessage = $"An error occurred while {action}. SqlErr {e.Number}: {e.Message}"
		};
	}

	public static async Task<ValidationResult<T>> RollbackOnServerError<T>(IUnitOfWork worker, Exception e)
	{
		await worker.RollbackAsync();
		if(e.Message.Contains("could not be found"))
		{
			await worker.RollbackAsync();
			return new ValidationResult<T> {
				ResultCode = BadRequest,
				ErrorMessage = "The request contained one or more invalid property names."
			};
		}
		return new ValidationResult<T> {
			ResultCode = InternalServerError,
			ErrorMessage = $"{e.Message}{e.InnerException?.Message.Prepend('\n')??""}"
		};
	}
}
