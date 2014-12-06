using blqw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo
{
    [SourceName(Name = "USERS")]
    public class User
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool? Sex { get; set; }
        public DateTime Birthday { get; set; }
    }
}
