using System;
using System.Threading;
using CoreApiClient;
using DesafioTorneioLuta.Helper;

namespace DesafioTorneioLuta.Factory
{
    internal static class APIClientFactory
    {
        #region Variables

        private static Uri apiUri;

        private static Lazy<APIClient> restClient = new Lazy<APIClient>(
          () => new APIClient(apiUri),
          LazyThreadSafetyMode.ExecutionAndPublication);

        #endregion


        #region Constructor

        /// <summary>
        /// Cria a 
        /// </summary>
        static APIClientFactory()
        {
            apiUri = new Uri(ApplicationSettings.WebApiUrl);
        }

        #endregion

        #region Methods

        public static APIClient Instance
        {
            get
            {
                return restClient.Value;
            }
        }

        #endregion

    }
}
