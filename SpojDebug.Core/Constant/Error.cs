using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Constant
{
    public enum Error
    {
        //General
        [Display(Name = "Parameter can not be null!")]
        ParamNull = 1,

        //Spoj External
        [Display(Name = "Cannot contact to SPOJ, there is a fault!")]
        Fault = 101,

        [Display(Name = "Cannot contact to SPOJ, the request was cancelled!")]
        Cancelled,
    }
}
