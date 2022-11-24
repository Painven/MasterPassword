using MasterPassword.Model;
using System.Collections.Generic;

namespace MasterPassword.DataAccess
{
    public interface IDatabaseStorage
    {
        void AddOrUpdateDataLine(DataLine line);
        void DeleteDataLine(DataLine line);
        List<DataLine> ReadDataLines();
    }
}
