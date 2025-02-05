using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class News:BaseEntity
    {
        #region поля
        /// <summary>
        /// Наименование новости 
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Ссылка на новость 
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Для загрузки изображения
        /// </summary>
        public string FilePathImage { get; set; }
        #endregion
    }
}
