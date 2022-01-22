using JwwViewer.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JwwViewer
{
    class JwwToShapeConverter
    {
        public ICadShape Convert(JwwHelper.JwwData jd)
        {
            return jd switch
            {
                JwwHelper.JwwSen s => new JwwLineShape(s),
                JwwHelper.JwwEnko s => new JwwArcShape(s),
                JwwHelper.JwwSolid s => new JwwSolidShape(s),
                JwwHelper.JwwMoji s => new JwwTextShape(s),
                JwwHelper.JwwTen s => new JwwDotShape(s),
                JwwHelper.JwwBlock s => new JwwBlockShape(s),
                _ => null
            };
        }
    }
}
