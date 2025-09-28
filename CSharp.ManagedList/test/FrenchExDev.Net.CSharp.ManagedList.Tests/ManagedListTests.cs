using FrenchExDev.Net.CSharp.ManagedList.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.ManagedList.Tests;

/// <summary>
/// Test suite validating that lifecycle hook callbacks (Add, Remove, Clear) are correctly invoked
/// for both <c>ClosedManagedList</c> (callbacks fixed at build time) and <c>OpenManagedList</c>
/// (callbacks can be appended after creation).
/// </summary>
/// <remarks>
/// Naming convention: Each test follows a BDD style pattern: <c>Given_[Context]_When_[Action]_It_Should_[Outcome]</c>.
/// Internally a lightweight <see cref="Test"/> helper captures whether specific hook callbacks were executed.
/// </remarks>
/// <example>
/// The helper <c>ManagedListTester.Closed&lt;string&gt;</c> wraps list creation and provides three delegates:
/// <code>
/// ManagedListTester.Closed<string>(
///     body: b => b.OnAdd(item => Console.WriteLine($"Adding {item}")),
///     act: list => list.Add("X"),
///     assert: list => {/* assertions */});
/// </code>
/// A similar pattern exists for open lists via <c>ManagedListTester.Open&lt;T&gt;</c>.
/// </example>
public class ManagedListTests
{
    /// <summary>
    /// Small probe object used to record whether any of the configured callbacks were triggered.
    /// Each hook simply flips a boolean flag so assertions can verify invocation.
    /// </summary>
    internal class Test
    {
        /// <summary>Stores value indicating visit on Clear.</summary>
        private bool _visitedClear = false;
        /// <summary>Stores value indicating visit on Remove.</summary>
        private bool _visitedRemove = false;
        /// <summary>Stores value indicating visit on Add.</summary>
        private bool _visitedAdd = false;
        /// <summary>Indicates at least one Clear callback ran.</summary>
        public bool VisitedClear => _visitedClear;
        /// <summary>Indicates at least one Remove callback ran.</summary>
        public bool VisitedRemove => _visitedRemove;
        /// <summary>Indicates at least one Add callback ran.</summary>
        public bool VisitedAdd => _visitedAdd;

        /// <summary>Clear hook (sets <see cref="VisitedClear"/> to true).</summary>
        public void OnClear(string item) => _visitedClear = true;
        /// <summary>Remove hook (sets <see cref="VisitedRemove"/> to true).</summary>
        public void OnRemove(string item) => _visitedRemove = true;
        /// <summary>Add hook (sets <see cref="VisitedAdd"/> to true).</summary>
        public void OnAdd(string item) => _visitedAdd = true;
    }

    /// <summary>
    /// Verifies that adding an element to a closed managed list triggers previously registered Add callbacks.
    /// </summary>
    /// <remarks>
    /// Given: A closed list with an Add hook.
    /// When: An item is added.
    /// Then: The Add hook is executed (flag becomes true).
    /// </remarks>
    [Fact]
    public void Given_ClosedManagedList_When_Adding_It_Should_Call_OnAdd_Hooks()
    {
        var test = new Test();

        ManagedListTester.Closed<string>(
            body: builder =>
            {
                builder.OnAdd(test.OnAdd);
            },
            act: (list) =>
            {
                list.Add("item1");
            },
            assert: (list) =>
            {
                test.VisitedAdd.ShouldBeTrue();
            });
    }

    /// <summary>
    /// Ensures that clearing a closed managed list invokes any registered Clear callbacks for existing items.
    /// </summary>
    /// <remarks>
    /// Given: A closed list with one Clear hook.
    /// When: An item is added then the list is cleared.
    /// Then: The Clear hook runs at least once.
    /// </remarks>
    [Fact]
    public void Given_ClosedManagedList_When_Clearing_It_Should_Call_OnClear_Hooks()
    {
        var test = new Test();

        ManagedListTester.Closed<string>(
            body: builder =>
            {
                builder.OnClear(test.OnClear);
            },
            act: (list) =>
            {
                list.Add("item1");
                list.Clear();
            },
            assert: (list) =>
            {
                test.VisitedClear.ShouldBeTrue();
            });
    }

    /// <summary>
    /// Confirms that removing an item from a closed managed list invokes the configured Remove callbacks.
    /// </summary>
    /// <remarks>
    /// Given: A closed list with a Remove hook.
    /// When: An item is added then removed.
    /// Then: The Remove hook is executed.
    /// </remarks>
    [Fact]
    public void Given_ClosedManagedList_When_Removing_It_Should_Call_OnRemove_Hooks()
    {
        var test = new Test();

        ManagedListTester.Closed<string>(
            body: builder =>
            {
                builder.OnRemove(test.OnRemove);
            },
            act: (list) =>
            {
                list.Add("item1");
                list.Remove("item1");
            },
            assert: (list) =>
            {
                test.VisitedRemove.ShouldBeTrue();
            });
    }

    /// <summary>
    /// Validates that Add callbacks registered after constructing an open list are still honored.
    /// </summary>
    /// <remarks>
    /// Given: An open list without initial callbacks.
    /// When: A runtime Add callback is attached and an item is added.
    /// Then: The callback runs.
    /// </remarks>
    [Fact]
    public void Given_OpenManagedList_When_Adding_It_Should_Call_OnAdd_Hooks()
    {
        var test = new Test();

        ManagedListTester.Open<string>(
            body: builder =>
            {
            },
            act: (list) =>
            {
                list.OnAdd(test.OnAdd);
                list.Add("item1");
            },
            assert: (list) =>
            {
                test.VisitedAdd.ShouldBeTrue();
            });
    }

    /// <summary>
    /// Ensures Clear callbacks registered directly on an open list operate when clearing its contents.
    /// </summary>
    /// <remarks>
    /// Given: An open list with no initial callbacks.
    /// When: A Clear hook is attached, an item is added, and the list is cleared.
    /// Then: The Clear hook runs.
    /// </remarks>
    [Fact]
    public void Given_OpenManagedList_When_Clearing_It_Should_Call_OnClear_Hooks()
    {
        var test = new Test();

        ManagedListTester.Open<string>(
            body: builder =>
            {
            },
            act: (list) =>
            {
                list.OnClear(test.OnClear);
                list.Add("item1");
                list.Clear();
            },
            assert: (list) =>
            {
                test.VisitedClear.ShouldBeTrue();
            });
    }

    /// <summary>
    /// Verifies that Remove callbacks added post-construction on an open list fire when removing an element.
    /// </summary>
    /// <remarks>
    /// Given: An open list with no registered callbacks.
    /// When: A Remove callback is appended, an item is added, then removed.
    /// Then: The Remove callback executes.
    /// </remarks>
    [Fact]
    public void Given_OpenManagedList_When_Removing_It_Should_Call_OnRemove_Hooks()
    {
        var test = new Test();

        ManagedListTester.Open<string>(
            body: builder =>
            {
            },
            act: (list) =>
            {
                list.OnRemove(test.OnRemove);
                list.Add("item1");
                list.Remove("item1");
            },
            assert: (list) =>
            {
                test.VisitedRemove.ShouldBeTrue();
            });
    }
}
