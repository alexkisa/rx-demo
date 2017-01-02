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
            customerListView.CustomerConfirmed.Subscribe(customer =>
            {
                var editWindow = new EditWindow(dataStore.GetCustomerAndUpdates(customer.Id));
                editWindow.CustomerSaved.Subscribe(c =>
                {
                    dataStore.SaveCustomer(c.Id, c);
                    editWindow.Close();
                });

                editWindow.Show();
            });
            LeftColumn.Content = customerListView;

            var selectedCustomerAndUpdates = customerListView.CustomerSelected
                .Where(customer => customer != null)
                .Select(customer => dataStore.GetCustomerAndUpdates(customer.Id)).Switch();

            var customerDetailView = new DetailView(selectedCustomerAndUpdates);
            RightColumn.Content = customerDetailView;
            
            SeedDataButton.Click += (_, __) =>
            {
                Observable.Zip(
                    Observable.Interval(TimeSpan.FromMilliseconds(200)),
                    new[]
                    {
                        new DataEntity {Id = "1", Name = "One", Phone = "111-1111", Email = "one@example.com"},
                        new DataEntity {Id = "2", Name = "Two", Phone = "222-222", Email = "two@example.com"},
                        new DataEntity {Id = "3", Name = "Three", Phone = "333-3333", Email = "three@example.com"},
                        new DataEntity {Id = "4", Name = "Four", Phone = "444-4444", Email = "four@example.com"},
                        new DataEntity {Id = "5", Name = "Five", Phone = "555-5555", Email = "five@example.com"}
                    },
                    (interval, customer) => customer)
                    .Subscribe(customer => repository.Update<DataEntity>(customer.Id, c => Entity.FromDataEntity(customer).UpdateDataEntity(c)));
            };
        }
    }
}
