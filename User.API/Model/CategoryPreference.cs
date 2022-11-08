using System.ComponentModel.DataAnnotations.Schema;

namespace User.Model
{
    public class CategoryPreference
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
