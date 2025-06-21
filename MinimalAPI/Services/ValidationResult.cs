using System.Net;

namespace MinimalAPI.Services;

public struct ValidationResult<T>
{
	public HttpStatusCode ResultCode;

	public T? ResultValue;

	public string? ErrorMessage;
}