using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RxDemo.Customer
{
    internal class ListView : System.Windows.Controls.ListView
    {
        public ListView(IObservable<ImmutableList<Entity>> customersAndUpdates)
        {
            ItemTemplate = new DataTemplate(typeof(Entity))
            {
                VisualTree = TextBlockFactory(nameof(Entity.Name))
            };

            CustomerSelected = Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(h => SelectionChanged += h, h => SelectionChanged -= h)
                .Select(e => e.EventArgs.AddedItems.OfType<Entity>().FirstOrDefault())
                .Publish()
                .RefCount();

            var doubleClicked = Observable.FromEventPattern<MouseButtonEventHandler, MouseButtonEventArgs>(h => MouseDoubleClick += h, h => MouseDoubleClick -= h);

            CustomerConfirmed = doubleClicked
                .WithLatestFrom(CustomerSelected, (_, customer) => customer)
                .Publish()
                .RefCount();

            customersAndUpdates
                .ObserveOnDispatcher()
                .WithLatestFrom(CustomerSelected.StartWith((Entity) null), (customers, selected) => new { customers, selected })
                .Subscribe(a =>
                {
                    ItemsSource = a.customers;
                    SelectedItem = a.customers.FirstOrDefault(c => c.Id == a.selected?.Id);
                });
        }

        public IObservable<Entity> CustomerSelected { get; }
        public IObservable<Entity> CustomerConfirmed { get; }

        private static FrameworkElementFactory TextBlockFactory(string valueName)
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.TextProperty, new Binding(valueName));

            return factory;
        }
    }
}
