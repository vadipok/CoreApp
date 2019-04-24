using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace kern.Models.DataBase
{
    public class AccauntUser
    {
        [Key, Required]
        public int IdUser { get; set; }
        public string Login { get; set; }
        public string UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public bool IsValid { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
