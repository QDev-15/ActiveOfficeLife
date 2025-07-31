using ActiveOfficeLife.Common.Models;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleModel> Add(RoleModel role);
        Task<RoleModel> Update(RoleModel role);
        Task<bool> Delete(Guid id);
    }
}
