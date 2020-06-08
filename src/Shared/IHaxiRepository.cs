using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Shared
{
    public interface IHaxiRepository
    {
        Task<IList<Haxi>> Get(Guid[] guids, CancellationToken cancellationToken);
    }
}
