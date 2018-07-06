using System.ComponentModel.DataAnnotations;

namespace RestfulASPNetCore.Web.Dtos
{
    public class CreateBook
    {
        [Required()]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

    }
}