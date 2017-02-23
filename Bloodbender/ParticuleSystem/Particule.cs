using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloodbender
{
    public class Particule : GraphicObj
    {
        public Particule() : base(OffSet.BottomCenterHorizontal)
        {

        }

        public override bool Update(float elapsed)
        {


            return base.Update(elapsed);
        }
    }
}
