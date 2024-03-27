using JiraSynchronizer.Core.Entities;
using JiraSynchronizer.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Application.Controllers;

public class BaseController
{
    internal readonly SqlConnection Connection;
    public BaseController(SqlConnection connection)
    {
        Connection = connection;
    }
}
