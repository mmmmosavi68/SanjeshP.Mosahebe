using SanjeshP.Mosahebe.Common;

namespace Services.RDC.DataInitializer
{
    public interface IDataInitializer : IScopedDependency
    {
        void InitializeData();
    }
}
