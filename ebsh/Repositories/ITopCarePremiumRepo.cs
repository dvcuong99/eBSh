using eBSH.Models;

namespace eBSH.Repositories
{
    public interface ITopCarePremiumRepo
    {
        TopCarePremium GetByYear(string CTBH, int ThoiHan, int Tuoi);
    }
}