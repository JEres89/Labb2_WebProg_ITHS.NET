using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;

namespace MinimalAPI.Services;

public static class Extensions
{
	public static bool IsSuccessCode(this HttpStatusCode code)
	{
		return code >= HttpStatusCode.OK && code < HttpStatusCode.Ambiguous;
	}
	public static bool IsErrorCode(this HttpStatusCode code)
	{
		return code >= HttpStatusCode.BadRequest && code < HttpStatusCode.InternalServerError;
	}
	public static bool IsServerError(this HttpStatusCode code)
	{
		return code >= HttpStatusCode.InternalServerError && code < HttpStatusCode.NetworkAuthenticationRequired;
	}

	public static void SetProperty<T>(this EntityEntry<T> entity, string key, string value) where T : class
	{
		MemberEntry member;
		try
		{
			if(!char.IsUpper(key[0]))
				throw new();

			member = entity.Member(key);
		}
		catch(Exception)
		{
			try
			{
				member = entity.Members.First(m => m.Metadata.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase));
			}
			catch(Exception)
			{
				throw new Exception($"Invalid property name: {key}");
			}
		}
		var memberType = member.Metadata.ClrType;
		if(memberType != typeof(string))
		{
			if(memberType.IsEnum)
			{
				if(Enum.TryParse(memberType, value, true, out var enumValue))
				{
					member.CurrentValue = enumValue;
					return;
				}
				else
				{
					throw new Exception($"Invalid value '{value}' for enum type '{memberType.Name}' in property '{key}'");
				}
			}
			// TODO: Enforce input having matching globalization for decimal symbol (, or .)
			member.CurrentValue = Convert.ChangeType(value, member.Metadata.ClrType);
		}
		else
		{
			member.CurrentValue = value;
		}
	}
}
