using System.Collections;
using GymDB.API.Data.Settings.HelperClasses;

namespace GymDB.API.Data.Settings
{
    public class RoleSettings : IEnumerable<RoleDefinition>
    {
        public RoleDefinition SuperAdmin { get; set; }

        public RoleDefinition Admin { get; set; }
        
        public RoleDefinition Normie { get; set; }

        public IEnumerator<RoleDefinition> GetEnumerator()
        {
            yield return SuperAdmin;
            yield return Admin;
            yield return Normie;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
