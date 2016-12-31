using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            customersAndUpdates
                .ObserveOnDispatcher()
                .Subscribe(customers => ItemsSource = customers);

            SelectedCustomer = Observable
                .FromEventPattern<SelectionChangedEventHandler, SelectionChangedEventArgs>(h => SelectionChanged += h, h => SelectionChanged -= h)
                .Select(e => e.EventArgs.AddedItems.OfType<Entity>().FirstOrDefault())
                .Publish()
                .RefCount();
        }

        public IObservable<Entity> SelectedCustomer { get; }

        private static FrameworkElementFactory TextBlockFactory(string valueName)
        {
            var factory = new FrameworkElementFactory(typeof(TextBlock));
            factory.SetValue(TextBlock.TextProperty, new Binding(valueName));

            return factory;
        }
    }
}
