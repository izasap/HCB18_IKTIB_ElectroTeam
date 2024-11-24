using HakatonIKTIB.Classes;
using System.ComponentModel.DataAnnotations;

namespace HakatonIKTIB.Models
{
    public class UserModel
    {
        public UserModel()
        {
            user = null;
        }

        public UserModel(User user)
        {
            this.user = user;
        }

        public UserModel(Discussion discussion)
        {
            this.discussion = discussion;
            user = null;
        }

        public UserModel(Discussion discussion, User user)
        {
            this.discussion = discussion;
            this.user = user;
        }

        public Discussion? discussion { get; }
        public User? user { get; }

        [Required]
        public string ApplicantId { get; set; }
        [Required]
        public string Topic { get; set; }
        [Required]
        public string Text { get; set; }

        [Required]
        public string AnswerDiscussionId { get; set; }
        [Required]
        public string AnswerUserId { get; set; }
        [Required]
        public string AnswerText { get; set; }
    }
}
