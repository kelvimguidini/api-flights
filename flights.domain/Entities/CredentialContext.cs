
namespace flights.domain.Entities
{
    public class CredentialContext
    {
        public int CredentialContextId { get; set; }
        public virtual Credential Credential{ get; set; }
        public virtual Application Application { get; set; }
        public virtual Provider Provider { get; set; }
    }
}