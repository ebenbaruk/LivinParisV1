using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Rendu1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Graphe graphe = new Graphe();    
            ChargerDonnees(graphe);
            AnalyserGraphe(graphe);
            VisualiserGraphe(graphe);
        }

        /// Charge les données du fichier et construit le graphe
        private static void ChargerDonnees(Graphe graphe)
        {
            // Obtenir le chemin du répertoire de l'exécutable
            string repertoireExecution = AppDomain.CurrentDomain.BaseDirectory;
            // Remonter jusqu'au répertoire du projet 
            string repertoireProjet = Path.GetFullPath(Path.Combine(repertoireExecution, "..\\..\\..\\"));
            // Combiner avec le chemin relatif du fichier
            string fichier = Path.Combine(repertoireProjet, "Association-soc-karate", "soc-karate.mtx");
            
            Console.WriteLine($"Lecture de {fichier}");
            if (!File.Exists(fichier))
            {
                Console.WriteLine($"Le fichier {fichier} n'existe pas.");
                return;
            }

            // Lecture du fichier
            string[] lignes = File.ReadAllLines(fichier);
            Console.WriteLine($"Nombre de lignes lues : {lignes.Length}");
            
            // identification de tous les noeuds
            List<int> noeudsUniques = IdentifierNoeuds(lignes);
            Console.WriteLine($"Nombre de noeuds trouvés : {noeudsUniques.Count}");

            // Ajouter tous les noeuds au graphe
            foreach (int id in noeudsUniques)
            {
                graphe.AjouterNoeud(id);
            }

            // ajoute les liens
            int nombreLiens = AjouterLiens(graphe, lignes);
            Console.WriteLine($"Nombre de liens ajoutés : {nombreLiens}");
        }
        /// Identifie tous les noeuds uniques dans le fichier
        private static List<int> IdentifierNoeuds(string[] lignes)
        {
            List<int> noeudsUniques = new List<int>();
            bool lectureCommencee = false;

            foreach (string ligne in lignes)
            {
                // Ignore les lignes de commentaire au début du fichier
                if (ligne.StartsWith("%")) continue;
                
                // Ignorer la première ligne non-commentaire qui ne sont pas utile
                if (!lectureCommencee)
                {
                    lectureCommencee = true;
                    continue;
                }

                string[] elements = ligne.Trim().Split(' ');
                if (elements.Length >= 2)
                {
                    int source = int.Parse(elements[0]);
                    int destination = int.Parse(elements[1]);
                    if (!noeudsUniques.Contains(source))
                    {
                        noeudsUniques.Add(source);
                    }
                    if (!noeudsUniques.Contains(destination))
                    {
                        noeudsUniques.Add(destination);
                    }
                }
            }

            return noeudsUniques;
        }

        /// Ajoute les liens au graphe à partir des données du fichier
        private static int AjouterLiens(Graphe graphe, string[] lignes)
        {
            int nombreLiens = 0;
            bool lectureCommencee = false;
            
            foreach (string ligne in lignes)
            {
                // Ignorer les lignes de commentaire
                if (ligne.StartsWith("%")) continue;
                
                // Ignorer la première ligne non-commentaire non utile
                if (!lectureCommencee)
                {
                    lectureCommencee = true;
                    continue;
                }

                string[] elements = ligne.Trim().Split(' ');
                if (elements.Length >= 2)
                {
                    int source = int.Parse(elements[0]);
                    int destination = int.Parse(elements[1]);
                    graphe.AjouterLien(source, destination);
                    nombreLiens++;
                }
            }

            return nombreLiens;
        }
        /// Analyse les propriétés du graphe et affiche les résultats
        private static void AnalyserGraphe(Graphe graphe)
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════╗");
            Console.WriteLine("║     ANALYSE DU GRAPHE - ASSOCIATION KARATE     ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            
            // Caractéristiques principales
            Console.WriteLine("\nLES CARACTÉRISTIQUES PRINCIPALES:");
            Console.WriteLine($"• Ordre: {graphe.ObtenirOrdre()} noeuds");
            Console.WriteLine($"• Taille: {graphe.ObtenirTaille()} liens");
            Console.WriteLine($"• Type: {(graphe.EstOriente ? "Orienté" : "Non orienté")}, {(graphe.EstPondere ? "Pondéré" : "Non pondéré")}");
            
            // Propriétés structurelles
            Console.WriteLine("\nPROPRIÉTÉS DU GRAPHE:");
            Console.WriteLine($"• Densité: {graphe.CalculerDensite():F3} ({graphe.CalculerDensite()*100:F1}%)");
            Console.WriteLine($"• Degré: min={graphe.CalculerDegreMinimum()}, moyen={graphe.CalculerDegreMoyen():F2}, max={graphe.CalculerDegreMaximum()}");
            Console.WriteLine($"• Connexité: {(graphe.EstConnexe() ? "Graphe connexe" : "Graphe non connexe")}");
            Console.WriteLine($"• Cycles: {(graphe.ContientCycles() ? "Présents" : "Absents")}");
            Console.WriteLine($"• Complétude: {(graphe.EstComplet() ? "Graphe complet" : "Graphe non complet")}");
            
            // Parcours
            Console.WriteLine("\nPARCOURS À PARTIR DU NOEUD 1:");
            
            Console.WriteLine("• Parcours en largeur:");
            List<int> parcoursLargeur = graphe.ParcoursLargeur(1);
            AfficherParcours(parcoursLargeur);

            Console.WriteLine("\n• Parcours en profondeur:");
            List<int> parcoursProfondeur = graphe.ParcoursProfondeur(1);
            AfficherParcours(parcoursProfondeur);
        }
        
        /// Affiche un parcourrs de graphe
        private static void AfficherParcours(List<int> parcours)
        {
            for (int i = 0; i < parcours.Count; i += 10)
            {
                // Prendr au maximum 10 éléments à partir de la pos de départ
                List<int> groupe = new List<int>();
                for (int j = i; j < i + 10 && j < parcours.Count; j++)
                {
                    groupe.Add(parcours[j]);
                }
                
                Console.WriteLine(string.Join(" -> ", groupe));
            }
        }
        private static void VisualiserGraphe(Graphe graphe)
        {
            string fichierImage = "graphe.png";
            Console.WriteLine("\nVISUALISATION DU GRAPHE");
            Console.WriteLine($"Génération de l'image dans '{fichierImage}'..."); 
            VisualisationGraphe visualisation = new VisualisationGraphe(graphe);
            visualisation.Dessiner(fichierImage);
            
            Console.WriteLine($"Visualisation sauvegardée avec succès.");
        }
    }
}
