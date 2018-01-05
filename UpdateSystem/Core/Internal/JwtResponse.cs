namespace CodeElements.UpdateSystem.Core.Internal
{
    public class JwtResponse<T>
    {
        public string Jwt { get; set; }
        public T Result { get; set; }
    }
}