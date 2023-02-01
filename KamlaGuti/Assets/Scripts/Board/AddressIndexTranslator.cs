using System.Collections.Generic;
using Board.Guti;

namespace Board
{
    public static class AddressIndexTranslator
    {
        private static Dictionary<Address, int> _addressToIndex;
        private static Dictionary<int, Address> _indexToAddress;

        public static void LoadAddressIndexTranslator(IReadOnlyList<GutiNode> gutiNodeArray)
        {
            _addressToIndex = new Dictionary<Address, int>(37);
            _indexToAddress = new Dictionary<int, Address>(37);
            Init(gutiNodeArray);
        }

        private static void Init(IReadOnlyList<GutiNode> gutiArray)
        {
            for (var index = 0; index < gutiArray.Count; index++)
            {
                var gutiNode = gutiArray[index];
                _addressToIndex.Add(gutiNode.Address, index);
                _indexToAddress.Add(index, gutiNode.Address);
            }
        }

        public static Address GetAddressFromIndex(int addressIndex) => _indexToAddress[addressIndex];
		
        public static int GetIndexFromAddress(Address address) => _addressToIndex[address];

    }
}