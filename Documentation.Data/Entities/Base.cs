using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.Entities
{
    public class Base
    {
        public Base()
        {
            CreatedOn = UpdatedOn = DateTime.Now;
            IsDeleted = false;
        }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Guid Creator { get; set; }
        public Guid Updator { get; set; }
        public bool IsDeleted { get; set; }
    }
}
