using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MVC_DB_.Models
{
    public class account
    {
        public int ID { get; set; }

        [Required]
        public string userName { get; set; } = string.Empty;

        [Required]
        public string passwd { get; set; } = string.Empty;

        [Required]
        public string name { get; set; } = string.Empty;
    }

}
