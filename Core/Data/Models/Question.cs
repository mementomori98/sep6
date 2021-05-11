using System.Collections.Generic;

namespace Core.Data.Models
{
    public class Question
    {
        public long Id { get; set; }
        public string Text { get; set; }
        
        public List<Choice> Choices { get; set; }
    }
}