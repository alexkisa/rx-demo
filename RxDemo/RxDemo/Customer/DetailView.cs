using System;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace RxDemo.Customer
{
    internal class DetailView : StackPanel
    {
        public DetailView(IObservable<Entity> customerAndUpdates)
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
