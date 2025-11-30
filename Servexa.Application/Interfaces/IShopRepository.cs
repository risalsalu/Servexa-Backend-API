using System;
using System.Threading.Tasks;
using Servexa.Domain.Models;

namespace Servexa.Application.Interfaces;

public interface IShopRepository
{
    Task<bool> OwnerHasShopAsync(Guid ownerId);
    Task<Guid> CreateAsync(Shop shop);
    Task<Shop?> GetByOwnerIdAsync(Guid ownerId);
    Task UpdateAsync(Shop shop);
    Task SetActiveStatusAsync(Guid ownerId, bool isActive);
}
