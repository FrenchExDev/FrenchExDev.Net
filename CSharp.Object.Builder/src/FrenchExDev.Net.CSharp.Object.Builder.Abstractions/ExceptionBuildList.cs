namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a list of exceptions, providing functionality to store and manage multiple exceptions.
/// </summary>
/// <remarks>This class extends <see cref="List{T}"/> with the type parameter <see cref="Exception"/>,  allowing
/// it to be used as a collection specifically for exceptions. It can be useful in scenarios  where multiple exceptions
/// need to be aggregated, such as in batch processing or error handling workflows.</remarks>
public class ExceptionBuildList : List<Exception>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionBuildList"/> class.
    /// </summary>
    public ExceptionBuildList() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionBuildList"/> class with the specified collection of
    /// exceptions.
    /// </summary>
    /// <param name="exceptions">The collection of exceptions to initialize the list with. Cannot be null.</param>
    public ExceptionBuildList(IEnumerable<Exception> exceptions) : base(exceptions) { }

    /// <summary>
    /// Creates a new instance of the <see cref="ExceptionBuildList"/> class initialized with the specified exceptions.
    /// </summary>
    /// <param name="exceptions">An array of <see cref="Exception"/> objects to include in the list. This parameter can be empty.</param>
    /// <returns>A new <see cref="ExceptionBuildList"/> containing the provided exceptions.</returns>
    public static ExceptionBuildList New(object @this, params Exception[] exceptions) => new ExceptionBuildList(exceptions);
}