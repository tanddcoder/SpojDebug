using SpojDebug.Core.Constant;
using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Models.ApplicationResponse
{
    public class ApplicationResult<TModel> where TModel: class
    {
        [Display(Name = "Is Success")]
        public bool IsSuccess { get; set; }

        [Display(Name = "Data")]
        public TModel Data { get; set; }

        [Display(Name = "Error Code")]
        public Error? ErrorCode { get; set; }

        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }

        private ApplicationResult( bool isSuccess, TModel model = null, Error? errorCode = null, string errorMessage = null)
        {
            Data = model;
            IsSuccess = true;
            ErrorCode = errorCode;
            ErrorMessage = ErrorMessage;
        }

        public static ApplicationResult<TModel> Ok()
        {
            return new ApplicationResult<TModel>(true);
        }

        public static ApplicationResult<TModel> Ok(TModel model)
        {
            return new ApplicationResult<TModel>(true, model);
        }

        public static ApplicationResult<TModel> Fail(string message)
        {
            return new ApplicationResult<TModel>(false);
        }

        public static ApplicationResult<TModel> Error(Error errorCode, string errorMessage)
        {
            return new ApplicationResult<TModel>(false, null, errorCode, errorMessage);
        }
    }
}
