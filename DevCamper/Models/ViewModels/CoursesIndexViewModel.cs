using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevCamper.Models.ViewModels
{
    public class CoursesIndexViewModel
    {
        public Bootcamp Bootcamp { get; set; }
        public IEnumerable<Course> Courses { get; set; }
    }
}
