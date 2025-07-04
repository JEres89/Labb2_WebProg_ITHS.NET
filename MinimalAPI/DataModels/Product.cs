﻿namespace MinimalAPI.DataModels;

public class Product
{
	public int Id { get; set; }
	public required string Name { get; set; }
	public required string Description { get; set; }
	public required string Category { get; set; }
	public required decimal Price { get; set; }
	public ProductStatus Status { get; set; } = 0;
	public int Stock { get; set; }
	//public ActiveOrdersCollection? ActiveOrders { get; set; }
}

//public class ActiveOrdersCollection
//{
//	public int ProductId { get; set; }
//	public List<int> OrderIds { get; set; }
//}