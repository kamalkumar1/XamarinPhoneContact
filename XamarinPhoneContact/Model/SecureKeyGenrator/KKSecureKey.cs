using System;

namespace XamarinPhoneContact.Model.SecureKeyGenrator;

public readonly struct KKSecureKey : IDisposable
{
    private readonly string _value;
    private readonly byte[] _bytes;
    
    public string Value => _value;
    public ReadOnlySpan<byte> Bytes => _bytes.AsSpan();  // ✅ .AsSpan()
    
    public KKSecureKey(ReadOnlySpan<byte> keyBytes)
    {
        _bytes = keyBytes.ToArray();
        _value = Convert.ToBase64String(_bytes);
    }
    
    public void Dispose()
    {
          if (_bytes != null)
             Array.Clear(_bytes, 0, _bytes.Length);  // ✅ Secure zeroing
    }
    
    public static implicit operator string(KKSecureKey key) => key.Value;
}
