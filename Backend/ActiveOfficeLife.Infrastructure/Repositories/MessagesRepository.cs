using ActiveOfficeLife.Domain.EFCore.DBContext;
using ActiveOfficeLife.Domain.Entities;
using ActiveOfficeLife.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Infrastructure.Repositories
{
    public class MessagesRepository : _Repository<Message>, IMessageRepository
    {
        public MessagesRepository(ActiveOfficeLifeDbContext dbContext) : base(dbContext)
        {
        }
    }
}
