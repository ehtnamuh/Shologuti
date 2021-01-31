using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[System.Serializable]
public class GutiNode
{
    public Address Address;
    public List<Address> ConnectedNeighbours;
    public GutiType gutiType;

    public GutiNode GetCopy()
    {
        var instance = new GutiNode();
        instance.Address = Address;
        instance.ConnectedNeighbours = ConnectedNeighbours;
        instance.gutiType = gutiType;
        return instance;
    }
}

[System.Serializable]
public class GutiNodes
{
    public GutiNode[] gutiArray;
}

[System.Serializable]
public struct Address
{
    public int x;
    public int y;

    public static Address operator -(Address a, Address b) => new Address {x = a.x - b.x, y = a.y - b.y};
    public static Address operator +(Address a, Address b) => new Address {x = a.x + b.x, y = a.y + b.y};
    public static bool operator ==(Address a, Address b) => (a.x == b.x && a.y == b.y);
    public static bool operator !=(Address a, Address b) => !(a == b);

    public static Address operator *(Address a, int multiplier)
    {
        return new Address {x = a.x * multiplier, y = a.y * multiplier};
    }
    
    public float GetMagnitude()
    {
        var xx = (float) x;
        var yy = (float) y;
        var sq = (float)Math.Sqrt(xx*xx + yy*yy);
        return sq;
    }
    
    public Address GetDirectionVector()
    {
        var addr = new Address {x = x == 0 ? 0 : x / Math.Abs(x), y = y == 0 ? 0 : y / Math.Abs(y)};
        return addr;
    }
    public bool Equals(Address other) => x == other.x && y == other.y;

    public override bool Equals(object obj) => obj is Address other && Equals(other);
    public override int GetHashCode()
    {
        unchecked
        {
            return (x * 397) ^ y;
        }
    }
    public override string ToString() => $"x: {x} y: {y}";
}

public enum GutiType
{
    RedGuti = 0,
    GreenGuti = 1,
    NoGuti = 2,
    Highlight = 3
}