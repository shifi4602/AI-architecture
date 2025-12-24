using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enteties;

namespace DTO_s
{
    public record OrdersDTO
    (
        int OrderId,
        DateOnly? OrderDate,
        int OrderSum,
        //ICollection<OrderItemDTO> ordersItem,
        int UserId
    );
}
