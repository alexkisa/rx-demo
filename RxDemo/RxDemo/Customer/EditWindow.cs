using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RxDemo.Customer
{
    internal class EditWindow : Window
    {
        public EditWindow(IObservable<Entity> customerAndUpdates, Window owner = null)
        {
            Owner = owner;
            ResizeMode = ResizeMode.NoResize;
            SizeToContent = SizeToContent.WidthAndHeight;
            Title = "Edit";
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new StackPanel {Margin = new Thickness(10)};
            Content = panel;

            var customerChanged = Observable.CombineLatest(
                customerAndUpdates.Select(c => c.Id),
                AddRow(panel, "Name", customerAndUpdates.Select(c => c.Name)),
                AddRow(panel, "Phone", customerAndUpdates.Select(c => c.Phone)),
                AddRow(panel, "Email", customerAndUpdates.Select(c => c.Email)),
                (id, name, phone, email) => new Entity(id, name, phone, email));

            var okButton = new Button { Content = "OK", HorizontalAlignment = HorizontalAlignment.Right, IsDefault = true, Padding = new Thickness(5, 0, 5, 0)};
            panel.Children.Add(okButton);

            CustomerSaved = Observable
                .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => okButton.Click += h, h => okButton.Click -= h)
                .WithLatestFrom(customerChanged, (_, customer) => customer);
        }

        public IObservable<Entity> CustomerSaved { get; }

        private static IObservable<string> AddRow(Panel panel, string label, IObservable<string> valueUpdated)
        {
            var horizontalPanel = new StackPanel {Margin = new Thickness(0, 0, 0, 5), Orientation = Orientation.Horizontal};

            horizontalPanel.Children.Add(new TextBlock {Text = label, Width = 80});

            var valueBox = new TextBox {Width = 120};
            horizontalPanel.Children.Add(valueBox);

            var valueChanged = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(h => valueBox.TextChanged += h, h => valueBox.TextChanged -= h)
                .Select(_ => valueBox.Text)
                .Merge(valueUpdated);

            valueUpdated.Subscribe(value => valueBox.Text = value);

            panel.Children.Add(horizontalPanel);

            return valueChanged;
        }
    }
}
