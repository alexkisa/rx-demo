using System;
using System.Reactive.Linq;
using System.Windows.Controls;

namespace RxDemo.Customer
{
    internal class DetailView : StackPanel
    {
        public DetailView(IObservable<Entity> customerAndUpdates)
        {
            var stackPanel = new StackPanel();
            Children.Add(stackPanel);

            var nameBlock = new TextBlock();
            stackPanel.Children.Add(nameBlock);

            var phoneBlock = new TextBlock();
            stackPanel.Children.Add(phoneBlock);

            var emailBlock = new TextBlock();
            stackPanel.Children.Add(emailBlock);

            customerAndUpdates
                .ObserveOnDispatcher()
                .Subscribe(customer =>
                {
                    nameBlock.Text = customer?.Name;
                    phoneBlock.Text = customer?.Phone;
                    emailBlock.Text = customer?.Email;
                });
        }
    }
}
