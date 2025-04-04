using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDD
{
    internal class CommandePlat {
        public string CommandeId { get; set; }
        public DateTime DateCommande { get; set; }
        public string Statut { get; set; }
        public List<PlatAvis> Plats { get; set; } = new List<PlatAvis>();
    }
}
