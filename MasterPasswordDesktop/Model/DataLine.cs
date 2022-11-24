using System;
using System.Linq;
using System.Text;

namespace MasterPasswordDesktop.Model
{
    public class DataLine
    {
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } 
        public string Email { get; set; }
        public string Host { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime EditDate { get; set; } = DateTime.Now;
        public DateTime LastViewDate { get; set; } = DateTime.Now;

        public string Other { get; set; }
    }
}
