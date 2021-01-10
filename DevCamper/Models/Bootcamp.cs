using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DevCamper.Models
{
    public class Bootcamp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Website { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        public byte[] Picture { get; set; }

        public bool Housing { get; set; }

        public bool JobAssistance { get; set; }

        public bool JobGuarantee { get; set; }

        public bool AcceptGi { get; set; }

        public int CareerId { get; set; }

        [ForeignKey("CareerId")]
        public Career Career { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
