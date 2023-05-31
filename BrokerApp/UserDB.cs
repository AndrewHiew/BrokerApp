using System.ComponentModel.DataAnnotations;

namespace BrokerApp
{
    public class UserDB
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public double AssetsValue { get; set; }
        public double CurrentBalance { get; set; }
    }
}