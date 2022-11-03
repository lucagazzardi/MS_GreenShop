using System.ComponentModel.DataAnnotations.Schema;

namespace User.Model
{
    public class CategoryPreference
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

        public User User { get; set; }
    }
}
