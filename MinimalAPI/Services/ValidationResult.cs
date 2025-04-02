namespace MinimalAPI.Services;

public struct ValidationResult<T>
{
	public ValidationResultCode ResultCode;

	public T? ResultValue;

	public string? ErrorMessage;
}
public enum ValidationResultCode
{
	Success,
	Unauthorized,
	NotFound,
	Failed
}
