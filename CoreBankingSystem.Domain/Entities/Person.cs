namespace CoreBankingSystem.Domain.Entities;

public class Person
{
    // Primary Key
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Identification { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
