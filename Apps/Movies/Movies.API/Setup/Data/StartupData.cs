using StronglyTypedAppSettings;

namespace Movies.API.Setup.Data;

/// <summary>
/// Class with config data for this app and it's libraries
/// </summary>
public class StartupData(IConfiguration config) : AppSettingsAccessor(config)
{

    /// <summary>
    /// Name of this application
    /// </summary>
    public string APP_NAME => "Docker Movies Demo";

    /// <summary>
    /// Company Colors. Used in Emails etc.
    /// </summary>
    public string COLOR_HEX_BRAND => "#eeb313";

    /// <summary>
    /// Name of company that this app is for
    /// </summary>
    public string COMPANY_NAME => "Spider Baby Movies";

    /// <summary>
    /// Name to appear in Logging Emails
    /// </summary>
    public string COMPANY_NAME_LOGGING => "Spider Baby Movies API";
      

    /// <summary>
    /// Reads the contents of an XML file and returns it as a string.
    /// </summary>
    /// <param name="filePath">The path to the XML file.</param>
    /// <returns>The contents of the XML file as a string.</returns>
    public static string FileToString(string filePath)
    {
        using var sr = File.OpenText(filePath);
        return sr.ReadToEnd();
    }


    /// <summary>
    /// Gets the XML string of the asymmetric public key from the Jwt/key.public.xml file.
    /// </summary>
    /// <returns>The XML string of the public key.</returns>
    public string GetAsymmetricPublicKeyXmlString()
        => FileToString(Path.Join("JwtKeys", "public.xml"));


    /// <summary>
    /// Gets the XML string of the asymmetric private key from the Jwt/key.private.xml file.
    /// </summary>
    /// <returns>The XML string of the private key.</returns>
    public string GetAsymmetricPrivateKeyXmlString()
        => FileToString(Path.Join("JwtKeys", "private.xml"));


    /// <summary>
    /// Gets the PEM string of the asymmetric public key from the Jwt/key.public.PEM file.
    /// </summary>
    /// <returns>The PEM string of the public key.</returns>
    public string GetAsymmetricPublicKeyPemString()
        => FileToString(Path.Join("JwtKeys", "public.pem"));


    /// <summary>
    /// Gets the PEM string of the asymmetric private key from the Jwt/key.private.PEM file.
    /// </summary>
    /// <returns>The PEM string of the private key.</returns>
    public string GetAsymmetricPrivateKeyPemString()
        => FileToString(Path.Join("JwtKeys", "private.pem"));



    /// <summary>
    /// Gets the URL of the company logo.
    /// </summary>
    /// <returns>A string representing the URL of the company logo.</returns>
    public string GetLogoUrl() =>
        "https://spider-baby-hub.web.app/images/myid/sb-id-demo-logo.png";



}//Cls


