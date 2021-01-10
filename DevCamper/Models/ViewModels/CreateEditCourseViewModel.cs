using System.Collections.Generic;

namespace DevCamper.Models.ViewModels
{
    public class CreateEditCourseViewModel
    {
        public Course Course { get; set; }
        public IEnumerable<Skill> Skills { get; set; }

        public string BootcampSlug { get; set; }

        public string BootcampTitle { get; set; }
    }
}
