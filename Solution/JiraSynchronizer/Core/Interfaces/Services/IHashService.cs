using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Interfaces.Services;

public interface IHashService
{
    string Encode(long input);
    long Decode(string input);
}
