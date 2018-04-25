using SpojDebug.Business;

namespace SpojDebug.Service.Logic
{
    public class SeedDataService : ISeedDataService
    {
        private readonly ISeedDataBusiness _seedDataBusiness;

        public SeedDataService(ISeedDataBusiness seedDataBusiness)
        {
            _seedDataBusiness = seedDataBusiness;
        }

        public void InitData()
        {
            _seedDataBusiness.InitData();
        }
    }
}
