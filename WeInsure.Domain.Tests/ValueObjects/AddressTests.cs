using WeInsure.Domain.Shared;
using WeInsure.Domain.ValueObjects;

namespace WeInsure.Domain.Tests.ValueObjects;

public class AddressTests
{
    [Theory]
    [InlineData("Address1", "Address2",  "Address3", "Postcode")]
    [InlineData("Address1", null, null, "Postcode")]
    [InlineData("Address1", "", "", "Postcode")]
    [InlineData("Address1", null, "", "Postcode")]
    [InlineData("Address1", "", null, "Postcode")]
    public void Address_Create_ShouldReturnAddress_WhenValid(string addressLine1, string? addressLine2, string? addressLine3, string postcode)
    {
        var address = Address.Create(addressLine1, addressLine2, addressLine3, postcode);
        
        Assert.True(address.IsSuccess);
        Assert.Null(address.Error);
        Assert.Equal(addressLine1, address.Data.AddressLine1);
        Assert.Equal(addressLine2, address.Data.AddressLine2);
        Assert.Equal(addressLine3, address.Data.AddressLine3);
        Assert.Equal(postcode, address.Data.PostCode);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    public void Address_Create_ShouldReturnError_WhenAddressLine1IsInvalid(string addressLine1)
    {
        var address = Address.Create(addressLine1, "Address2", "Address3", "PostCode");
        
        Assert.False(address.IsSuccess);
        Assert.Null(address.Data);
        Assert.Equal(ErrorType.Domain, address.Error.Type);
        Assert.Equal("AddressLine1 is required", address.Error.Message);
    }
    
    
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("ABCDEFGHI")]
    public void Address_Create_ShouldReturnError_WhenPostCodeIsInvalid(string postcode)
    {
        var address = Address.Create("addressLine1", "Address2", "Address3", postcode);
        
        Assert.False(address.IsSuccess);
        Assert.Null(address.Data);
        Assert.Equal(ErrorType.Domain, address.Error.Type);
        Assert.Equal("Postcode is required and must be no longer than 8 characters", address.Error.Message);
    }
}