using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCamper.Models.ViewModels
{
    public class CreateEditBootcampViewModel
    {
        public Bootcamp Bootcamp { get; set; }
        public IEnumerable<Career> Careers { get; set; }
    }
}
