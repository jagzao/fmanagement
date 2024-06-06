using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class UpdateBillCommand : IRequest<ResponseDto>
    {
        public int BillId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }
}
