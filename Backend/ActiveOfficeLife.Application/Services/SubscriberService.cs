using ActiveOfficeLife.Application.Interfaces;
using ActiveOfficeLife.Common.Models;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISubscriberRepository _subscriberRepository;
        public SubscriberService(IUnitOfWork unitOfWork, ISubscriberRepository subscriberRepository)
        {
            _unitOfWork = unitOfWork;
            _subscriberRepository = subscriberRepository;
        }

        public async Task<SubscriberModel> AddSubscriberAsync(string email)
        {
            var subs = new Subscriber()
            {
                Email = email,
                IsActive = true,
                SubscribedAt = DateTime.UtcNow
            };
            // email exits check
            var existingSubs = await _subscriberRepository.GetByEmailAsync(email);
            if (existingSubs != null)
            {
                return new SubscriberModel()
                {
                    Id = existingSubs.Id,
                    Email = existingSubs.Email,
                    IsActive = existingSubs.IsActive,
                    SubscribedAt = existingSubs.SubscribedAt
                };
            }
            await _subscriberRepository.AddAsync(subs);
            await _unitOfWork.SaveChangesAsync();
            return new SubscriberModel()
            {
                Id = subs.Id,
                Email = subs.Email,
                IsActive = subs.IsActive,
                SubscribedAt = subs.SubscribedAt
            };
        }
    }
}
