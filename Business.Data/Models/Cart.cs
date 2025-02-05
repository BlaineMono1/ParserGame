using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Корзина
    /// </summary>
    public class Cart:BaseEntity
    {
        #region связи
        /// <summary>
        /// Пользователь
        /// </summary>
        public Guid UserId { get; set; }
        public User User { get; set; }

        public List<CartItem>? CartItems { get; set; }
        #endregion
    }
}
