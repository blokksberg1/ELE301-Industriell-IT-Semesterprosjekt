using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO.Ports;

namespace KORTLESER
{
    internal class kortleser
    {
        static void Main(string[] args)
        {
            // definerer en seriell port
            SerialPort sp = new SerialPort("COM10", 9600);
            sp.Open();


            //definerer variabler vi trenger
            byte[] data = new byte[1024];
            string input, stringData;

            // lager klient og forteller den hvor serveren befinner seg
            Socket klientSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);


            // k obler seg til server
            try
            {
                klientSokkel.Connect(serverEP); // blokkerende metode
            }
            catch (SocketException e)
            {
                Console.WriteLine("Fikk ikke forbindelse med server.");
                Console.WriteLine(e.ToString());
                return;
            }


            // motakelse av beskjed fra sentral
            int nMottatt = klientSokkel.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, nMottatt);
            Console.WriteLine(stringData);

            // asigned kortlesere. (bør kanskje forandres til auto fra simsim)
            Console.WriteLine("tast inn kortleserid");
            string KortleserId = Console.ReadLine();


            while (true)
            {

                // får info om hva slags kort og pin som blir bruk             
                Console.WriteLine("hvilket kort bruker du");
                string kortid = Console.ReadLine();
                Console.WriteLine("tast inn pinkode");
                string pin = Console.ReadLine();


                // sending av pin og kortnr til server.
                // Samler informasjonen i én tekststreng
                input = kortid + ";" + pin + ";" + KortleserId;

                // Gjør teksten om til bytes
                data = Encoding.ASCII.GetBytes(input);

                // Sender informasjonen til serveren
                klientSokkel.Send(data);


                // mota signal fra sentral
                // her får vi bekreftelse
                data = new byte[1024];
                nMottatt = klientSokkel.Receive(data);
                bool godkjent = BitConverter.ToBoolean(data, 0);

                // simsim kommunikasjon

                StyrSimSim(godkjent, sp);


                // sende/endre signal så til simsim


            }

            // foreløpig avsluttning av programmet.
            Console.ReadKey();
            klientSokkel.Close();
        }

        static void StyrSimSim(bool godkjent, SerialPort sp)
        {
            if (godkjent == true)
            {
                sp.Write("$O50"); // låser opp dør
                sp.Write("$O61"); // dør åpnes
                Thread.Sleep(5000); //døren forblir åpen i 5 sek
                sp.Write("$O60"); // dør lukkes
                sp.Write("$O51"); // dør låses

            }
            else
            {
                sp.Write("$O51"); // Døren forblir låst
            }
        }


    }
}
