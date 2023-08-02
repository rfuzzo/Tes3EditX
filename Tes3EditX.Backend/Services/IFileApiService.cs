using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes3EditX.Backend.Services;

public interface IFileApiService
{
    Task<string> PickAsync(CancellationToken none);
}
