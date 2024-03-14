﻿using System.ComponentModel.DataAnnotations;

namespace PustokMVC.Models
{
    public class Genre : BaseEntity
    {
        [StringLength(50)]
        public string Name { get; set; }

        public List<Book>? Books { get; set; }
    }
}
