using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class FavoriteItem:BaseEntity
    {
        #region связи
        /// <summary>
        /// Товар
        /// </summary>
        public Guid ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        #endregion
    }
}
