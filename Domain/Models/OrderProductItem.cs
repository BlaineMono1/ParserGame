using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Item  в ордерах
    /// </summary>
    public class OrderProductItem:BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
