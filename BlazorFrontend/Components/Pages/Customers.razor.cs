using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Customers;
using MinimalAPI.DTOs.Responses.Customers;
using MinimalAPI.DTOs.Responses.Orders;

namespace BlazorFrontend.Components.Pages;
public partial class Customers
{
	private string? resultMessage;
	private void ResultMessage(string message)
	{
		// Handle the result message as needed
		InvokeAsync(() =>
		{
			resultMessage = message;
		});
	}


	#region GetCustomers
	private CustomerCollectionResponse? customersCollection;

	/// <summary>
	/// GetCustomersAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetCustomersAsync()
	{
		resultMessage = null;

		customersCollection = await CustomerService.GetCustomersAsync(ResultMessage);
	}
	#endregion


	#region GetCustomer
	private int? getCustomerId;
	private bool getCustomerInvalid => (getCustomerId??0) <= 0;

	private CustomerResponse? customerResult;

	/// <summary>
	/// GetCustomerAsync
	/// </summary>
	private async Task GetCustomerAsync()
	{
		resultMessage = null;

		customerResult = await CustomerService.GetCustomerAsync(getCustomerId!.Value, ResultMessage);
	}
	#endregion


	#region CreateCustomer
	private string? createCustomerFirstName;
	private string? createCustomerLastName;
	private string? createCustomerEmail;
	private string? createCustomerPhone;
	private string? createCustomerAddress;

	private bool createCustomerInvalid => 
		string.IsNullOrWhiteSpace(createCustomerFirstName) || 
		string.IsNullOrWhiteSpace(createCustomerLastName) || 
		string.IsNullOrWhiteSpace(createCustomerEmail) ;

	private CustomerResponse? createCustomerResult;

	/// <summary>
	/// CreateCustomerAsync
	/// </summary>
	private async Task CreateCustomerAsync()
	{
		resultMessage = null;
		var request = new CustomerCreateRequest {
			FirstName = createCustomerFirstName!,
			LastName = createCustomerLastName!,
			Email = createCustomerEmail!,
			Phone = createCustomerPhone,
			Address = createCustomerAddress
		};
		createCustomerResult = await CustomerService.CreateCustomerAsync(request, ResultMessage);
	}
	#endregion


	#region UpdateCustomer
	private int? updateCustomerId;

	private CustomerUpdateRequest updateCustomerFields = new CustomerUpdateRequest();

	private string? updateCustomerFirstName {
		get => updateCustomerFields.TryGetValue("FirstName", out var value) ? value : null;
		set {
			if(value != null)
				updateCustomerFields["FirstName"] = value;
			else
				updateCustomerFields.Remove("FirstName");
		}
	}

	private string? updateCustomerLastName {
		get => updateCustomerFields.TryGetValue("LastName", out var value) ? value : null;
		set {
			if(value != null)
				updateCustomerFields["LastName"] = value;
			else
				updateCustomerFields.Remove("LastName");
		}
	}
	private string? updateCustomerEmail {
		get => updateCustomerFields.TryGetValue("Email", out var value) ? value : null;
		set {
			if(value != null)
				updateCustomerFields["Email"] = value;
			else
				updateCustomerFields.Remove("Email");
		}
	}
	private string? updateCustomerPhone {
		get => updateCustomerFields.TryGetValue("Phone", out var value) ? value : null;
		set {
			if(value != null)
				updateCustomerFields["Phone"] = value;
			else
				updateCustomerFields.Remove("Phone");
		}
	}
	private string? updateCustomerAddress {
		get => updateCustomerFields.TryGetValue("Address", out var value) ? value : null;
		set {
			if(value != null)
				updateCustomerFields["Address"] = value;
			else
				updateCustomerFields.Remove("Address");
		}
	}

	private bool updateCustomerInvalid => (updateCustomerId??0) <= 0 || updateCustomerFields.Count == 0;

	private CustomerResponse? updateCustomerResult;

	/// <summary>
	/// UpdateCustomerAsync
	/// </summary>
	/// <returns></returns>
	private async Task UpdateCustomerAsync()
	{
		resultMessage = null;
		updateCustomerResult = await CustomerService.UpdateCustomerAsync(updateCustomerId!.Value, updateCustomerFields, ResultMessage);
	}
	#endregion


	#region DeleteCustomer
	private int? deleteCustomerId;
	private bool deleteCustomerInvalid => (deleteCustomerId??0) <= 0;

	private string deleteCustomerResult = string.Empty;

	/// <summary>
	/// DeleteCustomerAsync
	/// </summary>
	/// <returns></returns>
	private async Task DeleteCustomerAsync()
	{
		resultMessage = null;
		var result = await CustomerService.DeleteCustomerAsync(deleteCustomerId!.Value, ResultMessage);
		deleteCustomerResult = result ? "Customer deleted." : "Delete failed or error.";
	}
	#endregion


	#region GetCustomerOrders
	private int? getCustomerOrdersId;
	private bool getCustomerOrdersValid => (getCustomerOrdersId??0) <= 0;

	private OrderCollectionResponse? customerOrdersResult;

	/// <summary>
	/// GetCustomerOrdersAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetCustomerOrdersAsync()
	{
		resultMessage = null;
		customerOrdersResult = (await CustomerService.GetCustomerOrdersAsync(getCustomerOrdersId!.Value, ResultMessage));

		if(customerOrdersResult == null)
		{
			if(resultMessage == string.Empty)
			{
				resultMessage = "Unknown error occured.";
			}
		}
		else if(!customerOrdersResult.Any())
		{
			resultMessage = "No orders from that customer were found.";
		}
	}
	#endregion
}