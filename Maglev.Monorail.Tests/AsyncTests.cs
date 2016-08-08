using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Maglev.Monorail.Async;
using Maglev.Monorail.Tests.Mocks;
using NUnit.Framework;

namespace Maglev.Monorail.Tests
{
    [TestFixture]
    public class AsyncTests
    {
        private static void PumpQueue(AsyncOps actionOps)
        {
            while (actionOps.HasActions())
            {
                //Thread.Sleep(16);
                actionOps.Update(16);
            }
        }

        [Test]
        public void ShouldPass()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Test001()
        {
            var actionQueue = new AsyncOps();

            actionQueue.Queue(() => Console.WriteLine("Hello"))
                       .Wait(1000)
                       .Then(() => Console.WriteLine("World"));

            PumpQueue(actionQueue);
        }

        

        [Test]
        public void Test001b()
        {
            var actionQueue = new AsyncOps();

            actionQueue.Queue(() => Console.WriteLine("Hello"))
                       .Wait(1000)
                       .Then(actionQueue.Queue(() => Console.WriteLine("World")).Then(() => Console.WriteLine(" YAY!")));

            PumpQueue(actionQueue);
        }



        [Test]
        public void Test001c()
        {
            var actionQueue = new AsyncOps();

            actionQueue.Queue(AsyncNode.Create(() => Console.WriteLine("Hello")))
                       .Wait(1000)
                       .Then(() => Console.WriteLine("World"))
                       .WriteLine(" Yay!");

            PumpQueue(actionQueue);
        }


        [Test]
        public void Test002_Exception()
        {
            var actionQueue = new AsyncOps();

            actionQueue.Queue(() => { throw new Exception("Test");})
                       .Catch((e) =>
                           {
                               Console.WriteLine("Caught Exception");
                               Console.WriteLine(e.Message);
                               Console.WriteLine(e.StackTrace);
                           });

            PumpQueue(actionQueue);
        }

        [Test]
        public void Test002b_Exception()
        {
            var actionQueue = new AsyncOps();

            actionQueue.Queue(() => { /*throw new Exception("Test");*/ })
                       .Catch((e) =>
                       {
                           Console.WriteLine("Caught Exception");
                           Console.WriteLine(e);
                       });

            PumpQueue(actionQueue);
        }

        [Test]
        public void Test003()
        {
            var actionQueue = new AsyncOps();

            var i = 0;

            actionQueue.Queue(() => Console.WriteLine("Hello"))
                       .Wait(1000)
                       .Then(() => 
                            { 
                                Console.WriteLine("World");
                                i = 2;
                            });
                       /*
                       .Parallel(() => Console.WriteLine("A"), () => Console.WriteLine("B"))
                       .Sequence(() => i++, () => i++)
                       .If(() => i == 2, () => Console.WriteLine("YAY"))
                       .IfElse(() => i == 2, AsyncNode.Create(() => Console.WriteLine("YAY")).Then(() => Console.WriteLine("A+")), 
                                             () => Console.WriteLine("BOO"));
                       */
            PumpQueue(actionQueue);

            Assert.IsTrue(i==2);
        }
 
        [Test]
        [Ignore("Not implemented")]
        public void Test_Cancel()
        {
            var actionQueue = new AsyncOps();
            var node = actionQueue.Queue(Poem.ReadPoem(Poem.poem));
            
            PumpQueue(actionQueue);
            actionQueue.Cancel(node);
        }
    }
}
