﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Classphy.Server.Entities;

public partial class Calificaciones
{
    public int idCalificacion { get; set; }

    public int idAsignatura { get; set; }

    public int idEstudiante { get; set; }

    public int MedioTermino { get; set; }

    public int? Final { get; set; }
}