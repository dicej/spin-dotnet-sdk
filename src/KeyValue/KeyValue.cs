using System.Text;

namespace Spin.SDK.KeyValue;

public class Store
{
    private readonly SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0.IKeyValue.Store _store;

    private Store(SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0.IKeyValue.Store store)
    {
        _store = store;
    }
    private const string DEFAULT_STORE_NAME = "default";
    
    
    public static Store Open(string name)
    {
        var inner = SpinHttpWorld.wit.imports.fermyon.spin.v2_0_0.IKeyValue.Store.Open(name);
        return new Store(inner);

    }

    public static Store OpenDefault()
    {
        return Open(DEFAULT_STORE_NAME);
    }

    public IList<string> GetKeys()
    {
        return _store.GetKeys();
    }

    public bool Exists(string key)
    {
        return _store.Exists(key);
    }

    public byte[]? Get(string key)
    {
        return _store.Get(key);
    }

    public string? GetString(string key, Encoding encoding)
    {
        var raw = Get(key);
        return raw == null ? null : encoding.GetString(raw);
    }

    public void Set(string key, byte[] value)
    {
        _store.Set(key, value);
    }

    public void GetString(string key, string value, Encoding encoding)
    {
        _store.Set(key, encoding.GetBytes(value));
    }

    public void Delete(string key)
    {
        _store.Delete(key);
    }

}