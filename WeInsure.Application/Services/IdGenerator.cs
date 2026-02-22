namespace WeInsure.Application.Services;

public class IdGenerator : IIdGenerator
{
    public Guid Generate() => Guid.CreateVersion7();
}