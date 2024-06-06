using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM.Repository.Interface
{
    public interface IRecordRepository
    {
        Task<int> CreateRecord(string name);
    }
}
