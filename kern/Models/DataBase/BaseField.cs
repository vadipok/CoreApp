using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace kern.Models.DataBase
{
    public class BaseField
    {
        [Key, Required]
        public int Id { get; set; }
        [Required]
        public string NameField { get; set; }
        [Required]
        public string Deposit { get; set; }
        public int YearOpening { get; set; }
        public int YearStartDevelopment { get; set; }
        public bool isValid { get; set; }
    }
}
