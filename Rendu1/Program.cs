using System;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace Rendu1
{
    class Program
    {
        // Couleurs pour la console
        private static readonly ConsoleColor CouleurTitre = ConsoleColor.Cyan;
        private static readonly ConsoleColor CouleurSousTitre = ConsoleColor.Yellow;
        private static readonly ConsoleColor CouleurTexte = ConsoleColor.White;
        private static readonly ConsoleColor CouleurErreur = ConsoleColor.Red;
        private static readonly ConsoleColor CouleurSucces = ConsoleColor.Green;
        private static readonly ConsoleColor CouleurInfo = ConsoleColor.Blue;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            AfficherTitre("SYSTÈME DE NAVIGATION MÉTRO PARISIEN");
            
            // Chemin vers le fichier CSV contenant les données du métro
            string cheminBase = AppDomain.CurrentDomain.BaseDirectory;
            string cheminFichierMetro = Path.Combine(cheminBase, "DataMetro", "metro.csv");
            
            // Vérifier si le fichier de données existe
            if (!File.Exists(cheminFichierMetro))
            {
                AfficherErreur($"Le fichier {cheminFichierMetro} n'existe pas.");
                AfficherTexte("Veuillez vérifier que le dossier DataMetro contient le fichier metro.csv.");
                AfficherTexte("\nAppuyez sur une touche pour quitter...");
                Console.ReadKey();
                return;
            }

            // Chargement des données du métro parisien
            AfficherInfo("\nChargement des données du métro parisien...");
            MetroParisien metroParisien = new MetroParisien(cheminFichierMetro);
            
            bool continuer = true;
            
            while (continuer)
            {
                // Recherche d'itinéraire
                if (RechercherItinerairePersonnalisé(metroParisien) == false)
                {
                    continuer = false;
                }
                
                if (continuer)
                {
                    AfficherSeparateur();
                    AfficherTexte("Appuyez sur Entrée pour rechercher un nouvel itinéraire");
                    AfficherTexte("ou sur 'q' pour quitter");
                    AfficherSeparateur();
                    
                    string choix = Console.ReadLine() ?? "";
                    if (choix.ToLower() == "q")
                    {
                        continuer = false;
                    }
                }
            }
            
            AfficherTexte("\nMerci d'avoir utilisé le système de navigation du métro parisien.");
        }
        
        /// <summary>
        /// Permet à l'utilisateur de rechercher un itinéraire personnalisé
        /// </summary>
        /// <returns>true pour continuer, false pour quitter</returns>
        static bool RechercherItinerairePersonnalisé(MetroParisien metroParisien)
        {
            AfficherSousTitre("RECHERCHE D'ITINÉRAIRE PERSONNALISÉ");
            
            // Sélection de la station de départ
            string stationDepart = SélectionnerStation(metroParisien, "départ");
            if (string.IsNullOrEmpty(stationDepart))
                return false;
            
            // Sélection de la station d'arrivée
            string stationArrivee = SélectionnerStation(metroParisien, "arrivée");
            if (string.IsNullOrEmpty(stationArrivee))
                return false;
            
            // Éviter de rechercher un trajet vers la même station
            if (stationDepart == stationArrivee)
            {
                AfficherErreur("\nLes stations de départ et d'arrivée sont identiques.");
                return true;
            }
            
            AfficherSeparateur();
            AfficherTexte($"Recherche d'itinéraire de {stationDepart} à {stationArrivee}");
            AfficherSeparateur();
            
            // Recherche du plus court chemin
            metroParisien.TrouverPlusCourtChemin(stationDepart, stationArrivee);
            
            return true;
        }
        
        /// <summary>
        /// Permet à l'utilisateur de sélectionner une station existante
        /// </summary>
        /// <param name="metroParisien">Le système métro</param>
        /// <param name="type">Le type de station (départ ou arrivée)</param>
        /// <returns>Le nom de la station sélectionnée ou null si l'utilisateur annule</returns>
        static string SélectionnerStation(MetroParisien metroParisien, string type)
        {
            while (true)
            {
                AfficherTexte($"\nEntrez le nom (ou une partie) de la station de {type} (ou 'q' pour quitter): ");
                string? recherche = Console.ReadLine();
                
                if (recherche?.ToLower() == "q")
                    return string.Empty;
                
                if (string.IsNullOrWhiteSpace(recherche) || recherche.Length < 2)
                {
                    AfficherErreur("Veuillez entrer au moins 2 caractères pour la recherche.");
                    continue;
                }

                // Rechercher toutes les stations qui contiennent la chaîne de recherche (insensible à la casse)
                var correspondances = metroParisien.StationsParNom.Keys
                    .Where(nom => nom.IndexOf(recherche, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(nom => nom)
                    .ToList();
                
                if (correspondances.Count == 0)
                {
                    AfficherErreur("\nAucune station ne correspond à votre recherche. Essayez avec un autre terme.");
                    continue;
                }
                
                // Si une seule correspondance exacte est trouvée, la sélectionner automatiquement
                if (correspondances.Count == 1 || correspondances.Contains(recherche, StringComparer.OrdinalIgnoreCase))
                {
                    string stationExacte = correspondances.FirstOrDefault(s => s.Equals(recherche, StringComparison.OrdinalIgnoreCase)) ?? correspondances.First();
                    AfficherSucces($"\nStation sélectionnée: {stationExacte}");
                    return stationExacte;
                }
                
                // Afficher les résultats de la recherche
                AfficherTexte($"\n{correspondances.Count} stations trouvées pour '{recherche}':");
                
                // Limiter l'affichage à 15 résultats maximum
                int maxResultats = Math.Min(15, correspondances.Count);
                for (int i = 0; i < maxResultats; i++)
                {
                    AfficherTexte($"{i + 1}. {correspondances[i]}");
                }
                
                if (correspondances.Count > maxResultats)
                {
                    AfficherTexte($"... et {correspondances.Count - maxResultats} autres stations.");
                    AfficherTexte("Veuillez affiner votre recherche pour voir plus de résultats.");
                }
                
                AfficherTexte("\nEntrez le numéro de la station désirée (ou 0 pour refaire la recherche): ");
                string? choixStr = Console.ReadLine();
                if (string.IsNullOrEmpty(choixStr) || !int.TryParse(choixStr, out int choix) || choix < 0 || choix > maxResultats)
                {
                    AfficherErreur("Choix invalide. Veuillez réessayer.");
                    continue;
                }
                
                if (choix == 0)
                    continue;
                
                return correspondances[choix - 1];
            }
        }

        #region Méthodes d'affichage

        private static void AfficherTitre(string titre)
        {
            Console.ForegroundColor = CouleurTitre;
            Console.WriteLine("\n" + new string('=', titre.Length + 4));
            Console.WriteLine($"= {titre} =");
            Console.WriteLine(new string('=', titre.Length + 4) + "\n");
            Console.ForegroundColor = CouleurTexte;
        }

        private static void AfficherSousTitre(string sousTitre)
        {
            Console.ForegroundColor = CouleurSousTitre;
            Console.WriteLine("\n" + new string('-', sousTitre.Length + 4));
            Console.WriteLine($"- {sousTitre} -");
            Console.WriteLine(new string('-', sousTitre.Length + 4) + "\n");
            Console.ForegroundColor = CouleurTexte;
        }

        private static void AfficherSeparateur()
        {
            Console.ForegroundColor = CouleurInfo;
            Console.WriteLine(new string('=', 50));
            Console.ForegroundColor = CouleurTexte;
        }

        private static void AfficherTexte(string texte)
        {
            Console.ForegroundColor = CouleurTexte;
            Console.WriteLine(texte);
        }

        private static void AfficherErreur(string message)
        {
            Console.ForegroundColor = CouleurErreur;
            Console.WriteLine(message);
            Console.ForegroundColor = CouleurTexte;
        }

        private static void AfficherSucces(string message)
        {
            Console.ForegroundColor = CouleurSucces;
            Console.WriteLine(message);
            Console.ForegroundColor = CouleurTexte;
        }

        private static void AfficherInfo(string message)
        {
            Console.ForegroundColor = CouleurInfo;
            Console.WriteLine(message);
            Console.ForegroundColor = CouleurTexte;
        }

        #endregion
    }
}
