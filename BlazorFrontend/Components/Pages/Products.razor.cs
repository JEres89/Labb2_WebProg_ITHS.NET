using MinimalAPI.DataModels;
using MinimalAPI.DTOs.Requests.Products;
using MinimalAPI.DTOs.Responses.Products;

namespace BlazorFrontend.Components.Pages;
public partial class Products
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

	private ProductCollectionResponse? productsCollection;

	/// <summary>
	/// GetProductsAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetProductsAsync()
	{
		resultMessage = null;

		productsCollection = await ProductService.GetProductsAsync(ResultMessage);
	}

	private int? getProductId;
	private bool getProductInvalid => (getProductId??0) <= 0;

	private ProductResponse? productResult;

	/// <summary>
	/// GetProductAsync
	/// </summary>
	private async Task GetProductAsync()
	{
		resultMessage = null;

		productResult = await ProductService.GetProductAsync(getProductId!.Value, ResultMessage);
	}

	private string? createProductName;
	private string? createProductDescription;
	private string? createProductCategory;
	private decimal? createProductPrice;
	private int? createProductStock;

	private bool createProductInvalid => 
		string.IsNullOrWhiteSpace(createProductName) || 
		string.IsNullOrWhiteSpace(createProductDescription) || 
		string.IsNullOrWhiteSpace(createProductCategory) ;

	private ProductResponse? createProductResult;

	/// <summary>
	/// CreateProductAsync
	/// </summary>
	private async Task CreateProductAsync()
	{
		resultMessage = null;
		var request = new ProductCreateRequest {
			Name = createProductName!,
			Description = createProductDescription!,
			Category = createProductCategory!,
			Price = createProductPrice??0,
			Stock = createProductStock??0
		};
		createProductResult = await ProductService.CreateProductAsync(request, ResultMessage);
	}

	private int? updateProductId;

	private ProductUpdateRequest updateProductFields = new ProductUpdateRequest();

	private string? updateProductName {
		get => updateProductFields.TryGetValue("Name", out var value) ? value : null;
		set {
			if(value != null)
				updateProductFields["Name"] = value;
			else
				updateProductFields.Remove("Name");
		}
	}

	private string? updateProductDescription {
		get => updateProductFields.TryGetValue("Description", out var value) ? value : null;
		set {
			if(value != null)
				updateProductFields["Description"] = value;
			else
				updateProductFields.Remove("Description");
		}
	}
	private string? updateProductCategory {
		get => updateProductFields.TryGetValue("Category", out var value) ? value : null;
		set {
			if(value != null)
				updateProductFields["Category"] = value;
			else
				updateProductFields.Remove("Category");
		}
	}
	private decimal? updateProductPrice {
		get {
			if(updateProductFields.TryGetValue("Price", out var value))
				return Decimal.TryParse(value, out var price) ? price : null;
			else
				return null;
		}
		set {
			if(value.HasValue)
				updateProductFields["Price"] = value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
			else
				updateProductFields.Remove("Price");
		}
	}
	private ProductStatus? updateProductStatus {
		get {
			if(updateProductFields.TryGetValue("Status", out var value))
				return int.TryParse(value, out var status) ? (ProductStatus)status : null;
			else
				return null;
		}
		set {
			if(value.HasValue)
				updateProductFields["Status"] = ((int)value.Value).ToString();
			else
				updateProductFields.Remove("Status");
		}
	}
	private int? updateProductStock {
		get {
			if(updateProductFields.TryGetValue("Stock", out var value))
				return Int32.TryParse(value, out var stock) ? stock : null;
			else
				return null;
		}
		set {
			if(value.HasValue)
				updateProductFields["Stock"] = value.Value.ToString();
			else
				updateProductFields.Remove("Stock");
		}
	}

	private bool updateProductInvalid => (updateProductId??0) <= 0 || updateProductFields.Count == 0;

	private ProductResponse? updateProductResult;

	/// <summary>
	/// UpdateProductAsync
	/// </summary>
	/// <returns></returns>
	private async Task UpdateProductAsync()
	{
		resultMessage = null;
		updateProductResult = await ProductService.UpdateProductAsync(updateProductId!.Value, updateProductFields, ResultMessage);
	}

	private int? deleteProductId;
	private bool deleteProductValid => (deleteProductId??0) <= 0;

	private string deleteProductResult = string.Empty;

	/// <summary>
	/// DeleteProductAsync
	/// </summary>
	/// <returns></returns>
	private async Task DeleteProductAsync()
	{
		resultMessage = null;
		var result = await ProductService.DeleteProductAsync(deleteProductId!.Value, ResultMessage);
		deleteProductResult = result ? "Product deleted." : "Delete failed or error.";
	}

	private int? getProductOrdersId;
	private bool getProductOrdersValid => (getProductOrdersId??0) <= 0;

	private IEnumerable<ProductOrderResponse>? productOrdersResult;

	/// <summary>
	/// GetProductOrdersAsync
	/// </summary>
	/// <returns></returns>
	private async Task GetProductOrdersAsync()
	{
		resultMessage = null;
		productOrdersResult = await ProductService.GetProductOrdersAsync(getProductOrdersId!.Value, ResultMessage);

		if(productOrdersResult == null)
		{
			if(resultMessage == string.Empty)
			{
				resultMessage = "Unknown error occured.";
			}
		}
		else if(!productOrdersResult.Any())
		{
			resultMessage = "No orders with that product were found.";
		}
	}
}