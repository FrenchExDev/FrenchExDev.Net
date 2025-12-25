using Shouldly;
using FrenchExDev.Net.CSharp.Object.Result2;

namespace FrenchExDev.Net.CSharp.Object.Builder2.Tests;

public class SynchronizationStrategyTests
{
    [Fact]
    public void LockStrategy_Execute_Action_ShouldWork()
    {
        var executed = false;
        var lockObj = new object();

        LockSynchronizationStrategy.Instance.Execute(lockObj, () => executed = true);

        executed.ShouldBeTrue();
    }

    [Fact]
    public void LockStrategy_Execute_Func_ShouldReturnResult()
    {
        var lockObj = new object();

        var result = LockSynchronizationStrategy.Instance.Execute(lockObj, () => 42);

        result.ShouldBe(42);
    }

    [Fact]
    public void NoSyncStrategy_Execute_Action_ShouldWork()
    {
        var executed = false;
        var lockObj = new object();

        NoSynchronizationStrategy.Instance.Execute(lockObj, () => executed = true);

        executed.ShouldBeTrue();
    }

    [Fact]
    public void NoSyncStrategy_Execute_Func_ShouldReturnResult()
    {
        var lockObj = new object();

        var result = NoSynchronizationStrategy.Instance.Execute(lockObj, () => 42);

        result.ShouldBe(42);
    }

    [Fact]
    public void NoSyncStrategy_Instance_ShouldBeSingleton()
    {
        var a = NoSynchronizationStrategy.Instance;
        var b = NoSynchronizationStrategy.Instance;

        a.ShouldBeSameAs(b);
    }

    [Fact]
    public void ReaderWriterStrategy_ExecuteRead_Action_ShouldWork()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var executed = false;

        strategy.ExecuteRead(() => { executed = true; return 0; });

        executed.ShouldBeTrue();
    }

    [Fact]
    public void ReaderWriterStrategy_ConcurrentReads_ShouldAllowMultipleReaders()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var readCount = 0;
        var barrier = new Barrier(3);

        var tasks = Enumerable.Range(0, 3).Select(_ => Task.Run(() =>
        {
            strategy.ExecuteRead(() =>
            {
                Interlocked.Increment(ref readCount);
                barrier.SignalAndWait(TimeSpan.FromSeconds(1));
                return readCount;
            });
        })).ToArray();

        Task.WaitAll(tasks);
        readCount.ShouldBe(3);
    }

    [Fact]
    public void ReaderWriterStrategy_WriteBlocksReads()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var writeCompleted = false;

        strategy.Execute(new object(), () =>
        {
            Thread.Sleep(10);
            writeCompleted = true;
        });

        writeCompleted.ShouldBeTrue();
    }

    [Fact]
    public void ReaderWriterStrategy_NestedWriteLocks_ShouldWork()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var innerExecuted = false;

        strategy.Execute(new object(), () =>
        {
            strategy.Execute(new object(), () =>
            {
                innerExecuted = true;
            });
        });

        innerExecuted.ShouldBeTrue();
    }

    [Fact]
    public void ReaderWriterStrategy_NestedReadLocks_ShouldWork()
    {
        var strategy = new ReaderWriterSynchronizationStrategy();
        var innerResult = 0;

        var result = strategy.ExecuteRead(() =>
        {
            innerResult = strategy.ExecuteRead(() => 42);
            return innerResult;
        });

        result.ShouldBe(42);
        innerResult.ShouldBe(42);
    }

    [Fact]
    public void LockStrategy_Instance_ShouldBeSingleton()
    {
        var a = LockSynchronizationStrategy.Instance;
        var b = LockSynchronizationStrategy.Instance;

        a.ShouldBeSameAs(b);
    }
}

