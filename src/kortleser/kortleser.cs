using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KORTLESER
{
    internal class kortleser
    {
        static void Main(string[] args)
        {
            //definerer variabler vi trenger
            byte[] data = new byte[1024];
            string input, stringData;

            // lager klient og forteller den hvor serveren befinner seg
            Socket klientSokkel = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint serverEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);


            // kobler seg til server
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



            Console.WriteLine("tast inn kortleserid");
            string KortleserId = Console.ReadLine();
            
            

            // får info om hva slags kort og pin som blir brukt
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




            // foreløpig avsluttning av programmet.
            Console.ReadKey();
            klientSokkel.Close();
        }
    }
}
