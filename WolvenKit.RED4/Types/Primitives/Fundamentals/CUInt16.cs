using System.Diagnostics;
using System.Globalization;

namespace WolvenKit.RED4.Types;

[RED("Uint16")]
[DebuggerDisplay("{_value,nq}", Type = "CUInt16")]
public readonly struct CUInt16 : IRedPrimitive<ushort>, IComparable, IComparable<CUInt16>, IEquatable<CUInt16>, IRedInteger
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly ushort _value;

    private CUInt16(ushort value)
    {
        _value = value;
    }

    public static implicit operator CUInt16(ushort value) => new(value);
    public static implicit operator ushort(CUInt16 value) => value._value;

    public static CUInt16 operator +(CUInt16 objA, CUInt16 objB) => new((ushort)(objA._value + objB._value));
    public static CUInt16 operator -(CUInt16 objA, CUInt16 objB) => new((ushort)(objA._value - objB._value));
    public static CUInt16 operator *(CUInt16 objA, CUInt16 objB) => new((ushort)(objA._value * objB._value));
    public static CUInt16 operator /(CUInt16 objA, CUInt16 objB) => new((ushort)(objA._value / objB._value));


    public override int GetHashCode() => _value.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (obj.GetType() != this.GetType())
        {
            return false;
        }

        return Equals((CUInt16)obj);
    }

    public bool Equals(CUInt16 other) => Equals(_value, other._value);

    public override string ToString() => _value.ToString();
    public string ToString(CultureInfo cultureInfo) => _value.ToString(cultureInfo);

    #region IComparable, IComparable<CUInt16>

    public int CompareTo(object? value)
    {
        if (value == null)
        {
            return 1;
        }
        if (value is CUInt16 u)
        {
            return CompareTo(u);
        }
        throw new ArgumentException("Value is not a CUInt16", nameof(value));
    }

    public int CompareTo(CUInt16 value) => _value - value._value;

    #endregion IComparable, IComparable<CUInt16>
}