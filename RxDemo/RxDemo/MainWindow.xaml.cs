using System;
using System.Reactive.Linq;

namespace RxDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var repository = new Repository();
            var dataStore = new DataStore(repository);

            var customerListView = new CustomersListView(dataStore.GetCustomersAndUpdates());
            LeftColumn.Content = customerListView;

            var customerDetailView = new CustomerDetailView(customerListView.SelectedCustomer);
            RightColumn.Content = customerDetailView;

            SeedDataButton.Click += (_, __) =>
            {
                Observable.Zip(
                    Observable.Interval(TimeSpan.FromMilliseconds(200)),
                    new[]
                    {
                        new CustomerDataEntity {Id = "1", Name = "One", Email = "one@example.com"},
                        new CustomerDataEntity {Id = "2", Name = "Two", Email = "two@example.com"},
                        new CustomerDataEntity {Id = "3", Name = "Three", Email = "three@example.com"},
                        new CustomerDataEntity {Id = "4", Name = "Four", Email = "four@example.com"},
                        new CustomerDataEntity {Id = "5", Name = "Five", Email = "five@example.com"}
                    },
                    (interval, customer) => customer)
                    .Subscribe(customer => repository.Update<CustomerDataEntity>(customer.Id, c => Customer.FromDataEntity(customer).UpdateDataEntity(c)));
            };
        }
    }
}
