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




        enum Side { startside, adminbruker,adminkort, rapoort, avslutt }

        static void Main(string[] args)
        {

            // starter serveren vår som bakgrunnstråd.
            string connectionString = @"Data Source=C:\Users\trist\Documents\semester 5\semester prosjektet git\database\adgangskontroll.db";
            Thread bk = new Thread(() => backgrunnsKommunikasjon(connectionString));
            bk.IsBackground = true;
            bk.Start();



            Side tilstand = Side.startside;
            string komando = "";

            while (true)
            {
                
                switch (tilstand)
                {
                    case Side.startside:
                        Console.Clear();
                        Console.WriteLine("Vellkomen til sentralen hva har du lyst til og gjøre\n" +
                                          "1 Administrerebrukere, 2 administerekortleserer  3 rapporter, 4 avslutt");
                        komando = Console.ReadLine();
                        if (komando == "1")
                        { tilstand = Side.adminbruker; }
                        else if (komando == "2")
                        { tilstand = Side.adminkort; }
                        break;




                    case Side.adminbruker:
                        Console.Clear();
                        Console.WriteLine("Vellkomen til admin siden hva har du lyst til og gjøre\n" +
                                          "1: legge til en ny bruker, 2 slette en bruker \n" +
                                          "3: redigere eksisterende bruker, 4: gå tilbake til startide\n"+
                                          "5: hent ut bruker info");
                        komando = Console.ReadLine();
                        
                        
                        if (komando == "1")
                        {
                            leggtilBruker(connectionString);
                            tilstand = Side.startside; 
                        }
                   
                        else if (komando == "2")
                        {
                            slettBruker(connectionString);
                            Console.Clear();
                            Console.WriteLine("bruker er nå slettet");
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }


                        else if (komando == "3")
                        {
                            RedigerBruker(connectionString);
                            tilstand = Side.startside;
                        }

                        else if (komando == "4")
                        { tilstand = Side.startside; }

                        else if (komando == "5")
                        {
                            HentUtBrukerInfo(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside; 
                        }

                        break;


                    case Side.adminkort:
                        Console.Clear();
                        Console.WriteLine("hva har du lyst til og gjøre idag:\n" +
                                          "1. Legge til ny kortleser, 2. slette en kortleser\n" +
                                          "3. redigere en eksisterende kortleser, 4.hent ut info om en kortleser?");
                        komando = Console.ReadLine();
                        if (komando == "1") 
                        {
                            leggTilKortleser(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }


                        else if (komando == "2")
                        {
                            slettKortleser(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }

                        else if (komando == "3")
                        {
                            redigerKortleser(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }


                        else if (komando == "4")
                        {
                            hentUtKortleserInfo(connectionString);
                            Console.ReadKey();
                            tilstand = Side.startside;
                        }


                        break;
                        


                    case Side.rapoort:
                        Console.Clear();
                        Console.WriteLine("Vellkomen til raportsiden, hva har du lyst til og gjøre\n" +
                                          "1 gå tilbake til startside");
                        komando = Console.ReadLine();
                        if (komando == "1")
                        { tilstand = Side.startside; }
                        break;




                }



            }




        }

        //bruker admin kommandoer
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
                    command.CommandText = "DELETE FROM Bruker WHERE bruker_id = @id";
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
                    command.CommandText = @"INSERT INTO Bruker
    (bruker_id, kort_id, fornavn, etternavn, epost, pin)
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
                kolonne = "Fornavn";
            }



            else if (svar == "2")     
            {
                Console.Clear();
                Console.WriteLine("Hva vil du det nye navnet skal være:");
                nytekst = Console.ReadLine();
                kolonne = "Etternavn";

            }

            else if (svar == "3")
            {
                Console.Clear();
                Console.WriteLine("Hva vil du den nye eposten skal være:");
                nytekst = Console.ReadLine();
                kolonne = "Epost";

            }

             else if (svar == "4")
            {
                Console.Clear();
                Console.WriteLine("Hva vil du den nye pinkoden skal være:");
                nytekst = Console.ReadLine();
                kolonne = kolonne = "PIN";

            }

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"UPDATE Bruker SET {kolonne} = @nytekst WHERE bruker_id = @id";

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
                    command.CommandText = "SELECT * FROM Bruker WHERE bruker_id = @id";
                    command.Parameters.AddWithValue("@id", id);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        reader.Read();
                            
                        Console.WriteLine("Fornavn: " + reader["Fornavn"]);
                        Console.WriteLine("Etternavn: " + reader["Etternavn"]);
                        Console.WriteLine("Epost: " + reader["Epost"]);
                        Console.WriteLine("KortID: " + reader["Kort_id"]);
                        Console.WriteLine("PIN: " + reader["PIN"]);
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
            (kortleser_nr, romnummer)
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
                    command.CommandText = "DELETE FROM Kortleser WHERE kortleser_nr = @kortleserid";
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
                    command.CommandText = "UPDATE Kortleser SET romnummer = @romnummer WHERE kortleser_nr = @id";

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
                    command.CommandText = "SELECT * FROM Kortleser WHERE kortleser_nr = @Kortleserid";
                    command.Parameters.AddWithValue("@Kortleserid", kortleserid);

                    using (SqliteDataReader reader = command.ExecuteReader()) 
                    {
                        reader.Read();

                        Console.WriteLine("KortleserId: " + reader["Kortleser_nr"]);
                        Console.WriteLine("Romnummer: " + reader["Romnummer"]);


                        
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
                    command.CommandText = @"SELECT EXISTS (SELECT 1 FROM Bruker WHERE kort_id = @kortID AND PIN = @pin)";
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
                    finnBruker.CommandText = "SELECT bruker_id FROM Bruker WHERE kort_id = @kortid";
                    finnBruker.Parameters.AddWithValue("@kortid", kort);

                    brukerid = finnBruker.ExecuteScalar() as string;
                }



                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"INSERT INTO Adgangsforsøk
    (bruker_id, kortleser_nr, kort_id, tidspunkt, godkjent, begrunnelse)
    VALUES (@brukerid, @kortleser, @kortid, @tidspunkt, @godkjent, @begrunnelse)";



                    command.Parameters.AddWithValue("@brukerid", (object)brukerid ?? DBNull.Value);
                    command.Parameters.AddWithValue("@kortleser", kortleser);
                    command.Parameters.AddWithValue("@kortid", kort);
                    command.Parameters.AddWithValue("@tidspunkt", tidspunkt);
                    command.Parameters.AddWithValue("@godkjent", godkjent);
                    command.Parameters.AddWithValue("@begrunnelse", melding);


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
                    Loggføring(connectionString, "Godkjent", kortleserId , kortId, godkjent);
                }

                else if (hvaskjer == false) // dårlig stemmning
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = @"SELECT EXISTS (SELECT 1 FROM Bruker WHERE kort_id = @kortID)";
                        command.Parameters.AddWithValue("@kortID", kortId);
                        long resultat = (long)command.ExecuteScalar();



                        if (resultat == 1) { Loggføring(connectionString, "ikke Godkjent, feil pin", kortleserId, kortId, godkjent);}

                        else { Loggføring(connectionString, "ikke Godkjent, ugyldig kort", kortleserId, kortId, godkjent); }

                        
                    }

                }
            }
        
        }

        static void backgrunnsKommunikasjon(string connectionString) // dette er det som skal gjøre at vi kan motta info. 
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
                Socket kommSokkel = lytteSokkel.Accept(); // her lager vi kommunikasjon sokkel når vi får kontakt med klient
                IPEndPoint klientEP = (IPEndPoint)kommSokkel.RemoteEndPoint;


                // sender data til klient
                string hilsen = "tilkobling til sentral vellyket";
                data = Encoding.ASCII.GetBytes(hilsen);
                kommSokkel.Send(data, data.Length, SocketFlags.None);


                // motak av pin fra greien


                // Mottar informasjon fra klienten
                int nMottatt = kommSokkel.Receive(data);

                // Gjør mottatt informasjon om til tekst
                mottattTekst = Encoding.ASCII.GetString(data, 0, nMottatt);

                // Deler opp teksten ved semikolon
                string[] informasjon = mottattTekst.Split(';');

                // Lagrer KortID og PIN i egne variabler
                string kortID = informasjon[0];
                string pin = informasjon[1];
                string kortleserId = informasjon[2];

                // Skriver ut informasjonen for å se om det fungerer
                // Console.WriteLine($"KortID: {kortID}");
                ///Console.WriteLine($"PIN: {pin}");

                bool svar = sjekkAvKort(pin, kortID, connectionString);

                if (svar == true) { Console.WriteLine("godkjent mofo"); }
                else {Console.WriteLine("ikke godkjent mofo"); }

                LoggføringsValg(svar, kortID, kortleserId, connectionString);




                // Avslutter forbindelsen
                kommSokkel.Close();

            }




        }
    }

    

    


}