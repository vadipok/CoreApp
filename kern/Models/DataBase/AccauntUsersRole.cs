using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace kern.Models.DataBase
{
    public class AccauntUsersRole
    {
        public int Id { get; set; }

        [ForeignKey("AccauntUser")]
        public AccauntUser IdUser { get; set; }

        [ForeignKey("AccauntRole")]
        public int IdRole { get; set; }

        public DateTime CreateDate { get; set; }
    }
}
