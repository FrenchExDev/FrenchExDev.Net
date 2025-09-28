namespace FrenchExDev.Net.CSharp.ManagedList.Testing;

/// <summary>
/// Provides utility methods for constructing and validating managed lists in testing scenarios using builder patterns.
/// </summary>
/// <remarks>The methods in this class facilitate the creation, modification, and assertion of managed lists by
/// orchestrating builder and validation actions. These utilities are intended for use in test code to streamline
/// workflows involving managed list lifecycles. This class cannot be instantiated.</remarks>
public static class ManagedListTester
{
    /// <summary>
    /// Executes the specified actions to build and validate a closed managed list using a builder pattern.
    /// </summary>
    /// <remarks>This method is typically used in testing scenarios to construct a managed list, perform
    /// operations on it, and then verify its final state. The builder is only valid within the scope of the <paramref
    /// name="body"/> action; after closing, further modifications are not allowed.</remarks>
    /// <typeparam name="T">The type of elements contained in the managed list.</typeparam>
    /// <param name="body">An action that receives a <see cref="ManagedListBuilder{T}"/> to configure and populate the list before it is
    /// closed.</param>
    /// <param name="assert">An action that receives the resulting <see cref="ClosedManagedList{T}"/> for validation or assertions after the
    /// list has been closed.</param>
    public static void Closed<T>(Action<ManagedListBuilder<T>> body, Action<ClosedManagedList<T>> act, Action<ClosedManagedList<T>> assert)
    {
        var builder = new ManagedListBuilder<T>();
        body(builder);
        var list = builder.Closed();
        act(list);
        assert(list);
    }

    /// <summary>
    /// Executes a sequence of actions to build, append to, and assert an open managed list of elements of type T.
    /// </summary>
    /// <remarks>This method provides a structured workflow for constructing and validating a managed list.
    /// The actions are executed in order: first the builder is configured, then the list is opened and appended to, and
    /// finally assertions are performed. All actions must be non-null.</remarks>
    /// <typeparam name="T">The type of elements contained in the managed list.</typeparam>
    /// <param name="body">An action that receives a builder for the managed list and is used to configure or populate the list before it
    /// is opened.</param>
    /// <param name="act">An action that receives the opened managed list and is used to append additional elements or perform operations
    /// on the list.</param>
    /// <param name="assert">An action that receives the opened managed list and is used to perform assertions or validations on the list.</param>
    public static void Open<T>(Action<ManagedListBuilder<T>> body, Action<OpenManagedList<T>> act, Action<OpenManagedList<T>> assert)
    {
        var builder = new ManagedListBuilder<T>();
        body(builder);
        var list = builder.Open();
        act(list);
        assert(list);
    }
}
