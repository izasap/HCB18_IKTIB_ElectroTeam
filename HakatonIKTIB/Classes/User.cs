namespace HakatonIKTIB.Classes
{
    public class User
    {
        public User( ulong id, string login, string pass, string name, string lastname, string surname, int course, string specialization, string univer, List<Tag> tags, List<Discussion> activeDiscussions, List<Discussion> closedDiscussions )
        {
            Id = id;
            Login = login;
            Password = pass;
            Name = name;
            LastName = lastname;
            SurName = surname;
            Course = course;
            Specialization = specialization;
            University = univer;
            Tags = tags;
            ActiveDiscussions = activeDiscussions;
            ClosedDiscussions = closedDiscussions;
        }

        public ulong Id { get; }
        public string Login { get; }
        public string Password { get; }
        public string Name { get; }
        public string LastName { get; }
        public string SurName { get; }
        public int Course { get; }
        public string Specialization { get; }
        public string University { get; }
        public List<Tag> Tags { get; }
        public List<Discussion> ActiveDiscussions { get; }
        public List<Discussion> ClosedDiscussions { get; }
    }
}
