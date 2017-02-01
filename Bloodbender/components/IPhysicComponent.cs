using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public interface IPhysicComponent
    {
       void Initialize(PhysicObj obj);
       void Update(float elapsed);
    }
}
