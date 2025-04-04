using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD
{
    internal class Plat {
        public string Id { get; set; }
        public string Nom { get; set; }
        public string Type { get; set; }
        public decimal Prix { get; set; }
        public int Quantite { get; set; }
        public string Categorie { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DatePeremption { get; set; }
    }
}
