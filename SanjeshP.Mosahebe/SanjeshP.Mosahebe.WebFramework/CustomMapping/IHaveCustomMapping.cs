using AutoMapper;
using SanjeshP.Mosahebe.Entities.User;

namespace WebFramework.CustomMapping
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile profile);
    }
}
