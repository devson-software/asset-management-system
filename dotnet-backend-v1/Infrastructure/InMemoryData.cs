using DotnetBackendV1.Domain;

namespace DotnetBackendV1.Infrastructure;

/// <summary>
/// Simple in-memory data store to bootstrap the backend while we
/// gradually introduce a real database.
/// </summary>
public class InMemoryData
{
    private readonly List<Customer> _customers;

    public IReadOnlyList<Customer> Customers => _customers;

    public InMemoryData()
    {
        // Seeded roughly to match src/store.js so the front-end
        // can later be pointed at this API.
        _customers = new List<Customer>
        {
            new()
            {
                Id = "C1",
                Name = "Alpha Corp",
                Projects =
                {
                    new Project
                    {
                        Id = "P1",
                        Name = "Downtown Office",
                        VendorLocation = "Floor 12",
                        SiteAddress = "123 Business Ave, Tech City",
                        Assets =
                        {
                            new Asset
                            {
                                Id = "A1",
                                CustomerId = "C1",
                                ProjectId = "P1",
                                UnitRef = "Ac1.01",
                                UnitType = "Cassette Unit",
                                Manufacturer = "Samsung",
                                IndoorModel = "AC200KNHPKH/EU",
                                SerialNumber = "55896331",
                                OutdoorModel = "AC200KXAPKH/EU",
                                OutdoorSerial = "OUT-998877",
                                VendorArea = "Office Block A",
                                RefrigerantType = "R410A",
                                RefrigerantKg = "4",
                                Status = "Registered"
                            }
                        }
                    },
                    new Project
                    {
                        Id = "P6",
                        Name = "North Branch",
                        VendorLocation = "Server Room",
                        SiteAddress = "45 Industrial Way, North Tech",
                        Assets =
                        {
                            new Asset
                            {
                                Id = "A4",
                                CustomerId = "C1",
                                ProjectId = "P6",
                                UnitRef = "NB-01",
                                UnitType = "Midwall Split",
                                Manufacturer = "LG",
                                IndoorModel = "LG-V3",
                                SerialNumber = "SN1122",
                                OutdoorModel = "LG-V3-OUT",
                                OutdoorSerial = "LG-OS-4433",
                                VendorArea = "Server Wing",
                                RefrigerantType = "R32",
                                RefrigerantKg = "2",
                                Status = "Registered"
                            }
                        }
                    }
                }
            },
            new()
            {
                Id = "C2",
                Name = "Global Logistics Hub",
                Projects =
                {
                    new Project
                    {
                        Id = "P2",
                        Name = "Main Warehouse",
                        VendorLocation = "Loading Bay A",
                        SiteAddress = "789 Terminal Way, Port District",
                        Assets =
                        {
                            new Asset
                            {
                                Id = "A2",
                                CustomerId = "C2",
                                ProjectId = "P2",
                                UnitRef = "WH-01",
                                UnitType = "Hideaway Unit",
                                Manufacturer = "LG",
                                IndoorModel = "LG-INDUSTRIAL-V2",
                                SerialNumber = "SN992233",
                                OutdoorModel = "LG-IND-OUT-V2",
                                OutdoorSerial = "LG-OS-9988",
                                VendorArea = "Warehouse Floor",
                                RefrigerantType = "R32",
                                RefrigerantKg = "12",
                                Status = "Registered"
                            }
                        }
                    },
                    new Project
                    {
                        Id = "P3",
                        Name = "Admin Wing",
                        VendorLocation = "Roof Area",
                        SiteAddress = "789 Terminal Way, Port District"
                    },
                    new Project
                    {
                        Id = "P7",
                        Name = "Cold Storage",
                        VendorLocation = "Section B",
                        SiteAddress = "789 Terminal Way, Port District",
                        Assets =
                        {
                            new Asset
                            {
                                Id = "A5",
                                CustomerId = "C2",
                                ProjectId = "P7",
                                UnitRef = "CS-01",
                                UnitType = "Under Ceiling",
                                Manufacturer = "Carrier",
                                IndoorModel = "CARRIER-MAX",
                                SerialNumber = "SN5544",
                                OutdoorModel = "CARRIER-MAX-OUT",
                                OutdoorSerial = "CR-OS-5544",
                                VendorArea = "Cold Storage Wing",
                                RefrigerantType = "R404A",
                                RefrigerantKg = "15",
                                Status = "Registered"
                            }
                        }
                    }
                }
            },
            new()
            {
                Id = "C3",
                Name = "City Mall Plaza",
                Projects =
                {
                    new Project
                    {
                        Id = "P4",
                        Name = "Food Court",
                        VendorLocation = "Mezzanine Plant Room",
                        SiteAddress = "456 Retail Boulevard, Central",
                        Assets =
                        {
                            new Asset
                            {
                                Id = "A3",
                                CustomerId = "C3",
                                ProjectId = "P4",
                                UnitRef = "FC-AC-01",
                                UnitType = "Cassette Unit",
                                Manufacturer = "Daikin",
                                IndoorModel = "DAIKIN-CASSETTE-X",
                                SerialNumber = "DK887711",
                                OutdoorModel = "DAIKIN-OUTDOOR-X",
                                OutdoorSerial = "DK-OS-1122",
                                VendorArea = "Food Court Area",
                                RefrigerantType = "R410A",
                                RefrigerantKg = "2.5",
                                Status = "Registered"
                            }
                        }
                    }
                }
            },
            new()
            {
                Id = "C4",
                Name = "St. Mary's Hospital",
                Projects =
                {
                    new Project
                    {
                        Id = "P5",
                        Name = "Operating Theatre 1",
                        VendorLocation = "Clean Room Filter Bank",
                        SiteAddress = "101 Healthcare Lane, North Side"
                    },
                    new Project
                    {
                        Id = "P8",
                        Name = "Emergency Wing",
                        VendorLocation = "Ground Floor Lobby",
                        SiteAddress = "101 Healthcare Lane, North Side",
                        Assets =
                        {
                            new Asset
                            {
                                Id = "A6",
                                CustomerId = "C4",
                                ProjectId = "P8",
                                UnitRef = "EW-01",
                                UnitType = "Rooftop Package",
                                Manufacturer = "Trane",
                                IndoorModel = "TRANE-XL",
                                SerialNumber = "SN7788",
                                OutdoorModel = "TRANE-XL-OUT",
                                OutdoorSerial = "TR-OS-7788",
                                VendorArea = "ER Wing",
                                RefrigerantType = "R410A",
                                RefrigerantKg = "5",
                                Status = "Registered"
                            }
                        }
                    }
                }
            }
        };
    }

    public IEnumerable<Asset> GetAllAssets() =>
        _customers.SelectMany(c => c.Projects.SelectMany(p => p.Assets));

    public IEnumerable<Asset> GetAssetsForProject(string customerId, string projectId) =>
        _customers
            .Where(c => c.Id == customerId)
            .SelectMany(c => c.Projects)
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Assets);

    public Asset? GetAssetById(string id) =>
        GetAllAssets().FirstOrDefault(a => a.Id == id);

    public Asset AddAsset(string customerId, string projectId, Asset asset)
    {
        var customer = _customers.FirstOrDefault(c => c.Id == customerId)
                       ?? throw new InvalidOperationException($"Customer {customerId} not found");
        var project = customer.Projects.FirstOrDefault(p => p.Id == projectId)
                      ?? throw new InvalidOperationException($"Project {projectId} not found");

        asset.Id = "A" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        asset.CustomerId = customerId;
        asset.ProjectId = projectId;
        asset.Status = string.IsNullOrWhiteSpace(asset.Status) ? "Registered" : asset.Status;

        project.Assets.Add(asset);
        return asset;
    }

    public Asset? UpdateAsset(string id, Asset updated)
    {
        foreach (var customer in _customers)
        {
            foreach (var project in customer.Projects)
            {
                var index = project.Assets.FindIndex(a => a.Id == id);
                if (index >= 0)
                {
                    var existing = project.Assets[index];

                    existing.UnitRef = updated.UnitRef;
                    existing.UnitType = updated.UnitType;
                    existing.Manufacturer = updated.Manufacturer;
                    existing.IndoorModel = updated.IndoorModel;
                    existing.SerialNumber = updated.SerialNumber;
                    existing.OutdoorModel = updated.OutdoorModel;
                    existing.OutdoorSerial = updated.OutdoorSerial;
                    existing.VendorArea = updated.VendorArea;
                    existing.VendorLocation = updated.VendorLocation;
                    existing.RefrigerantType = updated.RefrigerantType;
                    existing.RefrigerantKg = updated.RefrigerantKg;
                    existing.Status = string.IsNullOrWhiteSpace(updated.Status) ? existing.Status : updated.Status;

                    return existing;
                }
            }
        }

        return null;
    }

    public bool DeleteAsset(string id)
    {
        foreach (var customer in _customers)
        {
            foreach (var project in customer.Projects)
            {
                var existing = project.Assets.FirstOrDefault(a => a.Id == id);
                if (existing != null)
                {
                    project.Assets.Remove(existing);
                    return true;
                }
            }
        }

        return false;
    }
}

