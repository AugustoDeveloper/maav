
using System.Linq;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace MAAV.Infrastrucuture.Repository.MongoDB.Conventions
{
    public class SnakeCaseElementNameConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            string name = memberMap.MemberName;
            name = string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            memberMap.SetElementName(name);
        }
    }
}