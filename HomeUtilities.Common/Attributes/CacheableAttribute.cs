namespace HomeUtilities.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CacheableAttribute : Attribute
    {
        public TimeSpan? AbsoluteExpiration { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }

        public CacheableAttribute(int absoluteExpirationMinutes = 60, int slidingExpirationMinutes = 30)
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(absoluteExpirationMinutes);
            SlidingExpiration = TimeSpan.FromMinutes(slidingExpirationMinutes);
        }

        public CacheableAttribute(TimeSpan absoluteExpiration)
        {
            AbsoluteExpiration = absoluteExpiration;
        }

        public CacheableAttribute() { }
    }   
}