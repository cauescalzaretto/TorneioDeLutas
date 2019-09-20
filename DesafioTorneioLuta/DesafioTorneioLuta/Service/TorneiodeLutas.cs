using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using CoreModels;
using DesafioTorneioLuta.Factory;
using DesafioTorneioLuta.Helper;
using DesafioTorneioLuta.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Pages.Internal.Account;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Options;


namespace DesafioTorneioLuta.Service
{
    public class TorneiodeLutas
    {
        #region Variables

        private readonly IOptions<MySettingsModel> appSettings;


        public string messageError;

        #endregion

        #region Constructor

        public TorneiodeLutas(IOptions<MySettingsModel> app)
        {
            //Recupera os dados de appsettings
            appSettings = app;
            ApplicationSettings.WebApiUrl = appSettings.Value.WebApiBaseUrl;
        }

        #endregion

        #region Methods

        public async Task<IEnumerable<LutadorModel>> Preparacao()
        {
            messageError = "";

            try
            {
                IEnumerable<LutadorModel> lutadores;

                //Recupera os dados dos lutadores
                var data = await APIClientFactory.Instance.GetLutadores();
                lutadores = data.ToList<LutadorModel>();
                return lutadores;

            }
            catch (System.Exception ex)
            {
                messageError = "Não foi possível recuperar a lista de lutadores";
                return null;
            }

        }

        public IEnumerable<GrupoModel> IniciarTorneio(IEnumerable<LutadorModel> lutadoresSelecionados)
        {

            try
            {
                //Monta os grupos com base nos lutadores selecionados
                IEnumerable<GrupoModel> grupoLutadores = MontaGruposLutadores(lutadoresSelecionados);

                //Fase de Grupos
                IEnumerable<GrupoModel> vencedoresFaseGrupo = FasedeGrupos(grupoLutadores);

                //Quartas de Final
                IEnumerable<GrupoModel> vencedoresQuartasFinal = QuartasdeFinal(vencedoresFaseGrupo);

                //Semifinal
                IEnumerable<GrupoModel> resultadoSemiFinal = SemiFinal(vencedoresQuartasFinal);

                //Final
                IEnumerable<GrupoModel> resultadoFinal = SemiFinal(resultadoSemiFinal);

                //Pega o resultado da final e deixa somente o primeiro, segundo e terceiro lugar
                IEnumerable<GrupoModel> ganhadores = Final(resultadoSemiFinal);

                return ganhadores;
            }
            catch (System.Exception ex)
            {
                messageError = "Não foi possível iniciar o torneio. Por favor informar o responsável!";
                return null;
            }


        }


        #endregion

        #region Torneio Methods

        /// <summary>
        /// Realiza a montagem de grupo de lutadores, 4 grupos com 5 lutadores, da menor idade para a maior
        /// </summary>
        /// <param name="lutadoresSelecionados">Lutadores Selecionados a participar da competição</param>
        /// <returns>retorna os grupos formados</returns>
        private static IEnumerable<GrupoModel> MontaGruposLutadores(IEnumerable<LutadorModel> lutadoresSelecionados)
        {
            //Ordena os lutadores por idade (crescente)
            IOrderedEnumerable<LutadorModel> lutadoresOrdenados = lutadoresSelecionados.OrderBy(lutador => lutador.idade);

            //Realiza divisão, cada grupo tem 5 lutadores
            List<GrupoModel> grupoLutadores = new List<GrupoModel>();
            int count = 1;

            foreach (LutadorModel lutador in lutadoresOrdenados)
            {
                GrupoModel grupo = new GrupoModel();

                if (count >= 1 && count <= 4)
                {
                    grupo.grupo = 1;
                }
                else if (count >= 5 && count <= 10)
                {
                    grupo.grupo = 2;
                }
                else if (count >= 11 && count <= 15)
                {
                    grupo.grupo = 3;
                }
                else if (count >= 16 && count <= 20)
                {
                    grupo.grupo = 4;
                }
                else
                {
                    break;
                }

                grupo.lutador = lutador;

                grupoLutadores.Add(grupo);

                count++;
            }

            return grupoLutadores.ToList<GrupoModel>();

        }

        /// <summary>
        /// Realiza a fase de grupos dos lutadores selecionados
        /// </summary>
        /// <param name="grupoLutadores"> lutadores selecionados</param>
        /// <returns> retorna os 2 ganhadores de cada grupo</returns>
        private static IEnumerable<GrupoModel> FasedeGrupos(IEnumerable<GrupoModel> grupoLutadores)
        {
            //Armazena os ganhadores da fase de grupo
            List<GrupoModel> ganhadoresGrupoLutadores = new List<GrupoModel>();

            //Seleciona o grupo 1, ordena por numero de vitorias e retorna os 2 primeiros
            IEnumerable<GrupoModel> classiicacaoPrimeiroGrupo = grupoLutadores.Where(g => g.grupo == 1);
            IEnumerable<GrupoModel> vencedoresPrimeiroGrupo = classiicacaoPrimeiroGrupo.OrderByDescending(g => g.lutador.percentualVitorias).ThenByDescending(g => g.lutador.artesMarciais.Count()).ThenByDescending(g => g.lutador.lutas).Take(2);
            ganhadoresGrupoLutadores = insereGrupo(vencedoresPrimeiroGrupo, ganhadoresGrupoLutadores, 1, true).ToList<GrupoModel>();
            
            //Seleciona o grupo 2, ordena por numero de vitorias e retorna os 2 primeiros
            IEnumerable<GrupoModel> classiicacaoSegundoGrupo = grupoLutadores.Where(g => g.grupo == 2);
            IEnumerable<GrupoModel> vencedoresSegundoGrupo = classiicacaoSegundoGrupo.OrderByDescending(g => g.lutador.percentualVitorias).ThenByDescending(g => g.lutador.artesMarciais.Count()).ThenByDescending(g => g.lutador.lutas).Take(2);
            ganhadoresGrupoLutadores = insereGrupo(vencedoresSegundoGrupo, ganhadoresGrupoLutadores, 2, true).ToList<GrupoModel>();

            //Seleciona o grupo 3, ordena por numero de vitorias e retorna os 2 primeiros
            IEnumerable<GrupoModel> classiicacaoTerceiroGrupo = grupoLutadores.Where(g => g.grupo == 3);
            IEnumerable<GrupoModel> vencedoresTerceiroGrupo = classiicacaoSegundoGrupo.OrderByDescending(g => g.lutador.percentualVitorias).ThenByDescending(g => g.lutador.artesMarciais.Count()).ThenByDescending(g => g.lutador.lutas).Take(2);
            ganhadoresGrupoLutadores = insereGrupo(vencedoresTerceiroGrupo, ganhadoresGrupoLutadores, 3, true).ToList<GrupoModel>();

            //Seleciona o grupo 4, ordena por numero de vitorias e retorna os 2 primeiros
            IEnumerable<GrupoModel> classiicacaoQuartoGrupo = grupoLutadores.Where(g => g.grupo == 4);
            IEnumerable<GrupoModel> vencedoresQuartoGrupo = classiicacaoSegundoGrupo.OrderByDescending(g => g.lutador.percentualVitorias).ThenByDescending(g => g.lutador.artesMarciais.Count()).ThenByDescending(g => g.lutador.lutas).Take(2);
            ganhadoresGrupoLutadores = insereGrupo(vencedoresQuartoGrupo, ganhadoresGrupoLutadores, 4, true).ToList<GrupoModel>();

            return ganhadoresGrupoLutadores;

        }

        /// <summary>
        /// Realiza as quartas de final
        /// </summary>
        /// <param name="grupoLutadores">lutadores ganhadores da fase de grupos </param>
        /// <returns> retorna os vencedores das quartas de final</returns>
        private static IEnumerable<GrupoModel> QuartasdeFinal(IEnumerable<GrupoModel> grupoLutadores)
        {

            List<GrupoModel> ganhadoresQuartas = new List<GrupoModel>();

            //1A x 2B - Jogo 1
            IEnumerable<GrupoModel> primeiroColocadoGrupoA = grupoLutadores.Where(g => g.grupo == 1 && g.posicao == 1);
            IEnumerable<GrupoModel> segundoColocadoGrupoB = grupoLutadores.Where(g => g.grupo == 2 && g.posicao == 2);
            ganhadoresQuartas = insereGrupo(Disputa(primeiroColocadoGrupoA, segundoColocadoGrupoB), ganhadoresQuartas, 1, true).ToList<GrupoModel>();

            //2A x 1B - Jogo 2
            IEnumerable<GrupoModel> segundoColocadoGrupoA = grupoLutadores.Where(g => g.grupo == 1 && g.posicao == 2);
            IEnumerable<GrupoModel> primeiroColocadoGrupoB = grupoLutadores.Where(g => g.grupo == 2 && g.posicao == 1);
            ganhadoresQuartas = insereGrupo(Disputa(segundoColocadoGrupoA, primeiroColocadoGrupoB), ganhadoresQuartas, 2, true).ToList<GrupoModel>();

            //1C x 2D - Jogo 3
            IEnumerable<GrupoModel> primeiroColocadoGrupoC = grupoLutadores.Where(g => g.grupo == 3 && g.posicao == 1);
            IEnumerable<GrupoModel> segundoColocadoGrupoD = grupoLutadores.Where(g => g.grupo == 4 && g.posicao == 2);
            ganhadoresQuartas = insereGrupo(Disputa(primeiroColocadoGrupoC, segundoColocadoGrupoD), ganhadoresQuartas, 3, true).ToList<GrupoModel>();

            //2C x 1D - Jogo 4
            IEnumerable<GrupoModel> segundoColocadoGrupoC = grupoLutadores.Where(g => g.grupo == 3 && g.posicao == 2);
            IEnumerable<GrupoModel> primeirpColocadoGrupoD = grupoLutadores.Where(g => g.grupo == 4 && g.posicao == 1);
            ganhadoresQuartas = insereGrupo(Disputa(segundoColocadoGrupoC, primeirpColocadoGrupoD), ganhadoresQuartas, 4, true).ToList<GrupoModel>();

            //retorna os ganhadores 
            return ganhadoresQuartas;

        }

        /// <summary>
        /// Realiza a semi final
        /// </summary>
        /// <param name="grupoLutadores">lutadores ganhadores das quartas de final</param>
        /// <returns>retorna os vencedores da semi final</returns>
        private static IEnumerable<GrupoModel> SemiFinal(IEnumerable<GrupoModel> grupoLutadores)
        {

            List<GrupoModel> ganhadoresSemiFinal = new List<GrupoModel>();

            //separa os lutadores para realizar a semifinal
            IEnumerable<GrupoModel> vencedorJogo1 = grupoLutadores.Where(g => g.grupo == 1 && g.posicao == 1);
            IEnumerable<GrupoModel> vencedorJogo2 = grupoLutadores.Where(g => g.grupo == 2 && g.posicao == 1);
            IEnumerable<GrupoModel> vencedorJogo3 = grupoLutadores.Where(g => g.grupo == 1 && g.posicao == 2);
            IEnumerable<GrupoModel> vencedorJogo4 = grupoLutadores.Where(g => g.grupo == 2 && g.posicao == 2);
            
            //Jogo 1 x Jogo 2
            ganhadoresSemiFinal = insereGrupo(Disputa(vencedorJogo1, vencedorJogo2), ganhadoresSemiFinal,  1, true).ToList<GrupoModel>();

            //Jogo 3 x Jogo 4
            ganhadoresSemiFinal = insereGrupo(Disputa(vencedorJogo3, vencedorJogo4), ganhadoresSemiFinal, 2, true).ToList<GrupoModel>();

            //retorna os ganhadores 
            return ganhadoresSemiFinal;

        }

        /// <summary>
        /// Realiza a Final
        /// </summary>
        /// <param name="grupoLutadores">lutadores ganhadores das semi final</param>
        /// <returns>retorna os vencedores da final</returns>
        private static IEnumerable<GrupoModel> Final(IEnumerable<GrupoModel> grupoLutadores)
        {

            List<GrupoModel> ganhadoresFinal = new List<GrupoModel>();

            IEnumerable<GrupoModel> primeiroLugar = grupoLutadores.Where(g => g.grupo == 1 && g.posicao == 1);
            IEnumerable<GrupoModel> segundoLugar = grupoLutadores.Where(g => g.grupo == 1 && g.posicao == 2);
            IEnumerable<GrupoModel> terceiroLugar = grupoLutadores.Where(g => g.grupo == 2 && g.posicao == 1);
            ganhadoresFinal = insereGrupo(primeiroLugar, ganhadoresFinal, 1, false, 1).ToList<GrupoModel>();
            ganhadoresFinal = insereGrupo(segundoLugar, ganhadoresFinal, 1, false, 2).ToList<GrupoModel>();
            ganhadoresFinal = insereGrupo(terceiroLugar, ganhadoresFinal, 1, false, 3).ToList<GrupoModel>();

            return ganhadoresFinal;
        }

        #endregion

        #region Auxiliar Methods

        private static IEnumerable<GrupoModel> insereGrupo(IEnumerable<GrupoModel> lutadores, List<GrupoModel> grupoLutadores, int qualgrupo, bool inserePosicao = false, int posicao = 0)
        {
            int count = 1;
            foreach (GrupoModel lutador in lutadores)
            {
                GrupoModel grupo = new GrupoModel();
                grupo.grupo = qualgrupo;
                grupo.lutador = lutador.lutador;

                if (inserePosicao)
                {
                    grupo.posicao = count;
                    count++;
                }
                if (posicao>0)
                {
                    grupo.posicao = posicao;
                }

                grupoLutadores.Add(grupo);
            }

            return grupoLutadores;

        }

        private static IEnumerable<GrupoModel> Disputa(IEnumerable<GrupoModel> primeiroLutador, IEnumerable<GrupoModel> segundoLutador)
        {
            //Insere os lutadores em um grupo
            List<GrupoModel> disputa = new List<GrupoModel>();
            disputa = insereGrupo(primeiroLutador, disputa, 1, false).ToList<GrupoModel>();
            disputa = insereGrupo(segundoLutador, disputa, 1, false).ToList<GrupoModel>();

            //Realiza a disputa
            IEnumerable<GrupoModel> resultadoDisputa = disputa.OrderByDescending(g => g.lutador.percentualVitorias).ThenByDescending(g => g.lutador.artesMarciais.Count()).ThenByDescending(g => g.lutador.lutas).Take(2);
            List<GrupoModel> ganhadoresDisputa = new List<GrupoModel>();
            ganhadoresDisputa = insereGrupo(resultadoDisputa, ganhadoresDisputa, 1, true).ToList<GrupoModel>();

            return ganhadoresDisputa;

        }

        #endregion

    }
}