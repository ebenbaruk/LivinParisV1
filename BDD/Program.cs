using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Reflection.PortableExecutable;

namespace BDD
{
    internal class Program
    {
        static void Requete(string requete) {
            // Paramétrage et ouverture d'une connexion avec l'objet MySqlConnection 
            string connectionString = @"
                                    SERVER=127.0.0.1;
                                    PORT=3306;
                                    DATABASE=livinparis;
                                    UID=root;
                                    PASSWORD=1234";

            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                try {
                    connect.Open();

                    // Création de l'objet MySqlCommand avec la requête associée
                    MySqlCommand commande = connect.CreateCommand();
                    commande.CommandText = requete;

                    // Récupération des résultats dans un objet MySqlDataReader
                    using (MySqlDataReader reader = commande.ExecuteReader()) {

                        int fieldCount = reader.FieldCount; //Donne les nombre de colonnes demandées

                        // On convertit chaque colonne en un string dans un tableau de string
                        string[] columnNames = new string[fieldCount];

                        for (int i = 0; i < fieldCount; i++) {
                            columnNames[i] = reader.GetName(i);
                        }

                        // En-tete
                        Console.WriteLine(string.Join(" | ", columnNames));
                        Console.WriteLine("------------------------------");

                        // Lecture des données ligne par ligne
                        while (reader.Read()) {
                            string[] valeurs = new string[fieldCount];
                            for (int i = 0; i < fieldCount; i++) {
                                valeurs[i] = reader[i].ToString();
                            }
                            Console.WriteLine(string.Join(" | ", valeurs));
                        }
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            } // `using` s'assure que la connexion est fermée automatiquement
        }
        static bool VerifU(string email, string mdp) {
            bool res = false;
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                try {
                    connect.Open();
                    string requete = "SELECT COUNT(*) FROM Utilisateur WHERE EmailU = @Email AND MDPU = @Mdp";
                    MySqlCommand commande = new MySqlCommand(requete, connect);
                    //Pour éviter les attaques par injection
                    commande.Parameters.AddWithValue("@Email", email);
                    commande.Parameters.AddWithValue("@Mdp", mdp);
                    int count = Convert.ToInt32(commande.ExecuteScalar());
                    if( count > 0) { res = true; }
                } catch (Exception ex) {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
                return res;
            }
        }
        static void CreationU() {
            #region Paramètres
            Console.Write("Entrez votre nom : ");
            string nom = Console.ReadLine();
            Console.Write("Entrez votre prénom : ");
            string prenom = Console.ReadLine();
            Console.Write("Entrez votre rue : ");
            string rue = Console.ReadLine();
            Console.Write("Entrez votre numéro : ");
            int numero = int.Parse(Console.ReadLine());
            Console.Write("Entrez votre code postal : ");
            int codePostal = int.Parse(Console.ReadLine());
            Console.Write("Entrez votre ville : ");
            string ville = Console.ReadLine();
            Console.Write("Entrez votre téléphone : ");
            string telephone = Console.ReadLine();
            Console.Write("Entrez votre email : ");
            string email = Console.ReadLine();
            Console.Write("Entrez la station la plus proche : ");
            string station = Console.ReadLine();
            Console.Write("Entrez votre mot de passe : ");
            string mdp = Console.ReadLine();
            Console.Write("Entrez votre âge : ");
            int age = int.Parse(Console.ReadLine());
            Console.Write("Entrez votre régime alimentaire : ");
            string regime = Console.ReadLine();
            Console.Write("Entrez vos préférences : ");
            string preference = Console.ReadLine();
            Console.Write("Entrez vos allergies : ");
            string allergie = Console.ReadLine();
            Console.Write("Entrez votre budget : ");
            double budget = double.Parse(Console.ReadLine());
            #endregion

            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                try {
                    connect.Open();
                    string requete = $"INSERT INTO Utilisateur (NomU, PrenomU, RueU, NumeroU, CodePostalU, VilleU, TelephoneU, " +
                        $"EmailU, StationPlusProcheU, MDPU, AgeU, RegimeAlimentaireU, PreferenceU, AllergieU, BudgetU) " +
                        $"VALUES ('{nom}', '{prenom}', '{rue}', {numero}, {codePostal}, '{ville}', '{telephone}', '{email}', " +
                        $"'{station}', '{mdp}', {age}, '{regime}', '{preference}', '{allergie}', {budget})";
                    MySqlCommand commande = new MySqlCommand(requete, connect);
                    int result = commande.ExecuteNonQuery();
                    if (result > 0) {
                        Console.WriteLine("Inscription réussie ! Vous pouvez maintenant vous connecter.");
                    } else {
                        Console.WriteLine("Erreur lors de l'inscription. Veuillez réessayer.");
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Erreur : " + ex.Message);
                }
            }
        }
        static int GetClientId(string email) {
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();
                string requete = "SELECT ClientID FROM Utilisateur WHERE EmailU = @Email";
                MySqlCommand commande = new MySqlCommand(requete, connect);
                commande.Parameters.AddWithValue("@Email", email);
                return Convert.ToInt32(commande.ExecuteScalar());
            }
        }

        static bool VerifC(int clientId) {
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();
                string requete = "SELECT COUNT(*) FROM Cuisinier WHERE ClientID = @ClientId";
                MySqlCommand commande = new MySqlCommand(requete, connect);
                commande.Parameters.AddWithValue("@ClientId", clientId);
                return Convert.ToInt32(commande.ExecuteScalar()) > 0;
            }
        }

        static void InterfaceCuisinier(int clientId) {
            //On verif si le cuisinier a une spécialité
            string specialite = GetSpecialite(clientId);

            if (string.IsNullOrEmpty(specialite)) {
                Console.WriteLine("\nVous n'avez pas encore renseigné votre spécialité culinaire.");
                Console.Write("Veuillez entrer votre spécialité (ex: Italienne, Française, etc.) : ");
                specialite = Console.ReadLine();
                SetSpecialite(clientId, specialite);
                Console.WriteLine("Spécialité enregistrée avec succès !");
            }

            Console.WriteLine($"\nInterface Cuisinier - Spécialité: {specialite}");
            // Ici vous ajouterez les fonctionnalités cuisinier
        }

        static void InterfaceClient(int clientId) {
            Console.WriteLine("\nInterface Client");

            while (true) {
                Console.WriteLine("\nMenu Principal :");
                Console.WriteLine("1. Passer une commande");
                Console.WriteLine("2. Consulter mes commandes");
                Console.WriteLine("3. Devenir cuisinier");
                Console.WriteLine("4. Se déconnecter");
                Console.Write("Votre choix : ");

                string choix = Console.ReadLine();

                switch (choix) {
                    case "1":
                        PasserCommande(clientId);
                        break;
                    case "2":
                        ConsulterCommandes(clientId);
                        break;
                    case "3":
                        CreationCuisinier(clientId);
                        break;
                    case "4":
                        Console.WriteLine("Déconnexion réussie.");
                        return;
                    default:
                        Console.WriteLine("Choix invalide.");
                        break;
                }
            }
        }
        /// <summary>
        /// Permet de créer un cuisinier pour client qui souhaite en devnir un
        /// </summary>
        /// <param name="clientId"></param>
        static void CreationCuisinier(int clientId) {
            //Verif si le user est déjà cuisinier
            if (VerifC(clientId)) {
                Console.WriteLine("\nVous êtes déjà enregistré comme cuisinier.");
                string specialite = GetSpecialite(clientId);
                Console.WriteLine($"Votre spécialité actuelle : {specialite}");

                Console.Write("Voulez-vous modifier votre spécialité ? (O/N) : ");
                string modifier = Console.ReadLine().ToUpper();

                if (modifier == "O") {
                    Console.Write("Entrez votre nouvelle spécialité : ");
                    string nouvelleSpe = Console.ReadLine();
                    SetSpecialite(clientId, nouvelleSpe);
                    Console.WriteLine("Spécialité mise à jour avec succès !");
                }
                return;
            }

            Console.WriteLine("\nDevenir cuisinier :");
            Console.WriteLine("En tant que cuisinier, vous pourrez proposer vos plats à la communauté.");
            Console.Write("Voulez-vous vraiment devenir cuisinier ? (O/N) : ");
            string confirmation = Console.ReadLine().ToUpper();

            if (confirmation == "O") {
                Console.Write("Entrez votre spécialité culinaire (ex: Italienne, Française, etc.) : ");
                string specialite = Console.ReadLine();

                SetSpecialite(clientId, specialite);
                Console.WriteLine("\nFélicitations ! Vous êtes maintenant cuisinier.");
                Console.WriteLine($"Spécialité enregistrée : {specialite}");
            } else {
                Console.WriteLine("Opération annulée.");
            }
        }
        /// <summary>
        /// Récupère la spécialité d'un cuisinier
        /// Sert à savoir si un user a un compte CUISINIER
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        static string GetSpecialite(int clientId) {
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();
                string requete = "SELECT SpecialiteC FROM Cuisinier WHERE ClientID = @ClientId";
                MySqlCommand commande = new MySqlCommand(requete, connect);
                commande.Parameters.AddWithValue("@ClientId", clientId);
                object result = commande.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        static void SetSpecialite(int clientId, string specialite) {
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString))
            {
                connect.Open();

                //Verif d'abord si l'utilisateur est déjà cuisinier
                string verifString = "SELECT COUNT(*) FROM Cuisinier WHERE ClientID = @ClientId";
                MySqlCommand verif = new MySqlCommand(verifString, connect);
                verif.Parameters.AddWithValue("@ClientId", clientId);
                bool exists = Convert.ToInt32(verif.ExecuteScalar()) > 0;

                string requete;
                if (exists) {
                    requete = "UPDATE Cuisinier SET SpecialiteC = @Specialite WHERE ClientID = @ClientId";
                } else {
                    requete = "INSERT INTO Cuisinier (ClientID, SpecialiteC) VALUES (@ClientId, @Specialite)";
                }

                MySqlCommand commande = new MySqlCommand(requete, connect);
                commande.Parameters.AddWithValue("@ClientId", clientId);
                commande.Parameters.AddWithValue("@Specialite", specialite);
                commande.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Algo pour qu'un user passe une commande
        /// </summary>
        /// <param name="clientId"></param>
        static void PasserCommande(int clientId) {
            Console.WriteLine("\n=== PASSER UNE COMMANDE ===");

            //Etape 1 : On affiche les cuisiniers dispo
            Console.WriteLine("\nCuisiniers disponibles :");
            Dictionary<int, string> cuisiniers = GetCuisiniers();

            //Cas où il n'y a pas de cuisinier dispo
            if (cuisiniers.Count == 0) {
                Console.WriteLine("Aucun cuisinier avec plats disponibles.");
                return;
            }

            //Affichage des cuisiniers (s'il en existe)
            foreach (var c in cuisiniers) {
                Console.WriteLine($"{c.Key}. {c.Value}");
            }

            //Etape 2 : Le user sélectionne un cuisinier
            Console.Write("\nChoisissez un cuisinier (numéro) : ");
            //Verif si le numéro est conforme
            if (!int.TryParse(Console.ReadLine(), out int cuisinierId) || !cuisiniers.ContainsKey(cuisinierId)) {
                Console.WriteLine("Choix invalide.");
                return;
            }

            //Etape 3 : On affiche les plats du cuisinier choisis
            Console.WriteLine($"\nPlats disponibles chez {cuisiniers[cuisinierId]} :");
            //Ici, on utilise une classe pour les plats (puisqu'il peut y en avoir plusieurs)
            Dictionary<string, Plat> plats = GetPlats(cuisinierId);

            if (plats.Count == 0) {
                Console.WriteLine("Aucun plat disponible pour ce cuisinier.");
                return;
            }

            //Afichage détaillé des plats
            foreach (var p in plats) {
                Console.WriteLine($"{p.Key}. {p.Value.Nom} - {p.Value.Prix}€ ({p.Value.Quantite} disponibles)");
                Console.WriteLine($"Type: {p.Value.Type} | Catégorie: {p.Value.Categorie}");
                //"ToShortDateString" convertit le format DateTime en un string lisible pour nous
                Console.WriteLine($"Date conception: {p.Value.DateCreation.ToShortDateString()}");
                Console.WriteLine($"Date de péremption: {p.Value.DatePeremption.ToShortDateString()}\n");
            }

            //Etape 4: On sélectionne les plats
            Dictionary<string, int> platsCommandes = new Dictionary<string, int>();
            
            bool continuer = true; //Permet de ferme la boucle quand on veut

            while (continuer) {
                Console.Write("Choisissez un plat (ID) ou 0 pour terminer : ");
                string platId = Console.ReadLine();

                //Arrêt de la prise de nouveaux plats
                if (platId == "0") {
                    continuer = false;
                    continue;
                }

                //Cas où l'ID n'est pas conforme (pas inclut ou pas bon format)
                if (!plats.ContainsKey(platId)) {
                    Console.WriteLine("ID de plat invalide.");
                    continue;
                }

                //Cas où le plat est valide. Dans ce cas, on chosiit la quantité.

                //Cas où la quantité entrée n'est pas conforme 
                Console.Write($"Quantité souhaitée (max {plats[platId].Quantite}) : ");
                if (!int.TryParse(Console.ReadLine(), out int quantite) || quantite <= 0 || quantite > plats[platId].Quantite) {
                    Console.WriteLine("Quantité invalide.");
                    continue;
                }

                platsCommandes.Add(platId, quantite);
                Console.WriteLine($"Ajouté: {quantite}x {plats[platId].Nom}");
            }

            //Cas où le client ne choisit pas de plat
            if (platsCommandes.Count == 0) {
                Console.WriteLine("Aucun plat sélectionné.");
                return;
            }

            //Etape 5: Finalisation de la commande
            Console.WriteLine("\nRécapitulatif de la commande :");
            decimal total = 0;
            //Calcul du prix à payer
            foreach (var pc in platsCommandes) {
                decimal prixPlat = plats[pc.Key].Prix * pc.Value;
                Console.WriteLine($"{pc.Value}x {plats[pc.Key].Nom} - {prixPlat}€");
                total += prixPlat;
            }
            Console.WriteLine($"\nTOTAL: {total}€");

            Console.Write("\nConfirmer la commande ? (O/N) : ");
            //Le client ne souhaite finalement pas faire la commande
            if (Console.ReadLine().ToUpper() != "O") {
                Console.WriteLine("Commande annulée.");
                return;
            }

            //Etape 6 : On enregistre la commande dans la BDD
            try {
                string commandeId = EnregistrerCommande(clientId, cuisinierId, platsCommandes, plats, total);
                Console.WriteLine($"\nCommande enregistrée ! Numéro: {commandeId}");
            } catch (Exception ex) {
                Console.WriteLine($"Erreur: {ex.Message}");
            }
        }
        /// <summary>
        /// Permet de renvoyer les cuisiniers et les détails sur eux (nom, spécialité,...)
        /// </summary>
        /// <returns></returns>
        static Dictionary<int, string> GetCuisiniers() {
            var cuisiniers = new Dictionary<int, string>();
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";

            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();
                string requete = @"
            SELECT DISTINCT c.ClientID, u.PrenomU, u.NomU, c.SpecialiteC 
            FROM Cuisinier c
            JOIN Utilisateur u ON c.ClientID = u.ClientID
            JOIN Creer cr ON c.ClientID = cr.ClientID
            JOIN Plat p ON cr.PlatID = p.PlatID
            WHERE p.QuantitePlat > 0 AND p.DatePeremption >= CURDATE()";

                MySqlCommand cmd = new MySqlCommand(requete, connect);

                using (MySqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        int id = reader.GetInt32("ClientID");
                        string info = $"{reader.GetString("PrenomU")} {reader.GetString("NomU")} - {reader.GetString("SpecialiteC")}";
                        cuisiniers.Add(id, info);
                    }
                }
            }
            return cuisiniers;
        }
        /// <summary>
        /// Permet de récupérer les plats liés à un cuisinier
        /// </summary>
        /// <param name="cuisinierId"></param>
        /// <returns></returns>
        static Dictionary<string, Plat> GetPlats(int cuisinierId) {
            var plats = new Dictionary<string, Plat>();
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";

            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();
                string requete = @"
            SELECT p.PlatID, p.NomPlat, p.TypePlat, p.PrixParPersonne, 
                   p.QuantitePlat, p.CategorieAlimentaire, 
                   p.DateCreation, p.DatePeremption
            FROM Plat p
            JOIN Creer c ON p.PlatID = c.PlatID
            WHERE c.ClientID = @CuisinierId 
            AND p.QuantitePlat > 0
            AND p.DatePeremption >= CURDATE()";

                MySqlCommand cmd = new MySqlCommand(requete, connect);
                cmd.Parameters.AddWithValue("@CuisinierId", cuisinierId);

                using (MySqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        //On transfo le plat en une instance de la classe plat
                        Plat p = new Plat {
                            Id = reader.GetString("PlatID"),
                            Nom = reader.GetString("NomPlat"),
                            Type = reader.GetString("TypePlat"),
                            Prix = Convert.ToDecimal(reader.GetString("PrixParPersonne")),
                            Quantite = reader.GetInt32("QuantitePlat"),
                            Categorie = reader.GetString("CategorieAlimentaire"),
                            DateCreation = reader.GetDateTime("DateCreation"),
                            DatePeremption = reader.GetDateTime("DatePeremption")
                        };
                        plats.Add(p.Id, p);
                    }
                }
            } 
            return plats;
        }
        /// <summary>
        /// Cette méthode est utilisée pour enregistrer la commande faite par un client dans la BDD
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="cuisinierId"></param>
        /// <param name="platsCommandes"></param>
        /// <param name="plats"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        static string EnregistrerCommande(int clientId, int cuisinierId, Dictionary<string, int> platsCommandes, 
            Dictionary<string, Plat> plats, decimal total) {

            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";
            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();

                #region J'ai utilisé de l'IA pour cette partie
                //ID unique pour la commande
                string commandeId = "CMD-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
                #endregion

                //Utiliser le trajet par défaut (à changer dans le futur)
                string trajetId = "1"; 

                //Créer la commande dans BonDeCommande_Liv
                string reqCommande = @"
            INSERT INTO BonDeCommande_Liv 
            (CommandeID, PrixPaye, DateSouhaitee, AdresseBon, Statut, TrajetID, ClientID) 
            VALUES 
            (@CommandeId, @Total, NOW(), 
             (SELECT CONCAT(NumeroU, ' ', RueU, ', ', CodePostalU, ' ', VilleU) 
              FROM Utilisateur WHERE ClientID = @ClientId), 
             'En préparation', @TrajetId, @ClientId)";

                MySqlCommand cmdCommande = new MySqlCommand(reqCommande, connect);
                cmdCommande.Parameters.AddWithValue("@CommandeId", commandeId);
                cmdCommande.Parameters.AddWithValue("@Total", total);
                cmdCommande.Parameters.AddWithValue("@TrajetId", trajetId);
                cmdCommande.Parameters.AddWithValue("@ClientId", clientId);
                cmdCommande.ExecuteNonQuery();

                //On lie les plats à la commande via la table "Correspond"
                foreach (var pc in platsCommandes) {
                    string reqCorrespond = @"
                INSERT INTO Correspond 
                (PlatID, CommandeID) 
                VALUES 
                (@PlatId, @CommandeId)";

                    MySqlCommand cmdCorrespond = new MySqlCommand(reqCorrespond, connect);
                    cmdCorrespond.Parameters.AddWithValue("@PlatId", pc.Key);
                    cmdCorrespond.Parameters.AddWithValue("@CommandeId", commandeId);
                    cmdCorrespond.ExecuteNonQuery();

                    //On met à jour la quantité dispo
                    string reqUpdate = @"
                UPDATE Plat 
                SET QuantitePlat = QuantitePlat - @Quantite 
                WHERE PlatID = @PlatId";

                    MySqlCommand cmdUpdate = new MySqlCommand(reqUpdate, connect);
                    cmdUpdate.Parameters.AddWithValue("@Quantite", pc.Value);
                    cmdUpdate.Parameters.AddWithValue("@PlatId", pc.Key);
                    cmdUpdate.ExecuteNonQuery();
                }

                return commandeId;
            }
        }
        /// <summary>
        /// Cette méthode est utilisée dans l'interface client poour 
        /// qu'il puisse laisser des avis sur ses commandes passées.
        /// </summary>
        /// <param name="clientId"></param>
        static void ConsulterCommandes(int clientId) {
            Console.WriteLine("\n=== MES COMMANDES ===");

            //Etape 1: On trouve les commandes du user
            //On utilise une classe spécifique pour lier les commandes aux plats
            Dictionary<string, CommandePlat> commandes = GetCommandes(clientId);

            //Cas où aucune commande n'a été faite
            if (commandes.Count == 0) {
                Console.WriteLine("Vous n'avez aucune commande.");
                return;
            }

            //Etape 2 : Affichage des commandes
            foreach (var cmd in commandes) {
                Console.WriteLine($"\nCommande #{cmd.Key} du {cmd.Value.DateCommande.ToShortDateString()}");
                Console.WriteLine($"Statut: {cmd.Value.Statut}");
                Console.WriteLine("Plats commandés:");

                //On affiche les plats qui ont été commandés par le client
                foreach (var plat in cmd.Value.Plats) {
                    Console.WriteLine($"- {plat.Nom} ({plat.Prix}€)");

                    //Verif si un avis existe déjà (on peut pas faire plus d'un avis)
                    if (plat.AvisExiste) {
                        Console.WriteLine("Vous avez déjà noté ce plat !");
                    }
                }
            }

            //Etape 3 : Le client sélectionne une commande à noter
            Console.Write("\nEntrez le numéro de commande à noter (ou 0 pour annuler) : ");
            string commandeId = Console.ReadLine();

            if (commandeId == "0" || !commandes.ContainsKey(commandeId)) return;

            //Etape 4 : On affiche les plats à noter
            Console.WriteLine("\nPlats à noter :");
            var platsANoter = commandes[commandeId].Plats.Where(p => !p.AvisExiste).ToList();
            //Cherche les plats qui n'ont pas encore été notés

            //Si tous les plats ont été notés, la commande est déjà notée
            if (platsANoter.Count == 0) {
                Console.WriteLine("Tous les plats de cette commande ont déjà été notés.");
                return;
            }

            for (int i = 0; i < platsANoter.Count; i++) {
                Console.WriteLine($"{i + 1}. {platsANoter[i].Nom}");
            }

            //Etape 5 : Le client sélectionne un plat à noter
            Console.Write("Choisissez un plat à noter (numéro) : ");
            if (!int.TryParse(Console.ReadLine(), out int choixPlat) || choixPlat <= 0 || choixPlat > platsANoter.Count) {
                Console.WriteLine("Choix invalide.");
                return;
            }

            var platSelectionne = platsANoter[choixPlat - 1];

            //Etape 6 : Le client donne son avis
            Console.Write($"Note (0-5) pour {platSelectionne.Nom} : ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal note) || note < 0 || note > 5) {
                Console.WriteLine("Note invalide. Doit être entre 0 et 5.");
                return;
            }

            Console.Write("Commentaire (optionnel) : ");
            string commentaire = Console.ReadLine();

            //Etape 7 : On enregistre l'avis dans la BDD
            try {
                EnregistrerAvis(clientId, platSelectionne.PlatId, note, commentaire);
                Console.WriteLine("Merci pour votre avis !");
            } catch (Exception ex) {
                Console.WriteLine($"Erreur : {ex.Message}");
            }
        }
        /// <summary>
        /// Méthode qui utilise les classes CommandePlat et PlatAvis
        /// pour récuperer la liste des plats d'une commande en sachant s'il ont été notés
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        static Dictionary<string, CommandePlat> GetCommandes(int clientId) {
            var commandes = new Dictionary<string, CommandePlat>();
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";

            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();

                //Requête pour récupérer commandes avec plats
                string requete = @"
            SELECT b.CommandeID, b.DateSouhaitee, b.Statut, 
                   p.PlatID, p.NomPlat, p.PrixParPersonne,
                   CASE WHEN a.PlatID IS NULL THEN 0 ELSE 1 END AS AvisExiste
            FROM BonDeCommande_Liv b
            JOIN Correspond c ON b.CommandeID = c.CommandeID
            JOIN Plat p ON c.PlatID = p.PlatID
            LEFT JOIN Avis a ON p.PlatID = a.PlatID AND a.ClientID = @ClientId
            WHERE b.ClientID = @ClientId
            ORDER BY b.DateSouhaitee DESC";

                MySqlCommand cmd = new MySqlCommand(requete, connect);
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                using (MySqlDataReader reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        string commandeId = reader.GetString("CommandeID");

                        if (!commandes.ContainsKey(commandeId)) {
                            commandes.Add(commandeId, new CommandePlat {
                                CommandeId = commandeId,
                                DateCommande = reader.GetDateTime("DateSouhaitee"),
                                Statut = reader.GetString("Statut")
                            });
                        }

                        commandes[commandeId].Plats.Add(new PlatAvis
                        {
                            PlatId = reader.GetString("PlatID"),
                            Nom = reader.GetString("NomPlat"),
                            Prix = Convert.ToDecimal(reader.GetString("PrixParPersonne")),
                            AvisExiste = reader.GetInt32("AvisExiste") == 1
                        });
                    }
                }
            }
            return commandes;
        }
        /// <summary>
        /// Enregistre l'avis d'un client dans la BDD
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="platId"></param>
        /// <param name="note"></param>
        /// <param name="commentaire"></param>
        /// <exception cref="Exception"></exception>
        static void EnregistrerAvis(int clientId, string platId, decimal note, string commentaire) {
            string connectionString = @"SERVER=127.0.0.1;PORT=3306;DATABASE=livinparis;UID=root;PASSWORD=1234";

            using (MySqlConnection connect = new MySqlConnection(connectionString)) {
                connect.Open();

                //Verif si l'avis existe déja
                string checkQuery = "SELECT COUNT(*) FROM Avis WHERE ClientID = @ClientId AND PlatID = @PlatId";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, connect);
                checkCmd.Parameters.AddWithValue("@ClientId", clientId);
                checkCmd.Parameters.AddWithValue("@PlatId", platId);

                if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0) {
                    throw new Exception("Vous avez déjà noté ce plat.");
                }

                //Enregistrement du nouvel avis
                string insertQuery = @"
            INSERT INTO Avis 
            (ClientID, PlatID, Note, Commentaire, DateNotation) 
            VALUES 
            (@ClientId, @PlatId, @Note, @Commentaire, NOW())";

                MySqlCommand insertCmd = new MySqlCommand(insertQuery, connect);
                insertCmd.Parameters.AddWithValue("@ClientId", clientId);
                insertCmd.Parameters.AddWithValue("@PlatId", platId);
                insertCmd.Parameters.AddWithValue("@Note", note);
                insertCmd.Parameters.AddWithValue("@Commentaire", string.IsNullOrEmpty(commentaire) ? DBNull.Value : (object)commentaire);

                insertCmd.ExecuteNonQuery();
            }
        }
        static void Main(string[] args) {

            //Entrée dans l'application
            Console.WriteLine("Bienvenue sur Liv'InParis\n");
            Console.WriteLine("Connexion :");
            Console.WriteLine("Appuyez sur 1 si vous possédez déjà un compte.");
            Console.WriteLine("Appuyez sur 2 si vous souhaitez créer un compte.");
            Console.Write("Votre choix : ");

            string choix = Console.ReadLine();

            if (choix == "1") {
                Console.Write("Entrez votre email : ");
                string email = Console.ReadLine();
                Console.Write("Entrez votre mot de passe : ");
                string mdp = Console.ReadLine();

                if (VerifU(email, mdp)) {
                    int clientId = GetClientId(email);
                    Console.WriteLine("Connexion réussie ! Bienvenue sur Liv'InParis.");

                    //Veirfie si l'utilisateur est cuisinier
                    bool verifC = VerifC(clientId);

                    if (verifC) {
                        Console.WriteLine("\nVous êtes connecté en tant que CUISINIER.");
                        Console.WriteLine("1. Accéder à l'interface cuisinier");
                        Console.WriteLine("2. Accéder à l'interface client");
                        Console.Write("Votre choix : ");
                        //Chosir l'interface

                        string role = Console.ReadLine(); 
                        if (role == "1") {
                            InterfaceCuisinier(clientId);
                        } else {
                            InterfaceClient(clientId);
                        }
                    } else {
                        Console.WriteLine("\nVous êtes connecté en tant que CLIENT.");
                        InterfaceClient(clientId);
                    }
                } else {
                    Console.WriteLine("Identifiants incorrects. Veuillez réessayer.");
                }
            } else if (choix == "2") {
                CreationU();
            } else {
                Console.WriteLine("Choix invalide. Veuillez redémarrer l'application.");
            }


            Console.WriteLine();
            Console.WriteLine("Press any key to exit !");
            Console.ReadLine();

        }
    }
}