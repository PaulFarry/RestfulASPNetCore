using System.ComponentModel.DataAnnotations;

namespace RestfulASPNetCore.Web.Dtos
{
    public class UpdateBook : ManipulateBook
    {
        [Required()]
        public override string Description
        {
            get => base.Description;
            set => base.Description = value;
        }

    }
}
