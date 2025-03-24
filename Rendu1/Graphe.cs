using System.Collections.Generic;

namespace Rendu1
{
    public class Graphe
    {
        public List<Noeud> Noeuds { get; private set; }
        public List<Lien> Liens { get; private set; }
        
        /// Matrice d'adjacence du graphe
        public bool[,] MatriceAdjacence;
        
        /// Liste d'adjacence du graphe
        public Dictionary<int, List<int>> ListeAdjacence;

        // Nouvelles propriétés
        public bool EstOriente { get; private set; }
        public bool EstPondere { get; private set; }

        /// Constructeur du graphe vide
        public Graphe(bool estOriente = false, bool estPondere = false)
        {
            Noeuds = new List<Noeud>();
            Liens = new List<Lien>();
            ListeAdjacence = new Dictionary<int, List<int>>();
            MatriceAdjacence = new bool[0, 0];
            EstOriente = estOriente;
            EstPondere = estPondere;
        }

        /// Ajoute un noeud au graphe avec l'identifiant unique
        public void AjouterNoeud(int id)
        {
            Noeuds.Add(new Noeud(id));
            ListeAdjacence[id] = new List<int>();
            MettreAJourMatrice();
        }

        /// Ajoute un lien entre deux noeuds identifiés par leurs ID
        public void AjouterLien(int sourceId, int destinationId)
        {
            Noeud? source = null;
            Noeud? destination = null;
            
            // Recherche des noeuds
            foreach (Noeud n in Noeuds)
            {
                if (n.Id == sourceId) source = n;
                if (n.Id == destinationId) destination = n;
            }

            if (source != null && destination != null)
            {
                Liens.Add(new Lien(source, destination));
                source.Voisins.Add(destination);
                destination.Voisins.Add(source);

                // Mise à jour de la liste/matrice d'adjacence
                if (!ListeAdjacence[sourceId].Contains(destinationId))
                    ListeAdjacence[sourceId].Add(destinationId);
                if (!ListeAdjacence[destinationId].Contains(sourceId))
                    ListeAdjacence[destinationId].Add(sourceId);
                
                MatriceAdjacence[sourceId - 1, destinationId - 1] = true;
                MatriceAdjacence[destinationId - 1, sourceId - 1] = true;
            }
        }

        /// Changement de la taille de la matrice d'adjacence
        private void MettreAJourMatrice()
        {
            int taille = Noeuds.Count;
            MatriceAdjacence = new bool[taille, taille];
        }

        public List<int> ParcoursLargeur(int depart)
        {
            List<int> resultat = new List<int>();
            Queue<Noeud> file = new Queue<Noeud>();
            List<int> visites = new List<int>();

            // Vérification du noeud de départ
            Noeud? noeudDepart = null;
            foreach (Noeud n in Noeuds)
            {
                if (n.Id == depart)
                {
                    noeudDepart = n;
                    break;
                }
            }
            
            if (noeudDepart == null) return resultat;

            file.Enqueue(noeudDepart);
            visites.Add(depart);

            while (file.Count > 0)
            {
                Noeud noeudCourant = file.Dequeue();
                resultat.Add(noeudCourant.Id);

                foreach (Noeud voisin in noeudCourant.Voisins)
                {
                    if (!visites.Contains(voisin.Id))
                    {
                        file.Enqueue(voisin);
                        visites.Add(voisin.Id);
                    }
                }
            }

            return resultat;
        }

        public List<int> ParcoursProfondeur(int depart)
        {
            List<int> resultat = new List<int>();
            List<int> visites = new List<int>();
            
            // Appel récursive
            DFS(depart, resultat, visites);
            
            return resultat;
        }
        
        /// Version récursive pour le parcours en profondeur
        private void DFS(int noeudId, List<int> resultat, List<int> visites)
        {
            if (!visites.Contains(noeudId))
            {
                visites.Add(noeudId);
                resultat.Add(noeudId);

                // Vérifier si la clé existe dans le dictionnaire avant d'y accéder
                if (ListeAdjacence.ContainsKey(noeudId))
                {
                    foreach (int voisinId in ListeAdjacence[noeudId])
                    {
                        DFS(voisinId, resultat, visites);
                    }
                }
            }
        }

        /// Vérifie si le graphe est connexe
        public bool EstConnexe()
        {
            if (Noeuds.Count == 0) return true;
            
            // Utiliser un parcours en largeur pour vérifier la connexité
            bool[] visite = new bool[Noeuds.Count];
            Queue<Noeud> file = new Queue<Noeud>();
            
            file.Enqueue(Noeuds[0]);
            visite[0] = true;
            
            while (file.Count > 0)
            {
                Noeud courant = file.Dequeue();
                
                foreach (Lien lien in Liens)
                {
                    if (lien.ContientNoeud(courant))
                    {
                        Noeud voisin = lien.ObtenirAutreExtremite(courant);
                        int indexVoisin = Noeuds.IndexOf(voisin);
                        
                        if (!visite[indexVoisin])
                        {
                            visite[indexVoisin] = true;
                            file.Enqueue(voisin);
                        }
                    }
                }
            }
            
            // Vérifier si tous les noeuds ont été visités
            foreach (bool v in visite)
            {
                if (!v) return false;
            }
            
            return true;
        }

        ///Vrai si le graphe contient au moins un cycle sinon faux
        public bool ContientCycles()
        {
            List<int> visites = new List<int>();
            
            foreach (Noeud noeud in Noeuds)
            {
                if (!visites.Contains(noeud.Id))
                {
                    List<int> enCours = new List<int>();
                    if (DetecterCycle(noeud.Id, -1, visites, enCours))
                        return true;
                }
            }

            return false;
        }
        
        private bool DetecterCycle(int noeudId, int parent, List<int> visites, List<int> enCours)
        {
            visites.Add(noeudId);
            enCours.Add(noeudId);

            foreach (int voisinId in ListeAdjacence[noeudId])
            {
                // Ignore le lien qui nous a amenés ici
                if (voisinId == parent) continue;
                
                // Si on retrouve un noeud, on a un cycle
                if (enCours.Contains(voisinId))
                    return true;
                
                // Continuer la recherche en profondeur
                if (!visites.Contains(voisinId))
                    if (DetecterCycle(voisinId, noeudId, visites, enCours))
                        return true;
            }

            // On a fini de visiter ce noeud
            enCours.Remove(noeudId);
            return false;
        }

        /// Calcule l'ordre et la taille du graphe
        public (int ordre, int taille) ObtenirProprietes()
        {
            return (Noeuds.Count, Liens.Count);
        }

        public double CalculerDensite()
        {
            int n = Noeuds.Count;
            if (n <= 1) return 0; // Pas de liens possibles avec 0 ou 1 noeud
            
            // Nombre maximum de liens dans un graphe non orienté : n(n-1)/2
            double maxLiens = n * (n - 1) / 2.0;
            return Liens.Count / maxLiens;
        }
        
        /// Calcule le degré moyen des noeuds du graphe
        public double CalculerDegreMoyen()
        {
            if (Noeuds.Count == 0) return 0;
            
            // La somme des degrés est égale à 2 fois le nombre de liens
            return (2.0 * Liens.Count) / Noeuds.Count;
        }

        /// Détermine le type du graphe
        public string DeterminerTypeGraphe()
        {
            // Le graphe est toujours non orienté et non pondéré ici
            return "Graphe non orienté et non pondéré";
        }

        /// Retourne l'ordre du graphe (nombre de noeuds)
        public int ObtenirOrdre()
        {
            return Noeuds.Count;
        }
        
        /// Retourne la taille du graphe (nombre de liens)
        public int ObtenirTaille()
        {
            return Liens.Count;
        }
        
        /// Calcule le degré maximum du graphe
        public int CalculerDegreMaximum()
        {
            int degreMax = 0;
            
            foreach (Noeud noeud in Noeuds)
            {
                int degre = 0;
                foreach (Lien lien in Liens)
                {
                    if (lien.ContientNoeud(noeud))
                    {
                        degre++;
                    }
                }
                
                if (degre > degreMax)
                {
                    degreMax = degre;
                }
            }
            
            return degreMax;
        }
        
        /// Calcule le degré minimum du graphe
        public int CalculerDegreMinimum()
        {
            if (Noeuds.Count == 0) return 0;
            
            int degreMin = int.MaxValue;
            
            foreach (Noeud noeud in Noeuds)
            {
                int degre = 0;
                foreach (Lien lien in Liens)
                {
                    if (lien.ContientNoeud(noeud))
                    {
                        degre++;
                    }
                }
                
                if (degre < degreMin)
                {
                    degreMin = degre;
                }
            }
            
            return degreMin;
        }
        
        /// Vérifie si le graphe est complet
        public bool EstComplet()
        {
            int n = Noeuds.Count;
            int nbLiensComplet = (n * (n - 1)) / 2;
            
            return Liens.Count == nbLiensComplet;
        }
    }
} 
