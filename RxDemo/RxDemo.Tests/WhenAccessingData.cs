using System;
using System.Reactive.Linq;
using RxDemo.Customer;
using Xunit;

namespace RxDemo.Tests
{
    public class WhenAccessingData
    {
        [Fact]
        public async void ShouldReturnCustomer()
        {
            var repository = new Repository();
            repository.Update<DataEntity>("1", entity => entity.Id = "1");
            var store = new DataStore(repository);

            var customer = await store.GetCustomerAndUpdates("1").FirstAsync();

            Assert.NotNull(customer);
        }

        [Fact]
        public void ShouldReturnUpdatedCustomer()
        {
            var repository = new Repository();
            repository.Update<DataEntity>("1", entity => entity.Id = "1");
            var store = new DataStore(repository);

            var received = 0;
            store.GetCustomersAndUpdates().Subscribe(c => received++);
            repository.Update<DataEntity>("1", entity => entity.Id = "1");

            Assert.Equal(2, received);
        }

        [Fact]
        public async void ShouldReturnCustomersList()
        {
            var repository = new Repository();
            repository.Update<DataEntity>("1", entity => entity.Id = "1");
            var store = new DataStore(repository);

            var customers = await store.GetCustomersAndUpdates().FirstAsync();

            Assert.Equal(1, customers.Count);
        }

        [Fact]
        public void ShouldPublishUpdatedList()
        {
            var repository = new Repository();
            var store = new DataStore(repository);

            var received = 0;
            store.GetCustomersAndUpdates().Subscribe(c => received++);
            repository.Update<DataEntity>("1", entity => entity.Id = "1");

            Assert.Equal(2, received);
        }
    }
}
