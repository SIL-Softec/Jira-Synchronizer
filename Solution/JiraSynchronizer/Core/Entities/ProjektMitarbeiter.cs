﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiraSynchronizer.Core.Entities;

public class ProjektMitarbeiter : BaseEntity
{
    public int ProjektId { get; set; }
    public int MitarbeiterId { get; set; }
}
