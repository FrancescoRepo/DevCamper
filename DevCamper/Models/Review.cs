using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DevCamper.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        [Range(1, 10, ErrorMessage = "Rating must be a value between 1 and 10")]
        public int Rating { get; set; }

        public int BootcampId { get; set; }

        [ForeignKey("BootcampId")]
        public Bootcamp Bootcamp { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
