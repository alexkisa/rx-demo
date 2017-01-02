namespace RxDemo.Customer
{
    internal class Entity
    {
        public Entity(string id, string name, string phone, string email)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Email = email;
        }

        public string Id { get; }
        public string Name { get; }
        public string Phone { get; }
        public string Email { get; }

        public static Entity FromDataEntity(DataEntity dataEntity)
        {
            return new Entity(
                dataEntity?.Id,
                dataEntity?.Name,
                dataEntity?.Phone,
                dataEntity?.Email);
        }

        public DataEntity ToDataEntity()
        {
            var dataEntity = new DataEntity();
            UpdateDataEntity(dataEntity);

            return dataEntity;
        }

        public void UpdateDataEntity(DataEntity dataEntity)
        {
            dataEntity.Id = Id;
            dataEntity.Name = Name;
            dataEntity.Phone = Phone;
            dataEntity.Email = Email;
        }
    }
}
