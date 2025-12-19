using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Foundation;

namespace MauiPhoneContactLibrary.Platforms.iOS;

public static class KKEncryptionHelperiOS
{
  /// <summary>
  /// Encrypts NSData using AES-256 encryption with the provided secure key
  /// </summary>
  public static NSData? EncryptData(NSData data, string secureKey)
  {
    if (data == null || string.IsNullOrEmpty(secureKey))
    {
      Debug.WriteLine("❌ EncryptData: Invalid input parameters");
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

      // Convert NSData to byte array
      var dataBytes = new byte[data.Length];
      System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, (int)data.Length);

      // Encrypt
      var encryptedBytes = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);

      // Combine IV + encrypted data
      var combined = new byte[aes.IV.Length + encryptedBytes.Length];
      Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
      Buffer.BlockCopy(encryptedBytes, 0, combined, aes.IV.Length, encryptedBytes.Length);

      return NSData.FromArray(combined);
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"❌ Encryption error: {ex.Message}");
      return null;
    }
  }

  /// <summary>
  /// Decrypts NSData using AES-256 decryption with the provided secure key
  /// </summary>
  public static NSData? DecryptData(NSData encryptedData, string secureKey)
  {
    if (encryptedData == null || string.IsNullOrEmpty(secureKey))
    {
      Debug.WriteLine("❌ DecryptData: Invalid input parameters");
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

      // Convert NSData to byte array
      var combinedBytes = new byte[encryptedData.Length];
      System.Runtime.InteropServices.Marshal.Copy(encryptedData.Bytes, combinedBytes, 0, (int)encryptedData.Length);

      // Extract IV (first 16 bytes) and encrypted data
      var iv = new byte[16];
      var encryptedBytes = new byte[combinedBytes.Length - 16];
      Buffer.BlockCopy(combinedBytes, 0, iv, 0, 16);
      Buffer.BlockCopy(combinedBytes, 16, encryptedBytes, 0, encryptedBytes.Length);

      aes.IV = iv;

      using var decryptor = aes.CreateDecryptor();
      var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

      return NSData.FromArray(decryptedBytes);
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
