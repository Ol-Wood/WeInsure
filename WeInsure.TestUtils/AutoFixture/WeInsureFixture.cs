using AutoFixture;

namespace TestUtils.AutoFixture;

public class WeInsureFixture : Fixture
{
    public WeInsureFixture()
    {
        Customizations.Add(new DateOnlySpecimenBuilder());
    }
}