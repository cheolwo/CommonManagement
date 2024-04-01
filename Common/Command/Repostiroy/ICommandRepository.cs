using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Command.Repostiroy
{
    public interface ICommandRepository<TCommand> where TCommand : class, IRequest<bool>
    {
        Task<bool> ExecuteAsync(TCommand command);
    }
}
