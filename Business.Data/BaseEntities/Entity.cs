
using System.ComponentModel;
using System.Text.Json.Serialization;
using Business.Data.Iterfaces;

namespace Business.Data.BaseEntities
{
    public class Entity : IEntity
    {
        public Entity()
        {
            DateCreate = DateTime.UtcNow;
            DateUpdate = DateCreate;
        }

        /// <summary>
        /// Дата создания записи
        /// </summary>
        [ReadOnly(true)]
        [JsonIgnore]
        public DateTime DateCreate { get ; set ; }
        /// <summary>
        /// Дата обновления записи
        /// </summary>
        [ReadOnly(true)]
        [JsonIgnore]
        public DateTime DateUpdate { get; set; }

        public void UpdateBeforeSave(DateTime now)
        {
            DateUpdate = now;
        }
    }
}
