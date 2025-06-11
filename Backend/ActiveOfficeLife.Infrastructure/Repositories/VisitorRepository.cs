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
    public class VisitorRepository : _Repository<Visitor>, IVisitorRepository
    {
        public VisitorRepository(ActiveOfficeLifeDbContext context) : base(context)
        {
        }
    }
}
