namespace HakatonIKTIB.Classes
{
    public class Tag
    {
        public Tag( ulong id, string name, string description )
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public ulong Id { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
