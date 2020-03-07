namespace ContosoUniversity.Shared.Infrastructure
{
    public interface IPerson
    {
        string LastName { get; set; }
        string FirstMidName { get; set; }
    }

    public static class IPersonExtensions
    {
        public static string FullName(this IPerson person) => person.LastName + ", " + person.FirstMidName;
    }
}
