using System.Collections.Generic;
using Board.Guti;

namespace Board
{
    public class AddressIndexTranslator
    {
        private readonly Dictionary<Address, int> _addressToIndex;
        private readonly Dictionary<int, Address> _indexToAddress;

        public AddressIndexTranslator(IReadOnlyList<GutiNode> gutiArray)
        {
            _addressToIndex = new Dictionary<Address, int>(37);
            _indexToAddress = new Dictionary<int, Address>(37);
            Init(gutiArray);
        }

        private void Init(IReadOnlyList<GutiNode> gutiArray)
        {
            for (var index = 0; index < gutiArray.Count; index++)
            {
                var gutiNode = gutiArray[index];
                _addressToIndex.Add(gutiNode.Address, index);
                _indexToAddress.Add(index, gutiNode.Address);
            }
        }

        public Address GetAddressFromIndex(int addressIndex) => _indexToAddress[addressIndex];
		
        public int GetIndexFromAddress(Address address) => _addressToIndex[address];

    }
}