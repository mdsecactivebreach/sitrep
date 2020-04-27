using static SitRep.Enums.Enums;

namespace SitRep.Interfaces
{
    public interface ICheck
    {
        bool Enabled { get; }
        bool IsOpsecSafe { get;}
        int DisplayOrder { get; }
        void Check();
        
        CheckType CheckType { get; }
    }
}
