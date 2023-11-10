using Microsoft.Extensions.Primitives;

namespace Otus.Caching
{
    public class RandomTimerChangeToken : IChangeToken
    {
        private bool _hasChanged;
        private Action _callback;
        private Timer _timer;

        public RandomTimerChangeToken() 
        {
            _timer = new Timer(OnTick, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }


        public bool HasChanged => _hasChanged;

        public bool ActiveChangeCallbacks => true;

        public IDisposable RegisterChangeCallback(Action<object?> callback, object? state)
        {
            _callback = () => callback(state);
            return NullDisposable.Instance;
        }

        private void OnTick(object? state) 
        {
            _hasChanged = true;
            _callback();
        }
    }

    public sealed class NullDisposable : IDisposable
    {
        public static readonly NullDisposable Instance = new NullDisposable();

        public void Dispose()
        {
        }
    }
}
