using ActiveOfficeLife.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ILogService
    {
        void Info(LogProperties option);
        void Error(LogProperties option);
        void Debug(LogProperties option);
        void Trace(LogProperties option);
        void Warn(LogProperties option);
    }
}
