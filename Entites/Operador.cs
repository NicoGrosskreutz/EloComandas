using SQLite;
using System;

namespace EloComandas.Entites
{
    [Table("CG_OPERADOR")]
    public class Operador
    {
        [PrimaryKey, Column("USROPER"), MaxLength(10)]
        public string USROPER { get; set; }
        public string NOMOPER { get; set; }
        public string DSCFUNC { get; set; }
        public string SNHOPER { get; set; }
        public string DSCEMAIL { get; set; }
        public string DSCSENHA { get; set; }
        public DateTime DTHULTAT { get; set; }
        public string USRULTAL { get; set; }
    }
}