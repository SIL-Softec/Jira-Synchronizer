using HashidsNet;
using JiraSynchronizer.Core.Interfaces.Services;

namespace JiraSynchronizer.Core.Services;

public class HashService : IHashService
{
    private readonly Hashids _hashids;

    public HashService()
    {
        _hashids = new Hashids("6IY0Q-0PZRX0COJ6-EV2N73AI1VAZ6-GDNX");
    }

    public string Encode(long input)
    {
        return _hashids.EncodeLong(input);
    }

    public long Decode(string input)
    {
        var decoded = _hashids.DecodeLong(input);
        return decoded != null && decoded.Length > 0 ? decoded[0] : long.MinValue;
    }
}