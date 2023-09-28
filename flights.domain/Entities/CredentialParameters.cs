
namespace flights.domain.Entities
{
    public class CredentialParameters
    {
        public int CredentialParametersId { get; set; }
        public string Parameter { get; set; }
        public string Value { get; set; }
        public virtual Credential Credential { get; set; }
    }
}
