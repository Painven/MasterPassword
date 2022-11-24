using MasterPasswordDesktop.Model;
using System.Collections.Generic;

namespace MasterPasswordDesktop.DataAccess
{
    public interface IDataStorage
    {
        void AddOrUpdateDataLine(DataLine newItem);
        void DeleteDataLine(DataLine line);
        List<DataLine> ReadDataLines();
    }
}