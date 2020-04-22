using System;
using System.Runtime.ExceptionServices;

namespace Sining
{
    public class STaskCompletionSource : IAwaiter, IDisposable
    {
        private Action _action;
        private ExceptionDispatchInfo _exception;
        private bool _faulted;

        public STask Task => new STask(this);

        public bool IsCompleted { get; private set; }

        public void SetResult()
        {
            IsCompleted = true;

            RunAction();
        }

        public void GetResult()
        {
            if (_faulted)
            {
                _exception?.Throw();
            }

            _faulted = false;
        }

        public void SetException(Exception exception)
        {
            _exception = ExceptionDispatchInfo.Capture(exception);

            _faulted = true;

            IsCompleted = true;

            RunAction();
        }

        private void RunAction()
        {
            if (!IsCompleted || _action == null) return;

            _action.Invoke();
            _action = null;
            IsCompleted = false;
        }

        public void OnCompleted(Action continuation)
        {
            _action = continuation;
            RunAction();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _action = continuation;
            RunAction();
        }

        public void Dispose()
        {
            _action = null;
            _faulted = false;
            IsCompleted = false;
        }
    }

    public class STaskCompletionSource<T> : IAwaiter<T>
    {
        private Action _action;
        private ExceptionDispatchInfo _exception;
        private bool _faulted;
        private T _result;
        
        public STask<T> Task => new STask<T>(this);

        public bool IsCompleted { get; private set; }

        public void SetResult(T result)
        {
            _result = result;
            IsCompleted = true;

            RunAction();
        }

        public T GetResult()
        {
            if (_faulted)
            {
                _exception?.Throw();
            }

            _faulted = false;

            return _result;
        }
        
        public void SetException(Exception exception)
        {
            _exception = ExceptionDispatchInfo.Capture(exception);

            _faulted = true;
            
            IsCompleted = true;
            
            RunAction();
        }

        private void RunAction()
        {
            if (!IsCompleted || _action == null) return;

            _action.Invoke();
            _action = null;
            IsCompleted = false;
        }

        public void OnCompleted(Action continuation)
        {
            _action = continuation;
            RunAction();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            _action = continuation;
            RunAction();
        }
    }
}