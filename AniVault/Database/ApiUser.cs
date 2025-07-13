using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace AniVault.Database;

[Table(nameof(ApiUser))]
public class ApiUser
{
    private const int RandomNumberLenght = 24;
    
    [Key]
    public int ApiUserId { get; set; }
    [MaxLength(50)]
    public string UserName { get; set; }
    [MaxLength(32)]
    public string ApiKey { get; set; }

    public ApiUser(string userName)
    {
        UserName = userName;
        ApiKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(RandomNumberLenght));
    }
    
    public override string ToString() => string.Join(" - ", ApiUserId, UserName);
}