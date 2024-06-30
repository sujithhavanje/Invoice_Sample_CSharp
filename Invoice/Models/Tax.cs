namespace Invoice.Models
{
    public class Tax
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Percentage { get; set; }
    }
}
