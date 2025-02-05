using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Товары для покупки Joy
    /// </summary>
    public class LoyaltyProduct:BaseEntity
    {
        #region поля
        /// <summary>
        /// Кол-во joy
        /// </summary>
        public decimal CountJoy { get; set; }
        /// <summary>
        /// Цена товара 
        /// </summary>
        public decimal Price { get; set; }
        #endregion
    }
}
