using System.Collections.Generic;
using BarbellTracker.ApplicationCode;
using Xunit;
using static BarbellTracker.ApplicationCode.IEventSystem;

namespace BarbellTracker.ApplicationCodeTests
{
    public class EventSystemTester
    {
        [Fact]
        public void Fire_WithoutSubscriber_ReturnsFalse()
        {
            var eventSystem = new EventSystem();

            Assert.False(eventSystem.Fire("nobody listens"));
        }

        [Fact]
        public void Fire_WithSubscriber_CallsHandlerWithArgument()
        {
            var eventSystem = new EventSystem();
            var received = new List<string>();
            EventDelegate<string> handler = received.Add;
            eventSystem.Subscribe(handler);

            var fired = eventSystem.Fire("hello");

            Assert.True(fired);
            Assert.Equal(new[] { "hello" }, received);
        }

        [Fact]
        public void Fire_HandlerUnsubscribesItself_DoesNotThrowAndIsNotCalledAgain()
        {
            var eventSystem = new EventSystem();
            var calls = 0;
            EventDelegate<string> handler = null;
            handler = _ =>
            {
                calls++;
                eventSystem.Unsubscribe(handler);
            };
            eventSystem.Subscribe(handler);

            eventSystem.Fire("first");
            eventSystem.Fire("second");

            Assert.Equal(1, calls);
        }
    }
}
