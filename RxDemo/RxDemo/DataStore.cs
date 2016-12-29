using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace RxDemo
{
    internal class DataStore
    {
        private readonly Repository _repository;
        private readonly IObservable<ImmutableList<Customer>> _customersAndUpdates;

        public DataStore(Repository repository)
        {
            _repository = repository;

            var customerUpdated = Observable.Create<Customer>(o =>
            {
                var subscription = _repository.Subscribe()
                    .Do<CustomerDataEntity>((id, entity) => o.OnNext(Customer.FromDataEntity(entity)));

                return Disposable.Create(() => _repository.Unsubscribe(subscription));
            });

            var initialCustomers = _repository
                .ReadAll<CustomerDataEntity>()
                .Select(Customer.FromDataEntity)
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

        public void SaveCustomer(string id, Customer customer)
        {
            _repository.Update<CustomerDataEntity>(id, customer.UpdateDataEntity);
        }

        public IObservable<Customer> GetCustomerAndUpdates(string id)
        {
            return _customersAndUpdates
                .Select(customers => customers.FirstOrDefault(c => c.Id == id))
                .DistinctUntilChanged();
        }

        public IObservable<ImmutableList<Customer>> GetCustomersAndUpdates() => _customersAndUpdates;
    }
}
