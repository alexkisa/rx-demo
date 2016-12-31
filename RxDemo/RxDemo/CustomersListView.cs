using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace RxDemo
{
    internal class CustomersListView : ListView
    {
        public CustomersListView(IObservable<ImmutableList<Customer>> customersAndUpdates)
        {
            ItemTemplate = new DataTemplate(typeof(Customer))
            {
                VisualTree = TextBlockFactory(nameof(Customer.Name))
            };

            customersAndUpdates
                .ObserveOnDispatcher()
                .Subscribe(customers => ItemsSource = customers);

            SelectedCustomer = Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(h => SelectionChanged += h, h => SelectionChanged -= h)
                .Select(e => e.EventArgs.AddedItems.OfType<Customer>().FirstOrDefault())
                .Publish()
                .RefCount();
        }

        public IObservable<Customer> SelectedCustomer { get; }

        private static FrameworkElementFactory TextBlockFactory(string valueName)
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.TextProperty, new Binding(valueName));

            return factory;
        }
    }
}
