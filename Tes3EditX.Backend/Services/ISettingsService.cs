using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Services;

public interface ISettingsService
{

    int MinConflicts { get; set; }
    bool CullConflicts { get; set; }
    string Theme { get; set; }

   
}
