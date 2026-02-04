using Enteties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_s
{
    public record ProductDTO
    (
        int ProductsId,
        string ProductName,
        int Price,
        string Description,
        int CategoryId
    );
}
