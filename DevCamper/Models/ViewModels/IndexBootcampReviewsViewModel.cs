using System.Collections.Generic;

namespace DevCamper.Models.ViewModels
{
    public class IndexBootcampReviewsViewModel
    {
        public IEnumerable<Review> Reviews { get; set; }
        public string Slug { get; set; }
        public string BootcampTitle { get; set; }
    }
}
