using System.Security.Cryptography;
using System.Text;

public class MachineHelper
{
    public static string GetMachineId()
    {
        try
        {
            string raw = Environment.MachineName + Environment.OSVersion.ToString();

            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }
        catch
        {
            return "DEFAULT_MACHINE";
        }
    }
}