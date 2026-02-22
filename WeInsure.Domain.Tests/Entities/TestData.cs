using System.Collections;

namespace WeInsure.Domain.Tests.Entities;

public class PolicyInvalidStartDateTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()            
    {
        yield return [DateOnly.FromDateTime(DateTime.UtcNow.AddDays(61))];
        yield return [DateOnly.FromDateTime(DateTime.UtcNow.AddDays(62))];
        yield return [DateOnly.FromDateTime(DateTime.UtcNow.AddDays(63))];
        yield return [DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))];
        yield return [DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1))];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}