using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Archpack.Training.ArchUnits.Validations.V1;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    public static class ValidationResultExtensions
    {

        /// <summary>
        /// Web API で発生したエラー情報クラスを生成します。
        /// </summary>
        /// <param name="validationResult"></param>
        /// <returns></returns>
        public static WebApiErrorResponse CreateWebApiErrorResponse(this ValidationResult validationResult)
        {
            WebApiErrorResponse result = new WebApiErrorResponse { ErrorType = WebApiErrorTypes.InputError, Message = Resources.ValidationErrorMessage };

            foreach (ValidationErrorInfo info in validationResult.Errors)
            {
                WebApiErrorDetail detail = new WebApiErrorDetail{ PropertyId = info.PropertyId, ObjectId = info.ObjectId, Name = info.Name, Value = info.Value, Message = info.Message };

                result.Errors.Add(detail);
            }

            return result;
        }
    }
}