namespace GITA.Models.DTOs
{
    public class TopicWithCommentsModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public string State { get; set; }
        public string Status { get; set; }
        public List<CommentModel> Comments { get; set; }
    }

    public class CommentModel
    {
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
    }
}
