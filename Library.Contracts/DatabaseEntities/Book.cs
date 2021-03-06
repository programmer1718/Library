using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Contracts.DatabaseEntities
{
    [Table("Book")]
    public class Book
    {
        public Guid Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public ICollection<Author> Authors { get; set; }
        public string ISBN { get; set; }
        public int PublicationYear { get; set; }
        public Publisher Publisher { get; set; }
        public Location Location { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}