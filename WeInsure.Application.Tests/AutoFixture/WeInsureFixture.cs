using AutoFixture;

namespace WeInsure.Application.Tests.AutoFixture;

public class WeInsureFixture : Fixture
{
    public WeInsureFixture()
    {
        Customizations.Add(new DateOnlySpecimenBuilder());
    }
}