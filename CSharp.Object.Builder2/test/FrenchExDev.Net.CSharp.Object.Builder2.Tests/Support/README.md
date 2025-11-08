# Support/TestBuilders

This folder contains test-only builders used across unit tests in the `FrenchExDev.Net.CSharp.Object.Builder2.Tests` project.

These types are test doubles and helpers to exercise `AbstractBuilder<T>` behavior (validation, build lifecycle, and protected helper methods).

They are not part of the production API and exist solely to reduce duplication in tests and centralize common builder scenarios.

Do not reference these types from production code; they live under the test project and are included only in test runs.
