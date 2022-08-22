namespace CusomerManagement.Infra.Persistance.Models
{
    public class Customer
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public bool IsActive { get; set; }
    }
}
