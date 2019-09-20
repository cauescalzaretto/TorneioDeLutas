using System.Runtime.Serialization;

namespace CoreModels
{
    [DataContract]
    public class GrupoModel
    {

        [DataMember(Name = "grupo")]
        public int grupo { get; set; }

        [DataMember(Name = "posicao")]
        public int posicao { get; set; }

        [DataMember(Name = "Lutador")]
        public LutadorModel lutador { get; set; }


    }


}
