using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace FinalQuintenRader
{
    class Projectile
    {
        Vector3 pos;
        public Vector3 Pos
        {
            get { return pos; }
        }
        Vector3 dir;
        public Projectile(Vector3 p, Vector3 d)
        {
            pos = p;
            dir = d;
        }

        public void Update(GameTime gameTime)
        {
            pos += dir * 3.0f;
        }
    }
}
