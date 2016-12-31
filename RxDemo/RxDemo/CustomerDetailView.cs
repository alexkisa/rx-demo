using System;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace RxDemo
{
    internal class CustomerDetailView : StackPanel
    {
        public CustomerDetailView(IObservable<Customer> customerAndUpdates)
        {
            var nameBlock = new TextBlock();
            var emailBlock = new TextBlock();

            var stackPanel = new StackPanel();
            stackPanel.Children.Add(nameBlock);
            stackPanel.Children.Add(emailBlock);
            Children.Add(stackPanel);

            customerAndUpdates
                .ObserveOnDispatcher()
                .Subscribe(customer =>
                {
                    nameBlock.Text = customer.Name;
                    emailBlock.Text = customer.Email;
                });
        }
    }
}
