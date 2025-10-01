using Shouldly;

namespace FrenchExDev.Net.CSharp.Object.Biz.Tests;

/// <summary>
/// Contains unit tests for verifying the behavior of facet objects and their associated facets.
/// </summary>
/// <remarks>This class is intended for internal testing purposes and is not part of the public API. It uses xUnit
/// to validate the correct association and retrieval of facet instances.</remarks>
public class FacetObjectTests
{
    /// <summary>
    /// Represents a facet that provides metadata or behavior specific to car-related objects within the facet system.
    /// </summary>
    /// <remarks>This class is intended for internal use and extends the base Facet functionality to support
    /// car-specific scenarios. It implements the IFacet interface to enable integration with systems that consume
    /// facets. Instances of this class are typically created and managed by the framework and are not intended for
    /// direct use in application code.</remarks>
    internal class CarFacet : Facet, IFacet
    {
        public CarFacet(object ofInstance) : base(ofInstance)
        {
        }
    }

    /// <summary>
    /// Verifies that a facet added to a FacetObject can be retrieved and matches the original instance.
    /// </summary>
    /// <remarks>This test ensures that the FacetObject correctly stores and returns a facet of type CarFacet,
    /// and that the retrieved facet is the same instance as the one added. Use this test to validate facet registration
    /// and retrieval behavior.</remarks>
    [Fact]
    public void Test1()
    {
        var car = new FacetObject();
        var carFacet = new CarFacet(car);
        car.Facet(carFacet);

        var getCarFacet = car.Facet<CarFacet>().Object;
        getCarFacet.ShouldBeSameAs(carFacet);
    }
}
