namespace Lab5.Models.ViewModels
{
    public class CustomerSubscriptionsViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
