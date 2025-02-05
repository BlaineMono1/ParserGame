using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Объект в корзине юзера
    /// </summary>
    public class CartItem:BaseEntity
    {
        #region поля 

        #endregion


        #region связи
        /// <summary>
        /// Корзина
        /// </summary>
        public Guid CartId {  get; set; }
        [ForeignKey("CartId")]
        public Cart Cart { get; set; }
        /// <summary>
        /// Товар
        /// </summary>
        /// 
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        #endregion
    }
}
