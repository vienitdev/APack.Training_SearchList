using Archpack.Training.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Authentication;
using System.Web;

namespace Archpack.Training.ArchUnits.WebApiModels.V1
{
    public partial class WebApiErrorResponse
    {
        public static WebApiErrorResponse Create(Exception exception)
        {
            var target = RetrieveTargetException(exception);
            return HandleAuthenticationError(target) ??
                    HandleAuthorizeError(target) ??
                    HandleDbUpdateConcurrencyError(target) ??
                    HandleDbUpdateError(target) ??
                    HandleDbEntityValidationError(target) ??
                    HandleWebApiError(target) ??
                    new WebApiErrorResponse()
                    {
                        ErrorType = WebApiErrorTypes.SystemError,
                        Message = Resources.ServiceError,
                        Description = target.ToString()
                    };
        }
        private static Exception RetrieveTargetException(Exception exception)
        {
            var aggregateEx = exception as AggregateException;
            if (aggregateEx != null)
            {
                var result = aggregateEx.InnerExceptions.FirstOrDefault();
                if (result != null)
                {
                    return result;
                }
            }
            return exception;
        }
        private static WebApiErrorResponse HandleAuthenticationError(Exception exception)
        {
            AuthenticationException targetException = exception as AuthenticationException;
            if (targetException == null)
            {
                targetException = exception.InnerException as AuthenticationException;
            }
            if (targetException == null)
            {
                return null;
            }
            return new WebApiErrorResponse()
            {
                ErrorType = WebApiErrorTypes.AuthenticationError,
                Message = Resources.AuthorizedErrorMessage,
                Description = string.Empty
            };
        }
        private static WebApiErrorResponse HandleAuthorizeError(Exception exception)
        {
            UnauthorizedAccessException targetException = exception as UnauthorizedAccessException;
            if (targetException == null)
            {
                targetException = exception.InnerException as UnauthorizedAccessException;
            }
            if (targetException == null)
            {
                return null;
            }
            return new WebApiErrorResponse()
            {
                ErrorType = WebApiErrorTypes.AuthorizationError,
                Message = exception.Message ?? Resources.RoleAuthorizedErrorMessage,
                Description = string.Empty
            };
        }
        private static WebApiErrorResponse HandleDbUpdateConcurrencyError(Exception exception)
        {
            DbUpdateConcurrencyException targetException = exception as DbUpdateConcurrencyException;
            if (targetException == null)
            {
                targetException = exception.InnerException as DbUpdateConcurrencyException;
            }
            if (targetException == null)
            {
                return null;
            }
            return new WebApiErrorResponse()
            {
                ErrorType = WebApiErrorTypes.ConflictError,
                Message = Resources.DbUpdateConcurrencyError,
                Description = targetException.ToString()
            };
        }
        private static WebApiErrorResponse HandleDbUpdateError(Exception exception)
        {
            DbUpdateException dbUpdateException = exception as DbUpdateException;
            if (dbUpdateException == null)
            {
                return null;
            }
            SqlException sqlException = dbUpdateException.InnerException as SqlException;
            UpdateException updateException = dbUpdateException.InnerException as UpdateException;
            if (sqlException != null || updateException != null)
            {
                //TODO: データベースで発生した例外に応じた処理を実装します
                return new WebApiErrorResponse()
                {
                    ErrorType = WebApiErrorTypes.DatabaseError,
                    Message = Resources.DbSaveErrorMessage,
                    Description = exception.ToString()
                };
            }
            return null;
        }
        private static WebApiErrorResponse HandleDbEntityValidationError(Exception exception)
        {
            DbEntityValidationException targetException = exception as DbEntityValidationException;
            if (targetException == null)
            {
                return null;
            }
            return new WebApiErrorResponse()
            {
                ErrorType = WebApiErrorTypes.DatabaseError,
                Message = Resources.DbSaveErrorMessage,
                Description = targetException.ToString()
            };
        }
        private static WebApiErrorResponse HandleWebApiError(Exception exception)
        {
            var targetException = exception as WebApiErrorException;
            if (targetException == null)
            {
                return null;
            }
            return targetException.ErrorResponseData;
        }
        
    }
}