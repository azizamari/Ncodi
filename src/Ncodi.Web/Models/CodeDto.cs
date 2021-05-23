using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ncodi.Web.Models
{
    public class CodeDto
    {
        public ICollection<string> Lines { get; set; }
        public string FileNmae { get; set; }
    }
}
