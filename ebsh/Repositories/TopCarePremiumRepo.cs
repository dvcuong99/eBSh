using Dapper;
using eBSH.Core;
using eBSH.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace eBSH.Repositories
{
    public class TopCarePremiumRepo : ITopCarePremiumRepo
    {
        IDbSession DbSession = null;
        public TopCarePremiumRepo(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }
        public TopCarePremium GetByYear(string CTBH, int ThoiHan, int Tuoi)
        {
            DynamicParameters dp = new DynamicParameters();
            dp.Add("@CTBH", CTBH);
            dp.Add("@ThoiHan", ThoiHan);
            dp.Add("@Tuoi", Tuoi);
            dp.Add("@iErrorCode", DbType.Int32, direction: ParameterDirection.Output);
            return DbSession.Connection.QuerySingle<TopCarePremium>("pr_tblTopCarePremimum_SelectOne", param: dp, commandType: CommandType.StoredProcedure);
        }
    }
}