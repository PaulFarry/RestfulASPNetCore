using System;

namespace RestfulASPNetCore.Web.Dtos
{
    public class CreateAuthor
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }
    }
}
