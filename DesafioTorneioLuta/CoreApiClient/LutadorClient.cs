using CoreModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApiClient
{
    public partial class APIClient
    {

        public async Task<List<LutadorModel>> GetLutadores()
        {
            var requestUrl = CreateRequestUri("");

            return await GetAsync<List<LutadorModel>>(requestUrl);
        }


    }
}
