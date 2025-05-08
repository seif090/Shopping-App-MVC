namespace CartApp.Models
{
    public class WishListItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public DateTime DateAdded { get; set; }
    }
}