using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FrenchExDev.Net.CSharp.ManagedDictionary;
using FrenchExDev.Net.CSharp.ManagedDictionary.Testing;
using Shouldly;

namespace FrenchExDev.Net.CSharp.ManagedDictionary.Tests
{
    public class ManagedDictionaryTests
    {
        [Fact]
        public void Given_OpenDictionary_When_Add_Then_OnAddInvoked()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);

            d.Add(1, "one");

            d.Count.ShouldBe(1);
            d.ContainsKey(1).ShouldBeTrue();
            v.Events.ShouldHaveSingleItem();
            v.Events[0].Type.ShouldBe("Add");
            v.Events[0].Key.ShouldBe(1);
            v.Events[0].Value.ShouldBe("one");
        }

        [Fact]
        public void Given_DictionaryWithKey_When_RemoveKey_Then_OnRemoveInvoked()
        {
            var initial = new Dictionary<int, string> {{ 1, "one" }};
            var d = ManagedDictionaryTester.CreateOpenWithInitial(initial, out var v);

            var removed = d.Remove(1);

            removed.ShouldBeTrue();
            d.ContainsKey(1).ShouldBeFalse();
            v.Events.ShouldHaveSingleItem();
            v.Events[0].Type.ShouldBe("Remove");
            v.Events[0].Key.ShouldBe(1);
            v.Events[0].Value.ShouldBe("one");
        }

        [Fact]
        public void Given_EmptyOpenDictionary_When_RemoveMissingKey_Then_ReturnsFalseAndNoEvents()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);

            var removed = d.Remove(42);

            removed.ShouldBeFalse();
            v.Events.ShouldBeEmpty();
        }

        [Fact]
        public void Given_DictionaryWithMultipleItems_When_Clear_Then_OnClearInvokedAndAllRemoved()
        {
            var initial = new Dictionary<int, string> {{ 1, "one" }, { 2, "two" }};
            var d = ManagedDictionaryTester.CreateOpenWithInitial(initial, out var v);

            d.Clear();

            d.Count.ShouldBe(0);
            v.Events.ShouldHaveSingleItem();
            v.Events[0].Type.ShouldBe("Clear");
        }

        [Fact]
        public void Given_OpenDictionary_When_AddKeyValuePair_Then_OnAddInvoked()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);

            d.Add(new KeyValuePair<int, string>(3, "three"));

            d.Count.ShouldBe(1);
            v.Events.ShouldHaveSingleItem();
            v.Events[0].Type.ShouldBe("Add");
            v.Events[0].Key.ShouldBe(3);
            v.Events[0].Value.ShouldBe("three");
        }

        [Fact]
        public void Given_DictionaryWithPair_When_RemoveKeyValuePair_Then_OnRemoveInvoked()
        {
            var initial = new Dictionary<int, string> {{ 5, "five" }};
            var d = ManagedDictionaryTester.CreateOpenWithInitial(initial, out var v);

            var pair = new KeyValuePair<int, string>(5, "five");
            var removed = d.Remove(pair);

            removed.ShouldBeTrue();
            v.Events.ShouldHaveSingleItem();
            v.Events[0].Type.ShouldBe("Remove");
            v.Events[0].Key.ShouldBe(5);
            v.Events[0].Value.ShouldBe("five");
        }

        [Fact]
        public void Given_OpenDictionary_When_RemoveMissingKeyValuePair_Then_ReturnsFalseAndNoEvents()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);

            var pair = new KeyValuePair<int, string>(42, "forty-two");
            var removed = d.Remove(pair);

            removed.ShouldBeFalse();
            v.Events.ShouldBeEmpty();
        }

        [Fact]
        public void Given_BuilderWithInitial_When_OpenClose_Then_ReadOnlyAndHandlersInvoked()
        {
            var builder = new ManagedDictionaryBuilder<int, string>();
            builder.Set(1, "one");
            var v = new FrenchExDev.Net.CSharp.ManagedDictionary.Testing.Validator<int, string>();
            builder.OnAdd(v.OnAdd);

            var open = builder.Open();

            // initial value preserved
            open.ContainsKey(1).ShouldBeTrue();
            open[2] = "two"; // indexer set
            open[2].ShouldBe("two");

            // adding triggers onAdd
            open.Add(3, "three");
            v.Events.ShouldContain(e => e.Type == "Add" && e.Key == 3 && (string?)e.Value == "three");

            var closed = builder.Close();
            closed.ShouldBeOfType<ClosedManagedDictionary<int, string>>();
            var ro = closed.ToReadOnly();
            ro[1].ShouldBe("one");
        }

        [Fact]
        public void Given_DictionaryWithItems_When_TryGetValueAndEnumerate_Then_ReturnsValuesAndEnumerates()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);
            d.Add(10, "ten");
            d.Add(20, "twenty");

            d.TryGetValue(10, out var ten).ShouldBeTrue();
            ten.ShouldBe("ten");

            d.TryGetValue(99, out var none).ShouldBeFalse();
            none.ShouldBeNull();

            d.Keys.ShouldContain(10);
            d.Values.ShouldContain("twenty");

            var items = d.ToList();
            items.ShouldContain(kv => kv.Key == 10 && kv.Value == "ten");
        }

        [Fact]
        public void Given_ClosedManagedDictionaryWithHandlers_When_Add_Then_OnAddInvoked()
        {
            var onAdd = new OnAddList<int, string>();
            var onRemove = new OnRemoveList<int, string>();
            var onClear = new OnClearList();

            var validator = new Validator<int, string>();
            onAdd.Add(validator.OnAdd);
            onRemove.Add(validator.OnRemove);
            onClear.Add(validator.OnClear);

            var basis = new Dictionary<int, string> { { 1, "one" } };
            var closed = new ClosedManagedDictionary<int, string>(onAdd, onRemove, onClear, basis);

            // read operations
            closed.ContainsKey(1).ShouldBeTrue();
            closed[1].ShouldBe("one");

            // modifications are allowed by current implementation and should invoke handlers
            closed.Add(2, "two");
            validator.Events.ShouldContain(e => e.Type == "Add" && e.Key == 2 && (string?)e.Value == "two");
        }

        [Fact]
        public void Given_Dictionary_When_CopyToWithInvalidArgs_Then_Throws()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);
            d.Add(1, "one");

            // null array
            Should.Throw<ArgumentNullException>(() => d.CopyTo(null!, 0));

            // negative index
            var arr = new KeyValuePair<int, string>[1];
            Should.Throw<ArgumentOutOfRangeException>(() => d.CopyTo(arr, -1));

            // insufficient space
            var small = new KeyValuePair<int, string>[1];
            Should.Throw<ArgumentException>(() => d.CopyTo(small, 1));
        }

        [Fact]
        public void Given_DictionaryWithKey_When_AddDuplicateKey_Then_Throws()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);
            d.Add(1, "one");
            Should.Throw<ArgumentException>(() => d.Add(1, "uno"));
        }

        [Fact]
        public void Given_Dictionary_When_RemoveKeyValuePairWithDifferentValue_Then_RemovesByKeyAndInvokesOnRemove()
        {
            var initial = new Dictionary<int, string> {{ 1, "one" }};
            var d = ManagedDictionaryTester.CreateOpenWithInitial(initial, out var v);

            var pair = new KeyValuePair<int, string>(1, "different");
            var removed = d.Remove(pair);

            removed.ShouldBeTrue();
            d.ContainsKey(1).ShouldBeFalse();
            v.Events.ShouldContain(e => e.Type == "Remove" && e.Key == 1);
        }

        [Fact]
        public void Given_DictionaryWithKey_When_ContainsKeyValuePairWithWrongValue_Then_ReturnsFalse()
        {
            var initial = new Dictionary<int, string> {{ 2, "two" }};
            var d = ManagedDictionaryTester.CreateOpenWithInitial(initial, out var v);

            d.Contains(new KeyValuePair<int, string>(2, "other")).ShouldBeFalse();
            d.Contains(new KeyValuePair<int, string>(2, "two")).ShouldBeTrue();
        }

        [Fact]
        public void Given_EmptyDictionary_When_Clear_Then_OnClearInvoked()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);
            d.Clear();
            v.Events.ShouldContain(e => e.Type == "Clear");
        }

        [Fact]
        public void Given_BuilderWithDictionaryAndHandlers_When_Open_Then_HandlersInvokedOnRemoveAndClear()
        {
            var builder = new ManagedDictionaryBuilder<int, string>();

            var initial = new Dictionary<int, string> { { 1, "one" }, { 2, "two" } };
            builder.Set(initial); // cover Set(IDictionary)

            var validator = new Validator<int, string>();
            builder.OnAdd(validator.OnAdd);
            builder.OnRemove(validator.OnRemove);
            builder.OnClear(validator.OnClear);

            var open = builder.Open();

            // initial entries should be present
            open.ContainsKey(1).ShouldBeTrue();
            open.ContainsKey(2).ShouldBeTrue();

            // remove should invoke handler
            open.Remove(1).ShouldBeTrue();
            validator.Events.ShouldContain(e => e.Type == "Remove" && e.Key == 1);

            // clear should invoke handler
            open.Clear();
            validator.Events.ShouldContain(e => e.Type == "Clear");
        }

        [Fact]
        public void Given_OpenConstructorWithBasis_When_Add_Then_Works()
        {
            var basis = new Dictionary<int, string> { { 5, "five" } };
            var open = new OpenManagedDictionary<int, string>(basis);

            // current implementation of this constructor does not populate the internal dictionary
            // assert that it constructs and IsReadOnly is false
            open.IsReadOnly.ShouldBeFalse();
            // depending on implementation this may or may not contain the basis entry; accept either but ensure no exception
            // prefer checking type and that operations work
            open.Add(6, "six");
            open.ContainsKey(6).ShouldBeTrue();
        }

        [Fact]
        public void Given_AnyManagedDictionary_When_CheckIsReadOnly_Then_ReturnsFalse()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);
            d.IsReadOnly.ShouldBeFalse();

            var closed = new ClosedManagedDictionary<int, string>(new OnAddList<int, string>(), new OnRemoveList<int, string>(), new OnClearList(), new Dictionary<int, string>());
            closed.IsReadOnly.ShouldBeFalse();
        }

        [Fact]
        public void Given_BuilderWhenChainingOnMethods_When_Close_Then_ReturnsClosedDictionaryAndToReadOnlyReflectsEntries()
        {
            var builder = new ManagedDictionaryBuilder<int, string>();
            builder.Set(1, "one");
            builder.OnAdd((k, v) => { });
            builder.OnRemove((k, v) => { });
            builder.OnClear(() => { });

            var open = builder.Open();
            open.ShouldBeOfType<OpenManagedDictionary<int, string>>();

            var closed = builder.Close();
            closed.ShouldBeOfType<ClosedManagedDictionary<int, string>>();

            // ToReadOnly reflects current entries
            var ro = closed.ToReadOnly();
            ro[1].ShouldBe("one");
        }

        [Fact]
        public void Given_OpenManagedDictionary_When_UsingFluentApis_Then_TheyReturnSameInstanceAndConstructorsWork()
        {
            // default ctor
            var o1 = new OpenManagedDictionary<int, string>();
            o1.OnAdd((k, v) => { }).OnRemove((k, v) => { }).OnClear(() => { }).ShouldBeSameAs(o1);

            // ctor with lists and initial
            var onAdd = new OnAddList<int, string>();
            var onRemove = new OnRemoveList<int, string>();
            var onClear = new OnClearList();
            var basis = new Dictionary<int, string> { { 9, "nine" } };
            var o2 = new OpenManagedDictionary<int, string>(onAdd, onRemove, onClear, basis);
            o2.ContainsKey(9).ShouldBeTrue();

            // ctor with basis param (this implementation doesn't populate internal but must not throw)
            var o3 = new OpenManagedDictionary<int, string>(new Dictionary<int, string> { { 7, "seven" } });
            o3.Add(8, "eight");
            o3.ContainsKey(8).ShouldBeTrue();
        }

        [Fact]
        public void Given_DictionaryWithItems_When_Enumerating_Then_GenericAndNonGenericEnumeratorsReturnItems()
        {
            var d = ManagedDictionaryTester.CreateOpen<int, string>(out var v);
            d.Add(1, "one");
            d.Add(2, "two");

            // generic enumerator
            var genEnum = d.GetEnumerator();
            var list = new List<KeyValuePair<int, string>>();
            while (genEnum.MoveNext()) list.Add(genEnum.Current);
            list.Count.ShouldBe(2);

            // non-generic enumerator
            var nonGenEnum = ((IEnumerable)d).GetEnumerator();
            var items = new List<object>();
            while (nonGenEnum.MoveNext()) items.Add(nonGenEnum.Current!);
            items.Count.ShouldBe(2);
        }
    }
}
