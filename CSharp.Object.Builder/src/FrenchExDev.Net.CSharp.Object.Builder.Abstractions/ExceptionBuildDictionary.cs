

namespace FrenchExDev.Net.CSharp.Object.Builder.Abstractions;

/// <summary>
/// Represents a list of exceptions, providing functionality to store and manage multiple exceptions.
/// </summary>
/// <remarks>This class extends <see cref="List{T}"/> with the type parameter <see cref="Exception"/>,  allowing
/// it to be used as a collection specifically for exceptions. It can be useful in scenarios  where multiple exceptions
/// need to be aggregated, such as in batch processing or error handling workflows.</remarks>
public class ExceptionBuildDictionary : Dictionary<MemberName, List<Exception>>
{
    /// <summary>
    /// Adds an entry to the dictionary that associates the specified member name with the given exception to be thrown
    /// for invalid data.
    /// </summary>
    /// <param name="memberName">The name of the member to associate with the exception. Cannot be null.</param>
    /// <param name="invalidDataException">The exception to be thrown when the specified member contains invalid data. Cannot be null.</param>
    /// <returns>The current <see cref="ExceptionBuildDictionary"/> instance with the new association added.</returns>
    public ExceptionBuildDictionary Add(string memberName, Exception invalidDataException)
    {
        return Add(new MemberName(memberName), invalidDataException);
    }

    /// <summary>
    /// Adds the specified exceptions to the dictionary under the key representing the type of the provided object.
    /// </summary>
    /// <param name="memberName"></param>
    /// <param name="invalidDataException"></param>
    public ExceptionBuildDictionary Add(MemberName memberName, Exception invalidDataException)
    {
        var list = this.GetValueOrDefault(memberName);
        if (list is null)
        {
            list = new List<Exception>();
            this[memberName] = list;
        }
        list.Add(invalidDataException);
        return this;
    }

    /// <summary>
    /// Adds an entry for the specified member if it does not already exist in the dictionary.
    /// </summary>
    /// <remarks>If an entry for <paramref name="memberName"/> already exists, the method does not modify its
    /// value. The <paramref name="enumerable"/> parameter is ignored.</remarks>
    /// <param name="memberName">The member name to add or ensure exists in the dictionary.</param>
    /// <param name="enumerable">A collection of key-value pairs representing member names and their associated exception lists. This parameter
    /// is not used by the method.</param>
    /// <returns>The current instance of <see cref="ExceptionBuildDictionary"/>.</returns>
    public ExceptionBuildDictionary Add(MemberName memberName, IEnumerable<KeyValuePair<MemberName, List<Exception>>> enumerable)
    {
        var list = this.GetValueOrDefault(memberName);
        if (list is null)
        {
            list = new List<Exception>();
            this[memberName] = list;
        }
        return this;
    }
}