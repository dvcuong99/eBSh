using eBSH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBSH.Repositories
{
    public interface IMotorVehicleRepository
    {
        decimal Insert(MotorVehicle motorcycle);
        decimal Update(MotorVehicle motorcycle);
        MotorVehicle GetByID(decimal ID);
        IEnumerable<MotorVehicle> GetList(DateTime SDate, DateTime EDate, string MaDvi, string Kenh, string DaiLy);
    }
}
