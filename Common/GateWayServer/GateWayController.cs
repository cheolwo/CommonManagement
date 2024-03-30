using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.GateWayServer
{
    public class GateWayController<T> : ControllerBase where T : IRequest<bool>
    {
        public GateWayController()
        {

        }
    }
}
