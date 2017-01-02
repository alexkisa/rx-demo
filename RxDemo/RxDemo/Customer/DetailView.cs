using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace RxDemo.Customer
{
    internal class DetailView : StackPanel
    {
        public DetailView(IObservable<Entity> customerAndUpdates)
        {
            Margin = new Thickness(10);

            var uiCustomerAndUpdates = customerAndUpdates.ObserveOnDispatcher();

            AddTextBlock("Name", uiCustomerAndUpdates.Select(c => c.Name));
            AddTextBlock("Phone", uiCustomerAndUpdates.Select(c => c.Phone));
            AddTextBlock("Email", uiCustomerAndUpdates.Select(c => c.Email));
        }

        private void AddTextBlock(string label, IObservable<string> value)
        {
            var block = new TextBlock();
            value.Subscribe(v => block.Text = v);

            Children.Add(block);
        }
    }
}
