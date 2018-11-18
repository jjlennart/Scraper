using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public Round Round { get; set; }
        public Game Game { get; set; }
    }
}
