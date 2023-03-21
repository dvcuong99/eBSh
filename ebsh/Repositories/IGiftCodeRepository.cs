using eBSH.Models;
using System.Collections.Generic;
using System;
namespace eBSH.Repositories
{
    public interface IGiftCodeRepository
    {
        GiftCode GetByCode(string CodeKM,string Ma_BH, string CTBH);
        decimal Update(GiftCode orderInf);
        void UpdateRemainderKM(string CodeKM);
        IEnumerable<GiftCode> GetList(string MA_BH,string CTBH, DateTime SDate, DateTime EDate);
        string Insert(GiftCode giftInf);
        GiftCode GetOne(string CodeKM, string Ma_BH, string CTBH);
    }
}