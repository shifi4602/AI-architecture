using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO_s
{
    public record ProductDTO
    (
        int ProductId,
        string ProductName,
        int Price,
        CategroryDTO Categrory, 
        string Description
    );
}
