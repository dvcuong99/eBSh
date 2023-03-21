using eBSH.Models;
using System.Collections.Generic;

namespace eBSH.Repositories
{
    public interface IRefCodeRepository
    {
        RefCode GetByCode(string Code);
        IEnumerable<RefCode> GetList(string CTBH, string Ma_BH, string Ma_Dvi);
        decimal Update(RefCode orderInf);
        string Insert(RefCode refCodeInf);
    }
}