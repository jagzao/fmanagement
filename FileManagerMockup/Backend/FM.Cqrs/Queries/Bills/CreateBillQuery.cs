﻿using FM.Core.Models;
using MediatR;

namespace FM.Cqrs.Queries
{
    public class CreateBillQuery : IRequest<ResponseDto>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
    }
}
