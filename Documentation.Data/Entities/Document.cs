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
        public int TypeId { get; set; }
        public Type Type { get; set; }
        [Required]
        [LessEqualCurrentDate]
        [DisplayFormat(DataFormatString = "{0:M/d/yyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Required]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{00/0000}")]
        [RegularExpression("(0[1-9]|[1-9]{2})/20[0-9]{2}$", ErrorMessage = "Format should be as 00/0000")]
        public string SerialNumber { get; set; }
        public string Remarks { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }

    }
}
