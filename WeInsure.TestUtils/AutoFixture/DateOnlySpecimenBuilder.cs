using AutoFixture;
using AutoFixture.Kernel;

namespace TestUtils.AutoFixture;

public class DateOnlySpecimenBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is not Type type || type != typeof(DateOnly))
        {
            return new NoSpecimen();
        }

        var dateTime = context.Create<DateTime>();
        return DateOnly.FromDateTime(dateTime);
    }
}