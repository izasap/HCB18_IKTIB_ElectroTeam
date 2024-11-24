namespace HakatonIKTIB.Classes
{
    public class Answer
    {
        public Answer( ulong discussionId, ulong userId, string text, int likesCount, int dislikeCount, bool correct )
        {
            DiscussionId = discussionId;
            UserId = userId;
            Text = text;
            LikesCount = likesCount;
            DislikeCount = dislikeCount;
            Correct = correct;
        }

        public ulong DiscussionId { get; }
        public ulong UserId { get; }
        public string Text { get; }
        public int LikesCount { get; }
        public int DislikeCount { get; }
        public bool Correct { get; }
    }
}
