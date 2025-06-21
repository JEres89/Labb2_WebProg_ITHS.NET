using System.Collections;

namespace BlazorFrontend.Services;

public static class Common
{
	public static async Task<T?> DoRequest<T>(Func<Task<HttpResponseMessage>> requestMethod, Action<string>? reportCallback = null)
	{
		HttpResponseMessage? response = null;
		try
		{
			response = await requestMethod();
			if(response.IsSuccessStatusCode)
			{
				Type targetType = typeof(T);
				if(targetType == typeof(bool))
				{
					// If T is bool, we assume a successful response means true
					// and any failure means false, so we can return true directly.
					return (T)(object)true; // Cast to object to avoid type mismatch
				}
				else if(response.StatusCode == System.Net.HttpStatusCode.NoContent)
				{
					if(typeof(IEnumerable).IsAssignableFrom(targetType))
					{
						// If T is IEnumerable, we return an empty collection
						if(targetType.GenericTypeArguments.Length > 0) 
						{
							// If T is IEnumerable<T>, we create an empty array of T
							// This works for arrays, lists, etc.
							var oType = targetType.GenericTypeArguments[0];
							return (T)(object)Array.CreateInstance(oType, 0);
						}
						// If T is IEnumerable but not IEnumerable<T>, we return an empty list
						var ctor = targetType.GetConstructor(Type.EmptyTypes);
						return ctor != null ? (T)ctor.Invoke(null)! : default;
					}
					else
					{
						// If T is not IEnumerable, we return default value
						return default;
					}
				}
				return await response.Content.ReadFromJsonAsync<T>();
			}
			else if(reportCallback != null)
			{
				GenerateErrorMessage(response, reportCallback);
			}
			return default;
		}
		catch(TaskCanceledException te)
		{
			reportCallback?.Invoke($"Request timed out, server is unavailable.\n{te.Message}");
			return default;
		}
		catch(Exception e)
		{
			if(reportCallback != null)
			{
				if(response != null)
				{
					GenerateErrorMessage(response, reportCallback, e);
				}
				else
				{
					reportCallback(e.Message);
				}
			}
			return default;
		}
	}
	private static async void GenerateErrorMessage(HttpResponseMessage response, Action<string> reportCallback, Exception? e = null)
	{
		if(response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
		{
			reportCallback("Unauthorized access. Please log in again.");
		}
		else if(response.StatusCode == System.Net.HttpStatusCode.Forbidden)
		{
			reportCallback($"Forbidden action.\n{response.ReasonPhrase}");
		}
		else if(e != null)
		{
			reportCallback($"{e.Message}\n{await response.Content.ReadAsStringAsync()}");
		}
		else
		{
			reportCallback($"Error: {response.ReasonPhrase}\n{await response.Content.ReadAsStringAsync()}");
		}
	}
}
