using eBSH.Models;

namespace eBSH.Repositories
{
    public interface IOrderRepository
    {
        OrderInfo GetByOrderID(decimal orderID);
        OrderInfo GetLastOrderByPolicyID(decimal PolicyID);
        decimal Insert(OrderInfo orderInf);
        decimal Update(OrderInfo orderInf);
    }
}