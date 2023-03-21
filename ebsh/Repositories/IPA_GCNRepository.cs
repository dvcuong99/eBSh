using eBSH.Models;
using System.Collections.Generic;
using System;

namespace eBSH.Repositories
{
    public interface IPA_GCNRepository
    {
        CN_PA_GCN GetByID(decimal PolicyID);
        decimal Insert(CN_PA_GCN orderInf);
        decimal Update(CN_PA_GCN orderInf);
        IEnumerable<CN_PA_GCN> GetList(string CTBH, DateTime SDate, DateTime EDate);
    }
}