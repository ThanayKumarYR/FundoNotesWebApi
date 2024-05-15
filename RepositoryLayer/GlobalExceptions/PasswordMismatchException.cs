namespace Repository.GlobalExceptions
{
    public class PasswordMismatchException : Exception
    {
        public PasswordMismatchException(string message) : base(message) { }
    }
}