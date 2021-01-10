using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DevCamper.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Weeks { get; set; }

        [Required]
        public double Tuition { get; set; }

        public int SkillId { get; set; }

        [ForeignKey("SkillId")]
        public Skill Skill { get; set; }

        public bool ScholarshipsAvailable { get; set; }

        public int BootcampId { get; set; }

        [ForeignKey("BootcampId")]
        public Bootcamp Bootcamp { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
