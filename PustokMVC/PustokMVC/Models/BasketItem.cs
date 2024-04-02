namespace PustokMVC.Models
{
    public class BasketItem : BaseEntity
    {
        public string AppUserId { get; set; }
        public int BookId { get; set; }
        public int Count { get; set; }

        public Book Book { get; set; }
        public AppUser AppUser { get; set; }
    }
}
