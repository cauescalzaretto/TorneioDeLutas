using System;
using System.Collections;
using System.Collections.Generic;

namespace CoreTorneio
{


    public class Torneio
    {

        #region Variables

        private readonly IOptions<MySettingsModel> appSettings;
        private static IEnumerable<CoreModels.LutadorModel> lutadores;

        #endregion





        #region Constructor

        public Torneio()
        {
            //Recupera os dados dos lutadores
            lutadores = CoreApiClient.APIClient()
        }



        #endregion

    }



}
