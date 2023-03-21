using eBSH.Models;
using System.Collections.Generic;
using System;

namespace eBSH.Repositories
{
    public interface IGCNRepository
    {
        PHH_CBS_GCN GetByID(decimal PolicyID);
        decimal Insert(PHH_CBS_GCN orderInf);
        decimal Update(PHH_CBS_GCN orderInf);
        IEnumerable<PHH_CBS_GCN> GetList(string CTBH, DateTime SDate, DateTime EDate);
        PHH_CBS_GCN GetByID(string SO_ID);
        IEnumerable<CBSExp> GetData(string CTBH, DateTime SDate, DateTime EDate);
    }
}