using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DesafioTorneioLuta.Models;
using DesafioTorneioLuta.Factory;
using Microsoft.Extensions.Options;
using DesafioTorneioLuta.Helper;
using Microsoft.AspNetCore.Http;

namespace DesafioTorneioLuta.Controllers
{
    public class TorneioController : Controller
    {
        #region Variables

        private readonly IOptions<MySettingsModel> appSettings;
        private Service.TorneiodeLutas torneio;

        #endregion

        #region Constructor

        public TorneioController(IOptions<MySettingsModel> app)
        {
            //Recupera os dados de appsettings
            appSettings = app;
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> Torneio()
        {
            try
            {
                //Realiza a preparação do torneio            
                torneio = new Service.TorneiodeLutas(appSettings);

                //Recupera os lutadoes para serem selecionados 
                IEnumerable<CoreModels.LutadorModel> prep = await torneio.Preparacao();
                
                //Verifica se foi retornado alguma informação
                if (prep == null)
                {
                    //Retorna os lutadores disponíveis
                    ViewBag.Message = torneio.messageError;
                    return View("Erro");
                }
                else
                {
                    List<CoreModels.LutadorModel> prep2 = prep.ToList<CoreModels.LutadorModel>();
                    //Retorna os lutadores disponíveis
                    ViewData["Lutadores"] = prep.ToList<CoreModels.LutadorModel>();
                    ViewBag.Message = null;
                    return View(prep2);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        public IActionResult Torneio(IEnumerable<CoreModels.LutadorModel> lutadores)
        {

            //Verifica se foram selecionados somente 20 lutadores
            IEnumerable<CoreModels.LutadorModel> lutadoresSelecionados = (IEnumerable<CoreModels.LutadorModel>)lutadores.Where(g => g.selecionado == true);

            if(lutadoresSelecionados.Count() != 20)
            {
                ViewBag.Message = "Selecione 20 lutadores para iniciar o Torneio!";
                return View(lutadores);
            }
            else
            {
                //Inicia o torneio
                torneio = new Service.TorneiodeLutas(appSettings);
                var vencedores = torneio.IniciarTorneio(lutadoresSelecionados);

                return View("Resultado", vencedores);
            }
            
        }



    }
}
