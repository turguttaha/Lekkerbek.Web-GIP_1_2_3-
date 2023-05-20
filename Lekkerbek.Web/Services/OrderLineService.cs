using Lekkerbek.Web.Models;
using Lekkerbek.Web.Repositories;

namespace Lekkerbek.Web.Services
{
    public class OrderLineService
    {
        private readonly OrderLineRepository _repository;
        public OrderLineService(OrderLineRepository repository)
        {
            _repository = repository;
        }
        public OrderLine GetSpecificOrderLineDetailed(int? id) 
        {
            return _repository.GetOrderLineDetailed(id);
        }
        public OrderLine GetSpecificOrderLine(int? id)
        {
            return _repository.GetOrderLine(id);
        }
        public void AddOrderLine(OrderLine orderLine) 
        {
            _repository.AddToDataBase(orderLine);
        }
        public void RemoveOrderLine(OrderLine orderLine)
        {
            _repository.DeleteFromDataBase(orderLine);
        }
    }
}
