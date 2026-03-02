namespace DotnetBackendV1.Domain;

public class Customer
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public List<Project> Projects { get; set; } = new();
}

public class Project
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string? VendorLocation { get; set; }
    public string? SiteAddress { get; set; }
    public List<Asset> Assets { get; set; } = new();
}

public class Asset
{
    public string Id { get; set; } = default!;

    // Foreign keys to match front-end queries
    public string CustomerId { get; set; } = default!;
    public string ProjectId { get; set; } = default!;

    public string UnitRef { get; set; } = default!;
    public string UnitType { get; set; } = default!;
    public string Manufacturer { get; set; } = default!;

    public string? IndoorModel { get; set; }
    public string? SerialNumber { get; set; }
    public string? OutdoorModel { get; set; }
    public string? OutdoorSerial { get; set; }

    public string? VendorArea { get; set; }
    public string? VendorLocation { get; set; }
    public string? RefrigerantType { get; set; }
    public string? RefrigerantKg { get; set; }

    public string Status { get; set; } = "Registered";
}

