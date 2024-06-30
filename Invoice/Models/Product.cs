namespace Invoice.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }  // This property corresponds to CategoryId in CSV
        public string CategoryName { get; set; } // This property will hold CategoryName

        // Constructor to initialize properties
        public Product()
        {
            CategoryName = ""; // Default value for CategoryName
        }
    }
}
