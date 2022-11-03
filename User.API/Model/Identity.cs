namespace User.Model
{
    public class Identity
    {
        public Guid ID { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public User User { get; set; }
    }
}
