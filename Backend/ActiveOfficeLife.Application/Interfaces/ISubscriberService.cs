using ActiveOfficeLife.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ISubscriberService
    {
        public Task<SubscriberModel> AddSubscriberAsync(string email);
    }
}
