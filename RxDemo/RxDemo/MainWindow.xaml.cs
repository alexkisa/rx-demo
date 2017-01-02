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
