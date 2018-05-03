using Documentation.Data.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Documentation.Data.Entities
{
    public class Document : Base
    {
        public Document()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public Type Type { get; set; }
        [Required]
        [LessEqualCurrentDate]
        public DateTime Date { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{00/0000}")]
        public string SerialNumber { get; set; }
        public string Remarks { get; set; }

    }
}
