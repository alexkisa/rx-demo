using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxDemo.Customer
{
    internal class DataStore
    {
        private readonly Repository _repository;
        private readonly IObservable<ImmutableList<Entity>> _customersAndUpdates;

        public DataStore(Repository repository)
        {
            _repository = repository;

            var customerUpdated = Observable.Create<Entity>(o =>
            {
                var subscription = _repository.Subscribe()
                    .Do<DataEntity>((id, entity) => o.OnNext(Entity.FromDataEntity(entity)));

                return Disposable.Create(() => _repository.Unsubscribe(subscription));
            });

            var initialCustomers = _repository
                .ReadAll<DataEntity>()
                .Select(Entity.FromDataEntity)
                .ToImmutableList();

            _customersAndUpdates = customerUpdated
                .Scan(initialCustomers, (customers, customer) =>
                {
                    var existingCustomer = customers.FirstOrDefault(c => c.Id == customer.Id);

                    return existingCustomer != null
                        ? customers.Replace(existingCustomer, customer)
                        : customers.Add(customer);
                })
                .StartWith(initialCustomers)
                .Replay(1)
                .RefCount();
        }

        public void SaveCustomer(string id, Entity entity)
        {
            _repository.Update<DataEntity>(id, entity.UpdateDataEntity);
        }

        public IObservable<Entity> GetCustomerAndUpdates(string id)
        {
            return _customersAndUpdates
                .Select(customers => customers.FirstOrDefault(c => c.Id == id))
                .DistinctUntilChanged();
        }

        public IObservable<ImmutableList<Entity>> GetCustomersAndUpdates() => _customersAndUpdates;
    }
}
