namespace GITA.Models.DTOs
{
    public class TopicInfoModel
    {
        public string Title { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
    }
}
