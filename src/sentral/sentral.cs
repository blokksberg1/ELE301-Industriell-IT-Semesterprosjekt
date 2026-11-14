
using Microsoft.Data.Sqlite;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SENTRAL
{
    internal class sentral
    {
        enum Side { startside, adminbruker, adminkort, rapport, avslutt }

        static void Main(string[] args)
        {
            // database oppsett
            string databaseMappe = Path.Combine(AppContext.BaseDirectory, "database");
            Directory.CreateDirectory(databaseMappe);

            string dbPath = Path.Combine(databaseMappe, "adgangskontroll.db");
            string connectionString = $"Data Source={dbPath}";

            if (!File.Exists(dbPath))
            {
                string sql = File.ReadAllText(
                    Path.Combine(databaseMappe, "adgangskontroll_kommentert.sql"));

                using (SqliteConnection connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
            }


            // starter serveren vår som bakgrunnstråd.

            Thread bk = new Thread(() => backgrunnsKommunikasjon(connectionString));
            bk.IsBackground = true;
            bk.Start();

            Side tilstand = Side.startside;
            string kommando = "";

            while (true)
            {
                Console.Clear();
                switch (tilstand)
                {
                    case Side.startside:
                        
                        Console.WriteLine("-------- SENTRAL --------\n" +
                                          "       |Hovedmeny|      \n\n" +
                                          "Skriv inn kommando:\n" +
                                          "1: Administrer brukere\n2: Administer kortleserer\n3: Vis rapporter\n4. Avslutt");
                        kommando = Console.ReadLine();

                        if (kommando == "1")
                        { tilstand = Side.adminbruker; }
                        else if (kommando == "2")
                        { tilstand = Side.adminkort; }
                        else if (kommando == "3")
                        { tilstand = Side.rapport; }
                        else if (kommando == "4")
                        { tilstand = Side.avslutt; }
                        break;


                    case Side.adminbruker:

                        Console.WriteLine("-------- SENTRAL --------\n" +
                                          "        |Brukere|      \n\n" +
                                          "Skriv inn kommando:\n" +
                                          "1: Legg til bruker\n2. Fjern bruker \n" +
                                          "3: Rediger eksisterende bruker\n4: Hent brukerinfo\n" +
                                          "5: Startside"
                                          );
                        kommando = Console.ReadLine();

                        if (kommando == "1")
                        {
                            leggtilBruker(connectionString);
                            tilstand = Side.startside;
                        }
                        else if (kommando == "2")
                        {
                            slettBruker(connectionString);

                            Console.WriteLine("bruker er nå slettet");
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }
                        else if (kommando == "3")
                        {
                            RedigerBruker(connectionString);
                            tilstand = Side.startside;
                        }
                        else if (kommando == "4")
                        {
                            HentUtBrukerInfo(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }
                        else if (kommando == "5")
                        { tilstand = Side.startside; }
                        break;

                    case Side.adminkort:

                        Console.WriteLine("-------- SENTRAL --------\n" +
                                          "      |Kortlesere|      \n\n" +
                                          "Skriv inn kommando:\n" +
                                          "1: Legg til kortleser\n2: Fjern kortleser \n" +
                                          "3: Rediger eksisterende kortleser\n4: Hent kortleserinfo\n" +
                                          "5: Startside"
                                          );
                        kommando = Console.ReadLine();

                        if (kommando == "1")
                        {
                            leggTilKortleser(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }
                        else if (kommando == "2")
                        {
                            slettKortleser(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }
                        else if (kommando == "3")
                        {
                            redigerKortleser(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }
                        else if (kommando == "4")
                        {
                            hentUtKortleserInfo(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }
                        break;

                    case Side.rapport:

                        Console.WriteLine("-------- SENTRAL --------\n" +
                                          "       |Rapporter|      \n\n" +
                                          "Skriv inn kommando:\n" +
                                          "1: Startside");
                        kommando = Console.ReadLine();

                        if (kommando == "1")
                        { tilstand = Side.startside; }
                        break;
                }
            }
        }

        // bruker admin kommandoer
        static void slettBruker(string connectionString)
        {
            // hvem sletter vi
            Console.Clear();
            Console.WriteLine("Hva er primærnøkkelen:");
            string primekey = Console.ReadLine();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Brukar WHERE brukar_id = @id";
                    command.Parameters.AddWithValue("@id", primekey);
                    command.ExecuteNonQuery();
                }
            }
        }

        static void leggtilBruker(string connectionString)
        {
            // oppgiing av al info
            Console.Clear();
            Console.WriteLine("Hva er bruker id:");
            string primekey = Console.ReadLine();

            Console.WriteLine("Hva er kort ID:");
            string kortid = Console.ReadLine();

            Console.WriteLine("Hva er fornavnet:");
            string fornavnet = Console.ReadLine();

            Console.WriteLine("Hva er etternavnet:");
            string etternavnet = Console.ReadLine();

            Console.WriteLine("Hva er eposten:");
            string epost = Console.ReadLine();

            Console.WriteLine("Hva er pinkoden:");
            string pinkode = Console.ReadLine();

            // lagring av info i databasen
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Brukar
    (brukar_id, kort_id, fornavn, etternavn, epost, pin)
    VALUES (@id, @kortid, @fornavn, @etternavn, @epost, @pin)";

                    command.Parameters.AddWithValue("@id", primekey);
                    command.Parameters.AddWithValue("@kortid", kortid);
                    command.Parameters.AddWithValue("@fornavn", fornavnet);
                    command.Parameters.AddWithValue("@etternavn", etternavnet);
                    command.Parameters.AddWithValue("@epost", epost);
                    command.Parameters.AddWithValue("@pin", pinkode);

                    command.ExecuteNonQuery();
                }
            }
        }

        static void RedigerBruker(string connectionString)
        {
            // hvem vil vi redigere
            Console.Clear();
            Console.WriteLine("hvem vil du redigerer");
            string brukerid = Console.ReadLine();

            // hva vil du redigerer
            Console.WriteLine("hva vil du redigere idag:\n" +
            "1: fornavn\n" +
            "2: etternavn\n" +
            "3: epost\n" +
            "4: pin\n");

            string svar = Console.ReadLine();
            string nytekst = "";
            string kolonne = "";

            // redigerering
            if (svar == "1")
            {
                Console.Clear();
                Console.WriteLine("Hva vil du det nye navnet skal være:");
                nytekst = Console.ReadLine();
                kolonne = "fornavn";
            }
            else if (svar == "2")
            {
                Console.Clear();
                Console.WriteLine("Hva vil du det nye navnet skal være:");
                nytekst = Console.ReadLine();
                kolonne = "etternavn";
            }
            else if (svar == "3")
            {
                Console.Clear();
                Console.WriteLine("Hva vil du den nye eposten skal være:");
                nytekst = Console.ReadLine();
                kolonne = "epost";
            }
            else if (svar == "4")
            {
                Console.Clear();
                Console.WriteLine("Hva vil du den nye pinkoden skal være:");
                nytekst = Console.ReadLine();
                kolonne = "pin";
            }

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Brukar SET {kolonne} = @nytekst WHERE brukar_id = @id";

                    command.Parameters.AddWithValue("@nytekst", nytekst);
                    command.Parameters.AddWithValue("@id", brukerid);

                    command.ExecuteNonQuery();
                }
            }
        }

        static void HentUtBrukerInfo(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("vær vennlig og oppgi bruker til person du vil ha info om:");
            string kortid = Console.ReadLine();
            skrivUtBrukerInfo(kortid, connectionString);
        }

        static void skrivUtBrukerInfo(string id, string connectionString)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Brukar WHERE brukar_id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        Console.WriteLine("Fornavn: " + reader["fornavn"]);
                        Console.WriteLine("Etternavn: " + reader["etternavn"]);
                        Console.WriteLine("Epost: " + reader["epost"]);
                        Console.WriteLine("KortID: " + reader["kort_id"]);
                        Console.WriteLine("PIN: " + reader["pin"]);
                    }
                }
            }
        }

        // kortleser admin kommandoer
        static void leggTilKortleser(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("hva er kortleseriden?:");
            string kortleserid = Console.ReadLine();

            Console.WriteLine("hvilke rom nummer har kortleserene?:");
            string romnummer = Console.ReadLine();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Kortleser
            (kortlesar_nr, romnummer)
            VALUES (@kortleserid, @romnummer)";

                    command.Parameters.AddWithValue("@kortleserid", kortleserid);
                    command.Parameters.AddWithValue("@romnummer", romnummer);

                    command.ExecuteNonQuery();
                }
            }
        }

        static void slettKortleser(string connectionString)
        {
            // hvem sletter vi
            Console.Clear();
            Console.WriteLine("Hva er primærnøkkelen:");
            string primekey = Console.ReadLine();

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM Kortleser WHERE kortlesar_nr = @kortleserid";
                    command.Parameters.AddWithValue("@kortleserid", primekey);

                    command.ExecuteNonQuery();
                }
            }
        }

        static void redigerKortleser(string connectionString)
        {
            // hvem redigers og hva
            Console.Clear();
            Console.WriteLine("hvem vil du redigerer");
            string kortleserid = Console.ReadLine();

            Console.WriteLine("hva vil du at rommet skal være");
            string romnummer = Console.ReadLine();

            // database oppdatering
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Kortleser SET romnummer = @romnummer WHERE kortlesar_nr = @id";

                    command.Parameters.AddWithValue("@romnummer", romnummer);
                    command.Parameters.AddWithValue("@id", kortleserid);

                    command.ExecuteNonQuery();
                }
            }
        }

        static void hentUtKortleserInfo(string connectionString)
        {
            Console.Clear();
            Console.WriteLine("vær vennlig og oppgi kortleserId-en til kortleseren du vil ha info om:");
            string kortid = Console.ReadLine();
            skrivUtKortleserInfo(kortid, connectionString);
        }

        static void skrivUtKortleserInfo(string kortleserid, string connectionString)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Kortleser WHERE kortlesar_nr = @Kortleserid";
                    command.Parameters.AddWithValue("@Kortleserid", kortleserid);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();

                        Console.WriteLine("KortleserId: " + reader["kortlesar_nr"]);
                        Console.WriteLine("Romnummer: " + reader["romnummer"]);
                    }
                }
            }
        }

        // funksjoner som endrer på kortleser logger
        static bool sjekkAvKort(string pin, string kortid, string connectionString)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT EXISTS (SELECT 1 FROM Brukar WHERE kort_id = @kortID AND pin = @pin)";
                    command.Parameters.AddWithValue("@kortID", kortid);
                    command.Parameters.AddWithValue("@pin", pin);

                    long resultat = (long)command.ExecuteScalar();

                    if (resultat == 1) { return true; }
                    else { return false; }
                }
            }
        }

        static void Loggføring(string connectionString, string melding, string kortleser, string kort, int godkjent)
        {
            string tidspunkt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                // henter ut bruker id
                string brukerid = null;

                using (SqliteCommand finnBruker = connection.CreateCommand())
                {
                    finnBruker.CommandText = "SELECT brukar_id FROM Brukar WHERE kort_id = @kortid";
                    finnBruker.Parameters.AddWithValue("@kortid", kort);

                    brukerid = finnBruker.ExecuteScalar() as string;
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Adgangsforsøk
    (brukar_id, kortlesar_nr, kort_id, tidspunkt, godkjent)
    VALUES (@brukerid, @kortleser, @kortid, @tidspunkt, @godkjent)";

                    command.Parameters.AddWithValue("@brukerid", (object)brukerid ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kortleser", kortleser);
                    command.Parameters.AddWithValue("@kortid", kort);
                    command.Parameters.AddWithValue("@tidspunkt", tidspunkt);
                    command.Parameters.AddWithValue("@godkjent", godkjent);

                    command.ExecuteNonQuery();
                }
            }
        }

        static void LoggføringsValg(bool hvaskjer, string kortId, string kortleserId, string connectionString)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                int godkjent = 0;
                connection.Open();

                if (hvaskjer == true) // hastag godstemmning
                {
                    godkjent = 1;
                    Loggføring(connectionString, "Godkjent", kortleserId, kortId, godkjent);
                }
                else if (hvaskjer == false) // dårlig stemmning
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT EXISTS (SELECT 1 FROM Brukar WHERE kort_id = @kortID)";
                        command.Parameters.AddWithValue("@kortID", kortId);

                        long resultat = (long)command.ExecuteScalar();

                        if (resultat == 1)
                        {
                            Loggføring(connectionString, "ikke Godkjent, feil pin", kortleserId, kortId, godkjent);
                        }
                        else
                        {
                            Loggføring(connectionString, "ikke Godkjent, ugyldig kort", kortleserId, kortId, godkjent);
                        }
                    }
                }
            }
        }

        static void backgrunnsKommunikasjon(string connectionString)
        {
            // oppsett av variabler osm skal burkes
            int recv;
            byte[] data = new byte[1024];
            string mottattTekst;
            string sendtTekst;

            // lager en server
            Socket lytteSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);
            lytteSokkel.Bind(serverEP);
            lytteSokkel.Listen(10);

            while (true)
            {
                // oppkobling til klient
                Socket kommSokkel = lytteSokkel.Accept();
                IPEndPoint klientEP = (IPEndPoint)kommSokkel.RemoteEndPoint;

                // sender data til klient
                string hilsen = "tilkobling til sentral vellyket";
                data = Encoding.ASCII.GetBytes(hilsen);
                kommSokkel.Send(data, data.Length, SocketFlags.None);


                while (true) // håndtering av kortleser status iguess
                {
                    // mottar informasjon fra klienten
                    data = new byte[1024];
                    int nMottatt = kommSokkel.Receive(data);

                    Console.WriteLine($"Antall bytes mottatt: {nMottatt}");
                    if (nMottatt == 0) // hopper ut av indre while løkke hvis kobling brutes
                    {
                        break;
                    }

                    // gjør mottatt informasjon om til tekst
                    mottattTekst = Encoding.ASCII.GetString(data, 0, nMottatt);
                    Console.WriteLine($"Mottatt: [{mottattTekst}]");

                    // deler opp teksten ved semikolon
                    string[] informasjon = mottattTekst.Split(';');

                    // lagrer KortID og PIN i egne variabler
                    string kortID = informasjon[0];
                    string pin = informasjon[1];
                    string kortleserId = informasjon[2];
                    

                    bool svar = sjekkAvKort(pin, kortID, connectionString);

                    if (svar == true)
                    {
                        Console.WriteLine("godkjent mofo");
                        bool godkjentEllerIkke = true;
                        data = BitConverter.GetBytes(godkjentEllerIkke);
                        kommSokkel.Send(data, data.Length, SocketFlags.None);
                    }
                    else
                    {
                        Console.WriteLine("ikke godkjent mofo");
                        bool godkjentEllerIkke = false;
                        data = BitConverter.GetBytes(godkjentEllerIkke);
                        kommSokkel.Send(data, data.Length, SocketFlags.None);
                        
                    }

                    LoggføringsValg(svar, kortID, kortleserId, connectionString);
                }
                // avslutter forbindelsen
                kommSokkel.Close();
            }
        }
    }
}
