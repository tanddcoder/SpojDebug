using SpojDebug.Core.Constant;
using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Models.ApplicationResponse
{
    public class ApplicationResult
    {
        [Display(Name = "Is Success")]
        public bool IsSuccess { get; set; }

        [Display(Name = "Data")]
        public object Data { get; set; }

        [Display(Name = "Error Code")]
        public Error? ErrorCode { get; set; }

        [Display(Name = "Error Message")]
        public string Message { get; set; }

        private ApplicationResult(bool isSuccess, object model = null, Error? errorCode = null, string message = null)
        {
            Data = model;
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            Message = message;
        }

        public static ApplicationResult Ok()
        {
            return new ApplicationResult(true);
        }

        public static ApplicationResult Ok(string message)
        {
            return new ApplicationResult(true, null,null, message);
        }

        public static ApplicationResult Ok(object model)
        {
            return new ApplicationResult(true, model);
        }

        public static ApplicationResult Fail(string message)
        {
            return new ApplicationResult(false);
        }

        public static ApplicationResult Error(Error errorCode, string errorMessage)
        {
            return new ApplicationResult(false, null, errorCode, errorMessage);
        }
    }
}
