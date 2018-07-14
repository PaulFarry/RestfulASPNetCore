﻿using System;
using System.Collections.Generic;

namespace RestfulASPNetCore.Web.Dtos
{
    public class CreateDeadAuthor
    {
        public CreateDeadAuthor()
        {
            Books = new List<CreateBook>();
        }
        public string Firstname { get; set; }
        public string Lastname { get; set; }

        public DateTimeOffset DateOfBirth { get; set; }
        public DateTimeOffset? DateOfDeath { get; set; }
        public string Genre { get; set; }

        public ICollection<CreateBook> Books { get; set; }
    }
}
