using System;

namespace RestfulASPNetCore.Web.Dtos
{
    public class Book : LinkedResourceBase
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid AuthorId { get; set; }

    }
}
