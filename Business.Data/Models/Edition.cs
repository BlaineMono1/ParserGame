using System.ComponentModel.DataAnnotations.Schema;
using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class Edition:BaseEntity
    {
        #region поля 
        /// <summary>
        /// Куса код игры 
        /// </summary>
        public string CusaCode {  get; set; }
        /// <summary>
        /// Тип объекта(Игра)
        /// </summary>
        public string Type {  get; set; }

        /// <summary>
        /// Наименование издания 
        /// </summary>
        public string EditionName { get; set; }
        /// <summary>
        /// Url изображения
        /// </summary>
        public string Image {  get; set; }
        /// <summary>
        /// Платформа игры (0 - ps4, 1- ps5, 2- ps4/ps5)
        /// </summary>
        public int Platform { get; set; }
        /// <summary>
        /// Входит в подписку 
        /// </summary>
        public string? Subscription {  get; set; }
        /// <summary>
        /// Что входит в издание
        /// </summary>
        public string Features { get; set; }
        /// <summary>
        /// Кол-во оценок 
        /// </summary>
        public string Popular {  get; set; }
        /// <summary>
        /// Рейтинг игры 
        /// </summary>
        public float Rating { get; set; }

        #endregion

        #region связи
        /// <summary>
        /// Id игры концепт
        /// </summary>
        public Guid GameId { get; set; }
        [ForeignKey("GameId")]
        public Game Game {  get; set; }
        /// <summary>
        /// Id товара 
        /// </summary>
        public Guid ProductId {  get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        #endregion

    }
}
