namespace MinimalAPI.DataModels;

public enum OrderStatus
{
	undefined = -1,
	New,
	Processing,
	Shipped,
	Delivered
}

public enum ProductStatus
{
	undefined = -1,
	Active,
	Discontinued,
}