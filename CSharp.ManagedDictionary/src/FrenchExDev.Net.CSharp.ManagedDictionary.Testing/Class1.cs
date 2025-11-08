using System.Collections.Generic;

namespace FrenchExDev.Net.CSharp.ManagedDictionary.Testing
{
    // Helpers used by unit tests to record and validate events from ManagedDictionary
    public class Validator<TKey, TValue> where TKey : notnull
    {
        public record Event(string Type, TKey? Key, TValue? Value);

        public List<Event> Events { get; } = new();

        public void OnAdd(TKey key, TValue value) => Events.Add(new Event("Add", key, value));

        public void OnRemove(TKey key, TValue value) => Events.Add(new Event("Remove", key, value));

        public void OnClear() => Events.Add(new Event("Clear", default, default));
    }

    public static class ManagedDictionaryTester
    {
        /// <summary>
        /// Creates an OpenManagedDictionary with handlers wired to a fresh Validator.
        /// </summary>
        public static OpenManagedDictionary<TKey, TValue> CreateOpen<TKey, TValue>(out Validator<TKey, TValue> validator)
            where TKey : notnull
        {
            validator = new Validator<TKey, TValue>();
            var d = new OpenManagedDictionary<TKey, TValue>();
            d.OnAdd(validator.OnAdd);
            d.OnRemove(validator.OnRemove);
            d.OnClear(validator.OnClear);
            return d;
        }

        /// <summary>
        /// Creates an OpenManagedDictionary pre-populated with the provided initial values and handlers wired to a Validator.
        /// </summary>
        public static OpenManagedDictionary<TKey, TValue> CreateOpenWithInitial<TKey, TValue>(IDictionary<TKey, TValue> initial, out Validator<TKey, TValue> validator)
            where TKey : notnull
        {
            validator = new Validator<TKey, TValue>();

            var onAdd = new OnAddList<TKey, TValue>();
            var onRemove = new OnRemoveList<TKey, TValue>();
            var onClear = new OnClearList();

            onAdd.Add(validator.OnAdd);
            onRemove.Add(validator.OnRemove);
            onClear.Add(validator.OnClear);

            var basis = new Dictionary<TKey, TValue>(initial);

            return new OpenManagedDictionary<TKey, TValue>(onAdd, onRemove, onClear, basis);
        }
    }
}
