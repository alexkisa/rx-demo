namespace RxDemo
{
    internal class Customer
    {
        public Customer(string id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
        }

        public string Id { get; }
        public string Name { get; }
        public string Email { get; }

        public static Customer FromDataEntity(CustomerDataEntity dataEntity)
        {
            return new Customer(dataEntity?.Id, dataEntity?.Name, dataEntity?.Email);
        }

        public CustomerDataEntity ToDataEntity()
        {
            var dataEntity = new CustomerDataEntity();
            UpdateDataEntity(dataEntity);

            return dataEntity;
        }

        public void UpdateDataEntity(CustomerDataEntity dataEntity)
        {
            dataEntity.Id = Id;
            dataEntity.Name = Name;
            dataEntity.Email = Email;
        }
    }
}
