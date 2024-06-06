using FM.Repository.Interface;

namespace FM.Repository.Repository
{
    public class RecordRepository : IRecordRepository
    {
        private readonly DapperContext _dapperContext;
        public RecordRepository(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }
        public Task<int> CreateRecord(string name)
        {
            throw new NotImplementedException();
        }
    }
}
