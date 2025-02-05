using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Наценка на подписки для разных регионов
    /// </summary>
    public class PriceSettingSubscription:BaseEntity
    {
        #region поля
        /// <summary>
        /// Наценка на подписки 
        /// </summary>
        public decimal Percent {  get; set; }

        /// <summary>
        /// регион (0-украина/1-турция)
        /// </summary>
        public int Region { get; set; }
        #endregion



        #region связи
        /// <summary>
        /// Подписки
        /// </summary>
        public Guid SubscriptionId { get; set; }
        [ForeignKey("SubscriptionId")]
        public Subscription Subscription { get; set; }
        #endregion
    }
}
