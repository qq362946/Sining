// using System;
// using System.Collections.Concurrent;
// using System.Threading;
// using System.Threading.Tasks;
// using Sining.Tools;
//
// namespace Sining
// {
//     public class ActorTask<TComponent> where TComponent : Component
//     {
//         private Action<ActorTask<TComponent>, TComponent> _action;
//
//         private readonly TaskCompletionSource<bool> _tcs =
//             new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
//
//         private TComponent _component;
//
//         public void Initialization(Action<ActorTask<TComponent>, TComponent> action, TComponent component)
//         {
//             _component = component;
//
//             _action = action;
//         }
//
//         public Task Handler()
//         {
//             _action.Invoke(this, _component);
//
//             return _tcs.Task;
//         }
//
//         public void Return()
//         {
//             _tcs.SetResult(true);
//         }
//     }
//
//     public class Actor<TComponent> where TComponent : Component
//     {
//         private readonly ConcurrentQueue<ActorTask<TComponent>> _queue = new ConcurrentQueue<ActorTask<TComponent>>();
//
//         private readonly TComponent _component;
//
//         private int _lockId;
//
//         private Actor() { }
//
//         public Actor(TComponent component)
//         {
//             _component = component;
//         }
//
//         public void Post(Action<ActorTask<TComponent>, TComponent> action)
//         {
//             var actorTask = ObjectPool<ActorTask<TComponent>>.Rent();
//
//             actorTask.Initialization(action, _component);
//
//             _queue.Enqueue(actorTask);
//
//             if (Interlocked.CompareExchange(ref _lockId, 1, 0) == 0)
//             {
//                 Start().Coroutine();
//             }
//         }
//
//         private async SVoid Start()
//         {
//             while (true)
//             {
//                 if (_queue.TryDequeue(out var action))
//                 {
//                     try
//                     {
//                         await action.Handler();
//                         
//                         ObjectPool<ActorTask<TComponent>>.Return(action);
//                     }
//                     catch (Exception e)
//                     {
//                         Log.Error(e);
//                     }
//
//                     continue;
//                 }
//
//                 // 防止漏掉没有处理的任务
//
//                 Thread.SpinWait(100);
//
//                 if (_queue.Count == 0) break;
//             }
//
//             Interlocked.Exchange(ref _lockId, 0);
//
//             await STask.CompletedTask;
//         }
//     }
// }