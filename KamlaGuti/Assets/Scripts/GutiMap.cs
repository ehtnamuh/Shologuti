using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GutiMap
{
    private Dictionary<Address, GutiNode> _gutiMap;

    private Dictionary<Address, GutiNode> GetGutiMap() => _gutiMap;

    public GutiMap(GutiMap gutiMap)
    {
        _gutiMap = new Dictionary<Address, GutiNode>();
        foreach (var node in gutiMap.GetGutiMap()) _gutiMap.Add(node.Key, node.Value.GetCopy());
    }

    public GutiMap() => _gutiMap = new Dictionary<Address, GutiNode>();

    private GutiNode GetGutiNode(Address address)
    {
        try
        {
            return _gutiMap[address];
        }
        catch (Exception e)
        {
            Debug.Log($"Error Accessing( {address} ) in GutiMap Class");
            return null;
        }
        
    }
    public GutiType GetGutiType(Address add) => GetGutiNode(add).gutiType;
    public void AddGuti(Address address, GutiNode gutiNode) => _gutiMap[address] = gutiNode;

    public IEnumerable<Address> GetWalkableNodes(Address address)
    {
        var neighbourGutiNodes =  _gutiMap[address].ConnectedNeighbours;
        var walkableGutiNodes = new List<Address>();
        foreach (var neighbourGutiNodeAddress in neighbourGutiNodes)
        {
            var neighbourGutiType = GetGutiType(neighbourGutiNodeAddress);
            if (neighbourGutiType == GutiType.NoGuti)
            {
                walkableGutiNodes.Add(neighbourGutiNodeAddress);
            } 
            else if (neighbourGutiType != _gutiMap[address].gutiType)
            {
                var direction = (neighbourGutiNodeAddress - address);
                var jumpAddress = neighbourGutiNodeAddress + direction;
                if (!_gutiMap[neighbourGutiNodeAddress].ConnectedNeighbours.Contains(jumpAddress)) continue;
                if(_gutiMap[jumpAddress].gutiType == GutiType.NoGuti) walkableGutiNodes.Add(jumpAddress);
            }
        }
        return walkableGutiNodes;
    }

    public bool CanCaptureGuti(Address address)
    {
        var neighbourGutiNodes =  _gutiMap[address].ConnectedNeighbours;
        for (var index = 0; index < neighbourGutiNodes.Count; index++)
        {
            var neighbourGutiNodeAddress = neighbourGutiNodes[index];
            var neighbourGutiType = _gutiMap[neighbourGutiNodeAddress].gutiType;
            if (neighbourGutiType == GutiType.NoGuti) continue;
            if (neighbourGutiType == _gutiMap[address].gutiType) continue;
            var direction = (neighbourGutiNodeAddress - address);
            var jumpAddress = neighbourGutiNodeAddress + direction;
            if (!_gutiMap[neighbourGutiNodeAddress].ConnectedNeighbours.Contains(jumpAddress)) continue;
            if (_gutiMap[jumpAddress].gutiType == GutiType.NoGuti) return true;
        }
        return false;
    }
    
    public bool CanCaptureGuti(Address sourceAddress, Address targetAddress)
    {
        var capturedGutiAddress = GetCapturedGutiAddress(sourceAddress, targetAddress);
        return capturedGutiAddress != targetAddress;
    }
    
    // Moves guti on logical board does not check for Validity of Move
    public void CaptureGuti(Address sourceAddress, Address targetAddress)
    {
        RemoveGuti(GetCapturedGutiAddress(sourceAddress, targetAddress)); // if no captured guti, sets Guti at target address to no guti
        GetGutiNode(targetAddress).gutiType = GetGutiType(sourceAddress);
        GetGutiNode(sourceAddress).gutiType = GutiType.NoGuti;
    }

    public void MoveGuti(Address sourceAddress, Address targetAddress)
    {
        GetGutiNode(targetAddress).gutiType = GetGutiType(sourceAddress);
        GetGutiNode(sourceAddress).gutiType = GutiType.NoGuti;
    }

    private void RemoveGuti(Address address) => GetGutiNode(address).gutiType = GutiType.NoGuti;

    public void RestoreGuti(Address address, GutiType gutiType) => GetGutiNode(address).gutiType = gutiType;


    public Address GetCapturedGutiAddress(Address sourceAddress, Address targetAddress)
    {
        var capturedGutiAddress = targetAddress - sourceAddress;
        if (Math.Abs(capturedGutiAddress.x) >= 3)
            capturedGutiAddress = sourceAddress + capturedGutiAddress.GetDirectionVector() * 2;
        else
            capturedGutiAddress = sourceAddress + capturedGutiAddress.GetDirectionVector();
        var connectedNeighbours = GetGutiNode(sourceAddress).ConnectedNeighbours;
        return connectedNeighbours.Contains(capturedGutiAddress) ? capturedGutiAddress : targetAddress;
    }
    
    
    public IEnumerable<Address> GetGutisOfType(GutiType gutiType) => (from pair in _gutiMap where pair.Value.gutiType == gutiType select pair.Key).ToList();

    public List<float> GetGutiTypeList() => _gutiMap.Select(node => (float)node.Value.gutiType).ToList();

}
