using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace kern.Models.DataBase
{
    public class AccauntRole
    {
        [Key]
        public int IdRole { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public bool isValid { get; set; }
    }
}
