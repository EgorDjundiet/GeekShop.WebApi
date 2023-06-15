namespace GeekShop.Domain.Exceptions
{
    public class AlreadyUsedException : Exception
    {
        public AlreadyUsedException(string message) : base(message)
        {
            
        }
    }
}
