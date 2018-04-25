using System.ComponentModel.DataAnnotations;

namespace SpojDebug.Core.Constant
{
    public class Enums
    {
        public enum SocialNetworkType
        {
            [Display(Name = "Facebook")]
            Facebook = 1,

            [Display(Name = "Instagram")]
            Instagram = 2,
        }

        public enum ResultType
        {   
            [Display(Name = "Wrong Answer")]
            WrongAnswer = 1,

            [Display(Name = "Time limit exceeded")]
            TimeLimited = 2,

            [Display(Name = "Compile Error")]
            CompileErroe = 3,

            [Display(Name = "Memory limit exceeded")]
            MemoryLimit = 4,

            [Display(Name = "Runtime Error")]
            RuntimeError = 5,

            [Display(Name = "Accepted")]
            Accepted = 6,
        }

        public enum UserRole
        {
            [Display(Name = "Admin")]
            Admin = 1,

            [Display(Name = "Supporter")]
            Supporter = 2,

            [Display(Name = "Assisstant")]
            TeacherAssisstant = 3,

            [Display(Name = "User")]
            NormalUser = 4,
        }
    }
}
