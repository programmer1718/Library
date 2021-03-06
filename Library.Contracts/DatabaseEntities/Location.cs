using System;
using System.ComponentModel.DataAnnotations.Schema;
using Library.Contracts.Common;

namespace Library.Contracts.DatabaseEntities
{
    [Table("Location")]
    public class Location
    {
        public Guid Id { get; set; }
        public LocationType Type { get; set; }
        public string Value { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }
    }
}