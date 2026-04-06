namespace backend_marsh_project.Exceptions
{
    public class BadRequestException: Exception
    {
        public BadRequestException(string message) : base(message) { }
    }
}
