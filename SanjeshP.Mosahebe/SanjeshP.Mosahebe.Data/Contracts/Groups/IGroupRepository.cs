using SanjeshP.Mosahebe.Data.Contracts.Common;
using SanjeshP.Mosahebe.Entities.Group;
using System.Collections.Generic;

namespace SanjeshP.Mosahebe.Data.Contracts.Groups
{
    public interface IGroupRepository : IEntityFrameworkRepository<Group>
    {
        IReadOnlyList<Group> GetAllGroups();
    }

}
