using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Избранное 
    /// </summary>
    public class Favorite:BaseEntity
    {
        #region поля 
        #endregion

        #region связи
        /// <summary>
        /// Пользователь
        /// </summary>
        public Guid UserId { get; set; }
       
        public User User { get; set; }
        /// <summary>
        ///Объект
        /// </summary>
        public List<FavoriteItem>? FavoriteItems { get; set; }
        #endregion
    }
}
