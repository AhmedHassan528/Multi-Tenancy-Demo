namespace Multi_Tenancy_Demo.Contracts
{
    public interface IMustHaveTenant
    {
        public string TenantId { get; set; }
    }
}
