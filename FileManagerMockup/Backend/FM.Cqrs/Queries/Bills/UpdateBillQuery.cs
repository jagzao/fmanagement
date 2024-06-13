using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class UpdateBillQuery : IRequest<ResponseDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
    }
}
