using System.Management;
using System.Security.Cryptography;
using System.Text;

public class MachineHelper
{
    public static string GetMachineId()
    {
        string cpuId = "";

        var searcher = new ManagementObjectSearcher("select ProcessorId from Win32_Processor");

        foreach (var item in searcher.Get())
        {
            cpuId = item["ProcessorId"].ToString();
            break;
        }

        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(cpuId));
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}