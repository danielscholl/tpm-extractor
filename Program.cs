using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Security;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iot_extractor
{
    class Program
    {
        private static string _idScope = string.Empty;
        private static string _registrationId = Environment.MachineName.ToLower();

        private const string GlobalDeviceEndpoint = "global.azure-devices-provisioning.net";

        static int Main(string[] args)
        {

            if ((args.Length > 0))
            {
                _registrationId = args[0];

                var r = new Regex("^[a-z0-9-]*$");
                if (!r.IsMatch(_registrationId))
                {
                    throw new FormatException("Invalid registrationId: The registration ID is alphanumeric, lowercase, and may contain hyphens");
                }
            }

            if (string.IsNullOrWhiteSpace(_idScope) && (args.Length > 1))
            {
                _idScope = args[1];
            }

            using (var security = new SecurityProviderTpmHsm(_registrationId))
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("$EndorsementKey=\"");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0}", Convert.ToBase64String(security.GetEndorsementKey()));
                Console.Write("\"");
                Console.WriteLine(Environment.NewLine);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("$RegistrationId=\"");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("{0}", security.GetRegistrationID());
                Console.Write("\"");
                Console.WriteLine(Environment.NewLine);

                if (!String.IsNullOrEmpty(_idScope)) Register(security).Wait();

            }
            return 0;
        }


        static async Task Register(SecurityProviderTpmHsm security)
        {
            Console.WriteLine("Press ENTER when ready to execute Registration.");
            Console.WriteLine("------------------");
            Console.ReadLine();

            using (var transport = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly))
            {
                var client = ProvisioningDeviceClient.Create(GlobalDeviceEndpoint, _idScope, security, transport);

                try
                {
                    DeviceRegistrationResult result = await client.RegisterAsync().ConfigureAwait(false);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\tAssigned Hub: :  ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("{0} ", result.AssignedHub);
                    Console.WriteLine(Environment.NewLine);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("\tDeviceId: :  ");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("{0} ", result.DeviceId);
                    Console.WriteLine(Environment.NewLine);
                }
                catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The registration has failed");
                    Console.ForegroundColor = ConsoleColor.White;
                    throw;
                }
            }
        }
    }
}
