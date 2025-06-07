using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using TimeCafeWinUI3.Core.Contracts.Services;
using TimeCafeWinUI3.Core.Models;
using TimeCafeWinUI3.Core.Services;

namespace TimeCafeWinUI3.Tests.MSTest.IntegrationTests;

[TestClass]
public class ClientServiceIntegrationTests
{
    private static IServiceProvider _serviceProvider;
    private static IClientService _clientService;
    private static TimeCafeContext _context;
    private static IServiceScope _scope;

    [ClassInitialize]
    public static async Task ClassInitialize(TestContext testContext)
    {
        var services = new ServiceCollection();

        services.AddDbContext<TimeCafeContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString());
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
            options.UseInternalServiceProvider(null);
        });

        services.AddScoped<IClientService, ClientService>();

        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();
        _clientService = _scope.ServiceProvider.GetRequiredService<IClientService>();
        _context = _scope.ServiceProvider.GetRequiredService<TimeCafeContext>();

        await InitializeTestDataAsync();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        _scope?.Dispose();
        if (_serviceProvider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    [TestInitialize]
    public async Task TestInitialize()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        await InitializeTestDataAsync();
    }

    private static async Task InitializeTestDataAsync()
    {
        _context.ClientStatuses.RemoveRange(_context.ClientStatuses);
        _context.Genders.RemoveRange(_context.Genders);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        var statuses = new[]
        {
            new ClientStatus { StatusId = (int)ClientStatusType.Draft, StatusName = "Черновик" },
            new ClientStatus { StatusId = (int)ClientStatusType.Active, StatusName = "Активный" },
            new ClientStatus { StatusId = (int)ClientStatusType.Rejected, StatusName = "Отказ от услуг" }
        };
        await _context.ClientStatuses.AddRangeAsync(statuses);

        var genders = new[]
        {
            new Gender { GenderId = 1, GenderName = "Мужской" },
            new Gender { GenderId = 2, GenderName = "Женский" }
        };
        await _context.Genders.AddRangeAsync(genders);

        await _context.SaveChangesAsync();
    }

    [TestMethod]
    public async Task CreateClient_ValidData_ShouldCreateClient()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+375 (29) 714 3237",
            StatusId = (int)ClientStatusType.Draft,
            GenderId = 1
        };

        // Act
        var result = await _clientService.CreateClientAsync(client);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.ClientId > 0);
        Assert.AreEqual(client.FirstName, result.FirstName);
        Assert.AreEqual(client.LastName, result.LastName);
        Assert.AreEqual(client.PhoneNumber, result.PhoneNumber);
        Assert.AreEqual(client.StatusId, result.StatusId);
        Assert.AreEqual(client.GenderId, result.GenderId);
    }

    [TestMethod]
    public async Task GetAllClients_ShouldReturnAllClients()
    {
        // Arrange
        var client1 = new Client
        {
            FirstName = "Test1",
            LastName = "User1",
            PhoneNumber = "+375 (29) 714 3237",
            StatusId = (int)ClientStatusType.Draft,
            GenderId = 1
        };
        var client2 = new Client
        {
            FirstName = "Test2",
            LastName = "User2",
            PhoneNumber = "+79001234568",
            StatusId = (int)ClientStatusType.Active,
            GenderId = 2
        };
        await _context.Clients.AddRangeAsync(client1, client2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clientService.GetAllClientsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.IsTrue(result.Any(c => c.PhoneNumber == client1.PhoneNumber));
        Assert.IsTrue(result.Any(c => c.PhoneNumber == client2.PhoneNumber));
    }

    [TestMethod]
    public async Task GetClientById_ExistingClient_ShouldReturnClient()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+375 (29) 714 3237",
            StatusId = (int)ClientStatusType.Draft,
            GenderId = 1
        };
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clientService.GetClientByIdAsync(client.ClientId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(client.ClientId, result.ClientId);
        Assert.AreEqual(client.FirstName, result.FirstName);
        Assert.AreEqual(client.LastName, result.LastName);
        Assert.AreEqual(client.PhoneNumber, result.PhoneNumber);
    }

    [TestMethod]
    public async Task GetClientById_NonExistingClient_ShouldReturnNull()
    {
        // Act
        var result = await _clientService.GetClientByIdAsync(999);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task ValidatePhoneNumber_ValidNumber_ShouldReturnTrue()
    {
        // Arrange
        var phoneNumber = "+375 (29) 714 3237";

        // Act
        var result = await _clientService.ValidatePhoneNumberAsync(phoneNumber);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task ValidatePhoneNumber_InvalidNumber_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumber = "invalid";

        // Act
        var result = await _clientService.ValidatePhoneNumberAsync(phoneNumber);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public async Task UpdateClientStatus_ValidData_ShouldUpdateStatus()
    {
        // Arrange
        var client = new Client
        {
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+375 (29) 714 3237",
            StatusId = (int)ClientStatusType.Draft,
            GenderId = 1
        };
        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        // Act
        var result = await _clientService.UpdateClientStatusAsync(client.ClientId, (int)ClientStatusType.Active);

        // Assert
        Assert.IsTrue(result);
        var updatedClient = await _context.Clients.FindAsync(client.ClientId);
        Assert.AreEqual((int)ClientStatusType.Active, updatedClient.StatusId);
    }
} 