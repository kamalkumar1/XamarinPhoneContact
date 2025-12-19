using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace MauiPhoneContactLibrary.Platforms.Android;

public static class KKEncryptionHelperAndroid
{
  /// <summary>
  /// Encrypts a string using AES-256 encryption with the provided secure key
  /// </summary>
  /// <param name="plainText">The text to encrypt</param>
  /// <param name="secureKey">The secure key for encryption</param>
  /// <returns>Base64 encoded encrypted string, or null if encryption fails</returns>
  public static string? EncryptString(string plainText, string secureKey)
  {
    if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(secureKey))
    {
      Debug.WriteLine("❌ EncryptString: Invalid input parameters");
      return null;
    }

    try
    {
      using var aes = Aes.Create();
      aes.KeySize = 256;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      // Derive key from secure key string
      var keyBytes = DeriveKeyFromString(secureKey);
      aes.Key = keyBytes;
      aes.GenerateIV();

      using var encryptor = aes.CreateEncryptor();

      // Convert string to bytes
      var plainBytes = Encoding.UTF8.GetBytes(plainText);

      // Encrypt
      var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

      // Combine IV + encrypted data
      var combined = new byte[aes.IV.Length + encryptedBytes.Length];
      Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
      Buffer.BlockCopy(encryptedBytes, 0, combined, aes.IV.Length, encryptedBytes.Length);

      // Return as Base64 string
      return Convert.ToBase64String(combined);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"❌ Encryption error: {ex.Message}");
      return null;
    }
  }

  /// <summary>
  /// Decrypts a Base64 encoded string using AES-256 decryption with the provided secure key
  /// </summary>
  /// <param name="encryptedText">The Base64 encoded encrypted text</param>
  /// <param name="secureKey">The secure key for decryption</param>
  /// <returns>Decrypted plain text string, or null if decryption fails</returns>
  public static string? DecryptString(string encryptedText, string secureKey)
  {
    if (string.IsNullOrEmpty(encryptedText) || string.IsNullOrEmpty(secureKey))
    {
      Debug.WriteLine("❌ DecryptString: Invalid input parameters");
      return null;
    }

    try
    {
      using var aes = Aes.Create();
      aes.KeySize = 256;
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      // Derive key from secure key string
      var keyBytes = DeriveKeyFromString(secureKey);
      aes.Key = keyBytes;

      // Convert Base64 string to bytes
      var combinedBytes = Convert.FromBase64String(encryptedText);

      // Extract IV (first 16 bytes) and encrypted data
      var iv = new byte[16];
      var encryptedBytes = new byte[combinedBytes.Length - 16];
      Buffer.BlockCopy(combinedBytes, 0, iv, 0, 16);
      Buffer.BlockCopy(combinedBytes, 16, encryptedBytes, 0, encryptedBytes.Length);

      aes.IV = iv;

      using var decryptor = aes.CreateDecryptor();
      var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

      // Convert bytes back to string
      return Encoding.UTF8.GetString(decryptedBytes);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"❌ Decryption error: {ex.Message}");
      return null;
    }
  }

  /// <summary>
  /// Derives a 256-bit (32-byte) key from the secure key string using SHA256
  /// </summary>
  private static byte[] DeriveKeyFromString(string secureKey)
  {
    using var sha256 = SHA256.Create();
    return sha256.ComputeHash(Encoding.UTF8.GetBytes(secureKey));
  }
}
