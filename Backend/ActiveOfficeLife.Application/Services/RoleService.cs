using ActiveOfficeLife.Application.Common;
using ActiveOfficeLife.Application.ExtensitionModel;
using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System.Reflection;

namespace ActiveOfficeLife.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly _IUnitOfWork _iUnitOfWork;

        public RoleService(IRoleRepository roleRepository, _IUnitOfWork iUnitOfWork)
        {
            _roleRepository = roleRepository;
            _iUnitOfWork = iUnitOfWork;
        }

        public async Task<RoleModel> Add(RoleModel role)
        {
            string msg = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name} : ";
            try
            {
                var newRole = new Role()
                {
                    Id = Guid.NewGuid(),
                    Description = role.Description,
                    Name = role.Name
                };
                await _roleRepository.AddAsync(newRole);
                await _iUnitOfWork.SaveChangesAsync();
                AOLLogger.Info($"{msg} - Added Role - {newRole.Name}");
                return newRole.ReturnModel();
                
            } catch(Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                throw new Exception("Role added");
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            string msg = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name} : ";
            try
            {
                var deleteRole = await _roleRepository.GetByIdAsync(id);
                if (deleteRole == null)
                {
                    AOLLogger.Error($"{msg} - Role not found by Id: {id.ToString()}");
                    throw new Exception($"Role not found");
                }
                _roleRepository.Remove(deleteRole);
                await _iUnitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                throw new Exception("Role delete Faild");
            }
        }

        public async Task<RoleModel> Update(RoleModel role)
        {
            string msg = $"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name} : ";
            try
            {
                var updateRole = await _roleRepository.GetByIdAsync(role.Id);
                if (updateRole == null)
                {
                    AOLLogger.Error($"{msg} - Role not found by Id: {role.Id.ToString()}");
                    throw new Exception($"Role not found");
                }
                updateRole.Description = role.Description;
                updateRole.Name = role.Name;
                _roleRepository.Update(updateRole);
                await _iUnitOfWork.SaveChangesAsync();
                return updateRole.ReturnModel();
            } catch(Exception ex)
            {
                AOLLogger.Error(ex.Message, ex.Source, null, ex.StackTrace);
                throw new Exception("Role Update Faild");
            }
        }
    }
}
