using WeInsure.Domain.Entities;
using WeInsure.Domain.Shared;

namespace WeInsure.Domain.Tests.Entities;

public class PolicyHolderTests
{
    
    private readonly Guid _policyHolderId = Guid.CreateVersion7();
    private readonly Guid _policyId = Guid.CreateVersion7();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void PolicyHolder_Create_ShouldReturnDomainError_WhenFirstNameIsNullOrWhitespace(string firstName)
    {
        var lastName = "Doe";
        var dateOfBirth = DateOnly.FromDateTime(new DateTime(1984, 1, 1));

        var result = PolicyHolder.Create(_policyHolderId, _policyId, firstName, lastName, dateOfBirth);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy holder first name is required.", result.Error.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void PolicyHolder_Create_ShouldReturnDomainError_WhenLastNameIsNullOrWhitespace(string lastName)
    {
        var firstName = "John";
        var dateOfBirth = DateOnly.FromDateTime(new DateTime(1984, 1, 1));

        var result = PolicyHolder.Create(_policyHolderId, _policyId,firstName, lastName, dateOfBirth);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy holder last name is required.", result.Error.Message);
    }

    [Fact]
    public void PolicyHolder_Create_ShouldReturnDomainError_WhenDateOfBirthIsInTheFuture()
    {
        var firstName = "John";
        var lastName = "Doe";
        var dateOfBirth = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

        var result = PolicyHolder.Create(_policyHolderId, _policyId, firstName, lastName, dateOfBirth);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.Equal(ErrorType.Domain, result.Error.Type);
        Assert.Equal("Policy holder date of birth cannot be in the future.", result.Error.Message);
    }

    [Fact]
    public void PolicyHolder_Create_ShouldReturnSuccess_WhenAllFieldsAreValid()
    {
        var firstName = "John";
        var lastName = "Doe";
        var dateOfBirth = DateOnly.FromDateTime(new DateTime(1984, 1, 1));

        var result = PolicyHolder.Create(_policyHolderId, _policyId, firstName, lastName, dateOfBirth);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(firstName, result.Data.FirstName);
        Assert.Equal(lastName, result.Data.LastName);
        Assert.Equal(dateOfBirth, result.Data.DateOfBirth);
    }
}