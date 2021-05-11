namespace Core.Data.Models
{
    public class Choice
    {
        public long QuestionId { get; set; }
        public string Text { get; set; }
        public int Votes { get; set; }
        
        public Question Question { get; set; }
    }
}