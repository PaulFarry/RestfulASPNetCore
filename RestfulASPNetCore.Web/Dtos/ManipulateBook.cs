using System.ComponentModel.DataAnnotations;

namespace RestfulASPNetCore.Web.Dtos
{
    public abstract class ManipulateBook
    {
        [Required()]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public virtual string Description { get; set; }

    }
}
