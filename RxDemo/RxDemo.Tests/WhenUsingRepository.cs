using Xunit;

namespace RxDemo.Tests
{
    public class WhenUsingRepository
    {
        [Fact]
        public void ShouldReadNullWhenEmpty()
        {
            var repository = new Repository();
            Assert.Null(repository.Read<Entity>(""));
        }

        [Fact]
        public void ShouldUpdateNewEntityWhenEmpty()
        {
            var repository = new Repository();
            repository.Update<Entity>("", Assert.NotNull);
        }

        [Fact]
        public void ShouldReadUpdatedEntity()
        {
            var repository = new Repository();
            repository.Update<Entity>("id", entity => entity.Property = "value");
            Assert.Equal("value", repository.Read<Entity>("id").Property);
        }

        [Fact]
        public void ShouldNotifyOnSubscription()
        {
            var repository = new Repository();

            var notified = false;
            repository.Subscribe()
                .Do<Entity>((_, __) => notified = true);

            repository.Update<Entity>("", entity => {});

            Assert.True(notified);
        }

        [Fact]
        public void ShouldFilterSubscriptionOnPredicate()
        {
            var repository = new Repository();

            var notified = false;
            repository.Subscribe()
                .When<Entity>((_, __) => false)
                .Do<Entity>((_, __) => notified = true);

            repository.Update<Entity>("", entity => { });

            Assert.False(notified);
        }

        [Fact]
        public void ShouldUnsubscribe()
        {
            var repository = new Repository();

            var notified = false;
            var subscription = repository.Subscribe()
                .Do<Entity>((_, __) => notified = true);

            repository.Unsubscribe(subscription);

            repository.Update<Entity>("", entity => { });

            Assert.False(notified);
        }

        public class Entity
        {
            public string Property { get; set; }
        }
    }
}
