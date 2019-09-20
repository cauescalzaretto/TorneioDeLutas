using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreModels
{
    [DataContract]
    public class LutadorModel
    {
        [BindProperty]
        [DataMember(Name = "Id")]
        public int id { get; set; }

        [BindProperty]
        [DataMember(Name = "Nome")]
        public string nome { get; set; }

        [DataMember(Name = "Idade")]
        public int idade { get; set; }

        [DataMember(Name = "ArtesMarciais")]
        public IList<string> artesMarciais { get; set; }

        [DataMember(Name = "Lutas")]
        public int lutas { get; set; }

        [DataMember(Name = "Derrotas")]
        public int derrotas { get; set; }

        [DataMember(Name = "Vitorias")]
        public int vitorias { get; set; }

        [DataMember(Name = "percentualVitorias")]
        public int percentualVitorias { get { return (int)System.Math.Truncate((decimal)((decimal)vitorias / (decimal)lutas) * 100); } }

        [DataMember(Name = "selecionado")]
        public bool selecionado { get; set; }

    }
}
