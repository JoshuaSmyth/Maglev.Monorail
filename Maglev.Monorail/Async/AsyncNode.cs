using System;
using System.Collections;
using System.Collections.Generic;

namespace Maglev.Monorail.Async
{
    public class AsyncNode
    {
        protected readonly Action Action;
        public AsyncStatus Status { get; set; }

        public List<AsyncNode> Children = new List<AsyncNode>();
   
        public AsyncNode()
        {
            
        }

        public AsyncNode(Action action)
        {
            this.Status = AsyncStatus.Queued;
            Action = action;
        }

        public void Start()
        {
            try
            {
                var a = new Action(Invoke);
                a.BeginInvoke(delegate
                    {
                        this.Status = AsyncStatus.Finished;
                    }, this);

            }
            catch (Exception exception)
            {
                // TODO Pass to Exception Handler if one is attached
                throw;
            }
        }

        private void Invoke()
        {
            try
            {
                Action.Invoke();
            }
            catch (Exception exception)
            {
                this.Status = AsyncStatus.ThrewException;
                var catchNode = this.FindCatch();
                if (catchNode != null)
                {
                    catchNode.Handle(exception);
                }
                else
                {
                    throw;   
                }
            }
        }

        private CatchNode FindCatch()
        {
            if (this is CatchNode)
            {
                return this as CatchNode;
            }

            foreach (var node in Children)
            {
                var isCatch = node.FindCatch();
                if (isCatch != null)
                    return isCatch;
            }

            return null;
        }
 
        public virtual void Update(float dt)
        {
            if (this.Status == AsyncStatus.Queued)
            {
                this.Status = AsyncStatus.Running;
                Start();
            }
        }

        public AsyncNode Wait(int ms)
        {
            var rv = new WaitNode(() => { }, ms);

            this.Children.Add(rv);
            return rv;
        }

        public AsyncNode Then(AsyncNode node)
        {
            this.Children.Add(node);
            return node;
        }

        public AsyncNode Then(Action action)
        {
            var rv = new AsyncNode(action);
            this.Children.Add(rv);
            return rv;
        }

        public AsyncNode Then(IEnumerator enumerator)
        {
            var rv = new CoroutineNode(enumerator);
            this.Children.Add(rv);
            return rv;
        }

        public AsyncNode Parallel(params Action[] actions)
        {
            throw new NotImplementedException();
        }

        public AsyncNode Sequence(params Action[] actions)
        {
            throw new NotImplementedException();
        }

        public AsyncNode If(Func<bool> predicate, Action action)
        {
            throw new NotImplementedException();
        }

        public AsyncNode IfElse(Func<bool> predicate, AsyncNode node, AsyncNode elseNode)
        {
            throw new NotImplementedException();
        }

        public AsyncNode IfElse(Func<bool> predicate, AsyncNode node, Action elseAction)
        {
            throw new NotImplementedException();
        }

        public AsyncNode IfElse(Func<bool> predicate, Action action, AsyncNode elseNode)
        {
            throw new NotImplementedException();
        }

        public AsyncNode IfElse(Func<bool> predicate, Action action, Action elseAction)
        {
            throw new NotImplementedException();
        }

        public AsyncNode Catch(Action<Exception> action)
        {
            var rv = new CatchNode(action);
            rv.Status = AsyncStatus.Ignore;
            this.Children.Add(rv);
            return rv;

        }

        public static AsyncNode Create(Action action)
        {

            var rv = new AsyncNode(action);
            rv.Status = AsyncStatus.Queued;
            return rv;
        }
    }
}
