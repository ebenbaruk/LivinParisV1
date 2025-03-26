namespace Rendu1
{
    public class Lien<T>
    {
        /// Noeud source du lien
        public Noeud<T> Source { get; set; }
        
        /// Noeud destination du lien
        public Noeud<T> Destination { get; set; }

        /// Indique si le lien est orienté
        public bool EstOriente { get; set; }
        
        /// Poids du lien (pour les graphes pondérés)
        public double Poids { get; set; }
        
        /// Temps de parcours en minutes
        public double TempsParcours { get; set; }

        public Lien(Noeud<T> source, Noeud<T> destination, bool estOriente = false, double poids = 1.0, double tempsParcours = 2.0)
        {
            Source = source;
            Destination = destination;
            EstOriente = estOriente;
            Poids = poids;
            TempsParcours = tempsParcours;
        }

        /// Vérifie si le lien contient un noeud spécifique
        public bool ContientNoeud(Noeud<T> noeud)
        {
            return Source.Id == noeud.Id || Destination.Id == noeud.Id;
        }
        
        /// Retourne l'autre extrémité du lien
        public Noeud<T> ObtenirAutreExtremite(Noeud<T> noeud)
        {
            if (Source.Id == noeud.Id)
                return Destination;
            if (Destination.Id == noeud.Id)
                return Source;
            return null;
        }
        
        public override string ToString()
        {
            string symbole = EstOriente ? " -> " : " -- ";
            return $"Lien: {Source.Id}{symbole}{Destination.Id} (temps: {TempsParcours} min, poids: {Poids})";
        }
    }
    
    // Gardons l'ancienne classe non générique pour la compatibilité avec le code existant
    public class Lien
    {
        /// Noeud source du lien
        public Noeud Source { get; set; }
        
        /// Noeud destination du lien
        public Noeud Destination { get; set; }

        /// Indique si le lien est orienté
        public bool EstOriente { get; set; }
        
        /// Poids du lien (pour les graphes pondérés)
        public double Poids { get; set; }

        public Lien(Noeud source, Noeud destination, bool estOriente = false, double poids = 1.0)
        {
            Source = source;
            Destination = destination;
            EstOriente = estOriente;
            Poids = poids;
        }

        /// Vérifie si le lien contient un noeud spécifique (pour savoir après si il a déjà été visité)
        public bool ContientNoeud(Noeud noeud)
        {
            return Source.Id == noeud.Id || Destination.Id == noeud.Id;
        }
        
        /// Retourne l'autre extrémité du lien
        public Noeud ObtenirAutreExtremite(Noeud noeud)
        {
            if (Source.Id == noeud.Id)
                return Destination;
            if (Destination.Id == noeud.Id)
                return Source;
            return null;
        }
        
        public override string ToString()
        {
            string symbole = EstOriente ? " -> " : " -- ";
            string infoPoids = Poids != 1.0 ? $" (poids: {Poids})" : "";
            return $"Lien: {Source.Id}{symbole}{Destination.Id}{infoPoids}";
        }
    }
} 
