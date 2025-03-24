using System.Collections.Generic;

namespace Rendu1
{
    public class Noeud
    {
        /// Identifiant unique pour chaque noeud
        public int Id { get; set; }
  
        /// Liste des noeuds adjacents au noeud
        public List<Noeud> Voisins { get; set; }
        
        /// Indique si le noeud a été visité ou non
        public bool Visite { get; set; }

        public Noeud(int id)
        {
            Id = id;
            Voisins = new List<Noeud>();
            Visite = false;
        }
        public override string ToString()
        {
            return $"Noeud {Id}";
        }
    }
} 
