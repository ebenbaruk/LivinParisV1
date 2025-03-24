using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace Rendu1
{
    /// <summary>
    /// Classe permettant de visualiser un graphe sous forme d'image
    /// </summary>
    public class VisualisationGraphe
    {
        /// <summary>
        /// Graphe à visualiser
        /// </summary>
        public Graphe Graphe;
        
        /// <summary>
        /// Positions des noeuds sur l'image
        /// </summary>
        public Dictionary<int, SKPoint> Positions;
        
        // Constantes de configuration
        private const float RAYON_NOEUD = 20;
        private const float LARGEUR = 800;
        private const float HAUTEUR = 600;
        private const float MARGE = 50;

        /// <summary>
        /// Crée un nouvel outil de visualisation pour le graphe spécifié
        /// </summary>
        /// <param name="graphe">Graphe à visualiser</param>
        public VisualisationGraphe(Graphe graphe)
        {
            this.Graphe = graphe;
            this.Positions = new Dictionary<int, SKPoint>();
            CalculerPositions();
        }

        /// <summary>
        /// Calcule les positions des noeuds pour l'affichage
        /// </summary>
        private void CalculerPositions()
        {
            var (ordre, _) = Graphe.ObtenirProprietes();
            double angle = 0;
            double angleIncrement = 2 * Math.PI / ordre;
            
            // Rayon du cercle sur lequel placer les noeuds
            double rayon = Math.Min(LARGEUR - 2*MARGE, HAUTEUR - 2*MARGE) * 0.4;
            
            // Positionner les noeuds en cercle
            foreach (Noeud noeud in Graphe.Noeuds)
            {
                // Calcul des coordonnées en fonction de l'angle
                float x = (float)(LARGEUR/2 + rayon * Math.Cos(angle));
                float y = (float)(HAUTEUR/2 + rayon * Math.Sin(angle));
                
                Positions[noeud.Id] = new SKPoint(x, y);
                angle += angleIncrement;
            }
        }

        /// <summary>
        /// Dessine le graphe et sauvegarde l'image dans un fichier
        /// </summary>
        /// <param name="cheminFichier">Chemin du fichier où sauvegarder l'image</param>
        public void Dessiner(string cheminFichier)
        {
            // Création de la surface de dessin
            using (SKSurface surface = SKSurface.Create(new SKImageInfo((int)LARGEUR, (int)HAUTEUR)))
            {
                SKCanvas canvas = surface.Canvas;

                // Fond blanc
                canvas.Clear(SKColors.White);

                // Dessiner les liens
                using (SKPaint peintureLien = new SKPaint())
                {
                    peintureLien.Color = SKColors.Gray;
                    peintureLien.StrokeWidth = 2;
                    peintureLien.IsAntialias = true;

                    foreach (Lien lien in Graphe.Liens)
                    {
                        SKPoint debut = Positions[lien.Source.Id];
                        SKPoint fin = Positions[lien.Destination.Id];
                        canvas.DrawLine(debut, fin, peintureLien);
                    }
                }

                // Dessiner les noeuds
                using (SKPaint peintureNoeud = new SKPaint())
                {
                    peintureNoeud.Color = SKColors.RoyalBlue;
                    peintureNoeud.IsAntialias = true;

                    using (SKPaint peintureTexte = new SKPaint())
                    {
                        peintureTexte.Color = SKColors.White;
                        peintureTexte.TextSize = 16;
                        peintureTexte.IsAntialias = true;
                        peintureTexte.TextAlign = SKTextAlign.Center;

                        foreach (KeyValuePair<int, SKPoint> paire in Positions)
                        {
                            int id = paire.Key;
                            SKPoint position = paire.Value;
                            canvas.DrawCircle(position, RAYON_NOEUD, peintureNoeud);
                            canvas.DrawText(id.ToString(), position.X, position.Y + 6, peintureTexte);
                        }
                    }
                }

                // Sauvegarder l'image
                using (SKImage image = surface.Snapshot())
                {
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        using (FileStream stream = File.OpenWrite(cheminFichier))
                        {
                            data.SaveTo(stream);
                        }
                    }
                }
            }
        }
    }
} 