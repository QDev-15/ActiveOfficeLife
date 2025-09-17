using ActiveOfficeLife.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveOfficeLife.Application.Interfaces
{
    public interface ISeoMetaDataService
    {
        Task<SeoMetadataModel> Add(SeoMetadataModel seoMetadataModel);
        Task<SeoMetadataModel> Update(SeoMetadataModel seoMetadataModel);
        Task<SeoMetadataModel> GetById(Guid id);
        Task<bool> Delete(Guid id);
    }
}
