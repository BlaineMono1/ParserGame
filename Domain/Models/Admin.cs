using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Данные админа 
    /// </summary>
    public class Admin:BaseEntity
    {
        #region поля
        public string Email { get; set; }
        public string Password { get; set; }

        #endregion

        #region связи 

        /// <summary>
        /// Пользователь
        /// </summary>
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        #endregion

    }
}
