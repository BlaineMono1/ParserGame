using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class Setting:BaseEntity
    {
        #region поля
        /// <summary>
        /// Настройка для определенного региона (0- украинаб 1 - турция) ToDo Enum
        /// </summary>
        public int IsRegion { get; set; } = default(int);
        /// <summary>
        /// Почта от аккаунта PsStore
        /// </summary>
        public string? EmailPsStore { get; set; }
        /// <summary>
        /// Пароль от PsStore
        /// </summary>
        public string? PasswordPsStore { get; set; }
        /// <summary>
        /// Одноразовый код
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// Почта пользователя для отпарвки чека 
        /// </summary>
        public string? Email {  get; set; }
        /// <summary>
        /// Платформа игры (0 - ps4, 1- ps5)
        /// </summary>
        public int Platform { get; set; }


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
