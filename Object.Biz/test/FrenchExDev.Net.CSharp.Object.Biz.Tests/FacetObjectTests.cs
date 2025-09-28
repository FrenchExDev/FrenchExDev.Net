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


/// <summary>
/// Provides unit tests for generic facet objects to verify correct facet assignment and retrieval behavior.
/// </summary>
/// <remarks>This class contains test cases for the interaction between facet objects and their associated facets,
/// ensuring that facets can be set and accessed as expected. Intended for use with automated testing
/// frameworks.</remarks>
public class FacetObjectGenericTests
{
    /// <summary>
    /// Represents a facet that provides car-specific metadata or categorization within a faceted search or filtering
    /// system.
    /// </summary>
    /// <remarks>Implementations of this interface typically define properties or methods that describe
    /// characteristics of cars, such as make, model, or other attributes relevant to filtering or grouping in search
    /// scenarios. This interface extends <see cref="IFacet"/>, allowing car-related facets to be used interchangeably
    /// with other facet types in the system.</remarks>
    internal interface ICarFacet : IGenericFacet<Car> { }

    /// <summary>
    /// Represents a car object that provides access to car-specific functionality through the associated facet.
    /// </summary>
    /// <remarks>Use this type to interact with car-related features exposed by the <see cref="ICarFacet"/>
    /// interface. This class is intended for internal use within the application and is not accessible outside its
    /// assembly.</remarks>
    internal class Car : FacetObject<Car, ICarFacet>
    {
    }

    /// <summary>
    /// Represents a facet of a car, providing access to the associated instance and facet operations.
    /// </summary>
    internal class CarFacet : ICarFacet
    {
        /// <summary>
        /// Stores the instance of the car associated with this facet.
        /// </summary>
        private Car _ofInstance;

        /// <summary>
        /// Initializes a new instance of the CarFacet class for the specified Car object.
        /// </summary>
        /// <param name="ofInstance">The Car instance to associate with this facet. Cannot be null.</param>
        public CarFacet(Car ofInstance)
        {
            _ofInstance = ofInstance;
        }

        /// <summary>
        /// Returns the associated Car instance for this object.
        /// </summary>
        /// <returns>The Car instance linked to this object.</returns>
        public Car OfInstance()
        {
            return _ofInstance;
        }

        /// <summary>
        /// Returns the current instance as a car facet, enabling access to facet-specific functionality.
        /// </summary>
        /// <returns>The current <see cref="CarFacet"/> instance representing this car facet.</returns>
        public CarFacet Facet()
        {
            return this;
        }
    }

    [Fact]
    public void Test1()
    {
        var car = new Car();
        var carFacet = new CarFacet(car);
        car.Facet(carFacet);
        var getCarFacet = car.Facet().Object;
        getCarFacet.ShouldBeSameAs(carFacet);
    }
}