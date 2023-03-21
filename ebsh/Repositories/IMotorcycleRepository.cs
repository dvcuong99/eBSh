using eBSH.Models;
using System.Collections.Generic;
using System;
namespace eBSH.Repositories
{
    public interface IMotorcycleRepository
    {
        decimal Insert(Motorcycle motorcycle);
        decimal Update(Motorcycle motorcycle);
        Motorcycle GetByID(decimal ID);
        IEnumerable<Motorcycle> GetList(DateTime SDate, DateTime EDate, string MaDvi, string Kenh, string DaiLy);
    }
}