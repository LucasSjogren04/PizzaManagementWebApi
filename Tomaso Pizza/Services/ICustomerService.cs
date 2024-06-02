using Microsoft.AspNetCore.Identity;
using Tomaso_Pizza.Data.DTO;

namespace Tomaso_Pizza.Services
{
    public interface ICustomerService
    {
        Task<OrderRQResult> PremiumOrder(IdentityUser user, OrderRequest orderRequest);
        Task<OrderRQResult> RegularOrder(IdentityUser user, OrderRequest orderRequest);
    }
}