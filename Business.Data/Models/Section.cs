using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    public class Section:BaseEntity
    {
        #region поля 
        /// <summary>
        /// Наименование раздела
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Для загрузки изображения
        /// </summary>
        public string FilePathImage { get; set; }
        #endregion

        #region связи
        /// <summary>
        /// Игры
        /// </summary>
        public List<Edition>?Editions { get; set; }
        #endregion

    }
}
