using SpojDebug.Business.AdminSetting;
using SpojDebug.Core.Constant;
using SpojDebug.Service.SPOJExternal;
using SpojDebug.Ultil.Exception;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace SpojDebug.Service.Logic.SPOJExternal
{
    public class SpojExternalService : HttpClient, ISpojExternalService
    {
        private readonly IAdminSettingBusiness _spojBusiness;

        public SpojExternalService(IAdminSettingBusiness spojBusiness)
        {
            _spojBusiness = spojBusiness;
        }

        public void GetSpojInfo()
        {
            _spojBusiness.GetSpojInfo();
        }

        public bool Login()
        {
            return _spojBusiness.Login();
        }
    }
}
