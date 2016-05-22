using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
{
    public struct ActosDbOperationResult<TSuccess, TFail>
    {

        private TSuccess successValue;
        private TFail failValue;

        private ActosDbOperationResult(TSuccess success): this()
        {
            this.successValue = success;
            this.failValue = default(TFail);
            this.IsSuccess = true;
            this.IsFail = false;
        }

        private ActosDbOperationResult(TFail fail): this()
        {
            this.successValue = default(TSuccess);
            this.failValue = fail;
            this.IsSuccess = false;
            this.IsFail = true;
        }

        public bool IsSuccess { get; private set; }

        public bool IsFail { get; private set; }

        public TSuccess SuccessValue
        {
            get
            {
                if (!IsSuccess) throw new InvalidOperationException();
                return this.successValue;
            }
        }

        public TFail FailValue
        {
            get
            {
                if (!IsFail) throw new InvalidOperationException();
                return this.failValue;
            }
        }


        public TResult Match<TResult>(Func<TSuccess, TResult> success, Func<TFail, TResult> fail)
        {
            if(!IsSuccess && !IsFail)
            {
                throw new InvalidOperationException();
            }
            if (IsSuccess)
            {
                return success(this.successValue);
            }else
            {
                return fail(this.failValue);
            }
        }


        public static ActosDbOperationResult<TSuccess, TFail> Success(TSuccess value)
        {
            return new ActosDbOperationResult<TSuccess, TFail>(value);
        }

        public static ActosDbOperationResult<TSuccess, TFail> Fail(TFail value)
        {
            return new ActosDbOperationResult<TSuccess, TFail>(value);
        }


    }

    public enum ActosDbOperationFail
    {
        EmployeeNotFoundInReferenceTable = 1,
        EmployeeFoundInTable = 2,
    }

}