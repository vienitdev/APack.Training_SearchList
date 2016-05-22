using Archpack.Training.ArchUnits.Contracts.V1;
using Archpack.Training.ArchUnits.Identity.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Archpack.Training.ArchUnits.Arcs.Authentications.V2
{

    public class ActosDbAuthManager : IdentityManager<ActosDbAuthContext, ActosDbAuthUser, string>
    {
        public ActosDbAuthManager() : base(ActosDbAuthContext.CreateContext())
        {

        }

        public ActosDbAuthManager(ActosDbAuthContext context) : base(context)
        {
        }

        public override ActosDbAuthUser FindUser(string key)
        {
            return this.DbContext.User.Where(u => u.IdentityKey == key && u.DeleteFlag == "0").FirstOrDefault();
        }

        public ActosDbOperationResult<ActosDbAuthUser, ActosDbOperationFail> CreateUser(string key, string plainPass, string creatorId)
        {
            Contract.NotEmpty(key, "key");
            Contract.NotEmpty(plainPass, "plainPass");

            if (!ExistKeyInEmployeeTable(key))
            {
                return ActosDbOperationResult<ActosDbAuthUser, ActosDbOperationFail>.Fail(ActosDbOperationFail.EmployeeNotFoundInReferenceTable);
            }
            if (ExistKeyInAccountTable(key))
            {
                return ActosDbOperationResult<ActosDbAuthUser, ActosDbOperationFail>.Fail(ActosDbOperationFail.EmployeeFoundInTable);
            }

            var hashPass = GeneratePasswordHash(plainPass);
            var addModifyDate = DateTime.Now;

            var result = this.DbContext.User.Add(new ActosDbAuthUser()
            {
                IdentityKey = key,
                PassKey = hashPass,
                CreatedById = creatorId,
                CreateDate = addModifyDate,
                UpdateId = creatorId,
                UpdateDate = addModifyDate,
                DeleteFlag = "0"
            });
            this.DbContext.SaveChanges();
            return ActosDbOperationResult<ActosDbAuthUser, ActosDbOperationFail>.Success(result);
        }

        private bool ExistKeyInEmployeeTable(string key)
        {
            return (new EmployeeInformation()).HasUserInfo(new GenericIdentity(key));
        }

        private bool ExistKeyInAccountTable(string key)
        {
            return this.FindUser(key) != null;
        }

        public static string GeneratePasswordHash(string password)
        {
            return CreateHashValue(password);
        }

        private static string CreateHashValue(string text)
        {
            byte[] plainText = Encoding.Unicode.GetBytes(text);

            SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider();
            byte[] cryptoText = provider.ComputeHash(plainText);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in cryptoText)
            {
                builder.AppendFormat("{0:x2}", b);
            }
            return builder.ToString();
        }

    }

}