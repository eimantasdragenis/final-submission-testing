namespace PSEngagementSystem
{
    public class CheckIn
    {
        public int ID { get; set; }
        public int StudentID { get; set; }  // assume 1 for now
        public int Mood { get; set; }       // 1-5
        public DateTime Date { get; set; }
        public string Comment { get; set; } = "";  // enter optional Comment.
    }
}
