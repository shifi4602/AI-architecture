using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_s
{
    internal record OrderDTO
    (
        int OrderId,
        DateOnly? OrderDate,
        int OrderSum,
        string UserFirstName,
        string UserLastName,
        int UserId
    );
}
