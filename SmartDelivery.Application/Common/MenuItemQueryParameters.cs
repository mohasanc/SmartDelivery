namespace SmartDelivery.Application.Common
{
    public class MenuItemQueryParameters : QueryParameters
    {
        public Guid? CategoryId { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? IsVegetarian { get; set; }
        public bool? IsVegan { get; set; }
        public bool? IsGlutenFree { get; set; }
        public decimal? MaxPrice { get; set; }
        public decimal? MinPrice { get; set; }
    }
}
