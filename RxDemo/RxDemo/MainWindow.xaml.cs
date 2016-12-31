using System;
using System.Reactive.Linq;
using RxDemo.Customer;

namespace RxDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var repository = new Repository();
            var dataStore = new DataStore(repository);

            var customerListView = new ListView(dataStore.GetCustomersAndUpdates());
            LeftColumn.Content = customerListView;

            var customerDetailView = new DetailView(customerListView.SelectedCustomer);
            RightColumn.Content = customerDetailView;

            SeedDataButton.Click += (_, __) =>
            {
                Observable.Zip(
                    Observable.Interval(TimeSpan.FromMilliseconds(200)),
                    new[]
                    {
                        new DataEntity {Id = "1", Name = "One", Email = "one@example.com"},
                        new DataEntity {Id = "2", Name = "Two", Email = "two@example.com"},
                        new DataEntity {Id = "3", Name = "Three", Email = "three@example.com"},
                        new DataEntity {Id = "4", Name = "Four", Email = "four@example.com"},
                        new DataEntity {Id = "5", Name = "Five", Email = "five@example.com"}
                    },
                    (interval, customer) => customer)
                    .Subscribe(customer => repository.Update<DataEntity>(customer.Id, c => Entity.FromDataEntity(customer).UpdateDataEntity(c)));
            };
        }
    }
}
