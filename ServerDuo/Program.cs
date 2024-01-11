using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerDuo
{
    internal class Program
    {
        static void Main()
        {
            using(ServiceHost host = new ServiceHost(typeof(CommunicationService.ServiceImplementation)))
            {
                host.Open();
                Console.WriteLine("Server is running");
                Console.ReadLine();
            }
        }
    }
}
