using Business.Data.BaseEntities;

namespace Business.Data.Models
{
    /// <summary>
    /// Игры концепция 
    /// </summary>
    public class Game
    {
        #region поля
        /// <summary>   
        /// Id игры с PsStore
        /// </summary>
        public string ConceptId { get; set; }
        /// <summary>
        /// Наименование игры
        /// </summary>
        public string Name { get; set; }

 
        
        /// <summary>
        /// Язык игры
        /// </summary>
        public string LanguagesVoice {  get; set; }
        public string LanguagesInterface { get; set; }
       
        /// <summary>
        /// В каком регионе игра есть (0-украина, 1- турция, 2 - украина/турция) To Do: Сделать Enum
        /// </summary>

        public  int StarCount {  get; set; }
        #endregion

        #region связи
        /// <summary>
        /// Издания
        /// </summary>
        public List<Edition>? Editions { get; set; }
        /// <summary>
        /// AddOns
        /// </summary>
        //public List<AddOn> AddOns { get; set; }
        #endregion


    }
}
