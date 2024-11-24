namespace HakatonIKTIB.Classes
{
    public class Discussion
    {
        public Discussion( ulong id, ulong applicantId, string topic, string text, bool isClosed, List<Tag> tags, List<Answer> answers )
        {
            Id = id;
            ApplicantId = applicantId;
            Topic = topic;
            Text = text;
            IsClosed = isClosed;
            Tags = tags;
            Answers = answers;
        }

        public ulong Id { get; }
        public ulong ApplicantId { get; }
        public string Topic { get; }
        public string Text { get; }
        public bool IsClosed { get; }
        public List<Tag>? Tags { get; }
        public List<Answer>? Answers { get; }
    }
}
