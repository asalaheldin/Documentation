using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.Attributes
{
    public class LessEqualCurrentDateAttribute : RangeAttribute
    {
        public LessEqualCurrentDateAttribute()
        : base(typeof(DateTime), DateTime.MinValue.ToShortDateString(), DateTime.Now.ToShortDateString()) { }
    }
}
