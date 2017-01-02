using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RxDemo.Customer
{
    internal class EditWindow : Window
    {
        public EditWindow(IObservable<Entity> customerAndUpdates)
        {
            SizeToContent = SizeToContent.WidthAndHeight;

            var panel = new StackPanel();
            Content = panel;

            var nameBox = Row("Name", customerAndUpdates.Select(c => c.Name));
            panel.Children.Add(nameBox.Item1);

            var phoneBox = Row("Phone", customerAndUpdates.Select(c => c.Phone));
            panel.Children.Add(phoneBox.Item1);

            var emailBox = Row("Email", customerAndUpdates.Select(c => c.Email));
            panel.Children.Add(emailBox.Item1);
            
            var okButton = new Button {Content = "OK", HorizontalAlignment = HorizontalAlignment.Right, IsDefault = true};
            panel.Children.Add(okButton);

            var okClicked = Observable
                .FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => okButton.Click += h, h => okButton.Click -= h)
                .Select(e => true);

            var customerChanged = Observable.CombineLatest(
                customerAndUpdates.Select(c => c.Id),
                nameBox.Item2,
                phoneBox.Item2,
                emailBox.Item2,
                (id, name, phone, email) => new Entity(id, name, phone, email));

            CustomerSaved = okClicked
                .WithLatestFrom(customerChanged, (_, customer) => customer);
        }

        public IObservable<Entity> CustomerSaved { get; }

        private static Tuple<StackPanel, IObservable<string>> Row(string label, IObservable<string> valueUpdated)
        {
            var panel = new StackPanel {Orientation = Orientation.Horizontal};

            panel.Children.Add(new TextBlock {Text = label, Width = 80});

            var valueBox = new TextBox {Width = 120};
            panel.Children.Add(valueBox);

            var valueChanged = Observable
                .FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(h => valueBox.TextChanged += h, h => valueBox.TextChanged -= h)
                .Select(_ => valueBox.Text)
                .Merge(valueUpdated);

            valueUpdated.Subscribe(value => valueBox.Text = value);

            return Tuple.Create(panel, valueChanged);
        }
    }
}
