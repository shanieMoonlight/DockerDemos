using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace DkrDemo1;

public class CertificateTester
{

    public static void Validate(string password = "password")
    {
        try
        {
            var file = File.ReadAllText(@"C:\Users\Shaneyboy\source\repos\Docker\Https\aspnetapp.pfx");
            Debug.WriteLine("File read successfully.");
            //Debug.WriteLine(file);
#pragma warning disable SYSLIB0057 // Type or member is obsolete
            var cert = new X509Certificate2(@"C:\Users\Shaneyboy\source\repos\Docker\Https\aspnetapp.pfx", password);
#pragma warning restore SYSLIB0057 // Type or member is obsolete
            Debug.WriteLine("Password is correct.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Password is incorrect: " + ex.Message);
        }
    }


}
