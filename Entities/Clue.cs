using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class Clue
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public bool IsDailyDouble { get; set; }
        public string ClueText { get; set; }
        public string Response { get; set; }        
        public Category Category { get; set; }       
    }
}
