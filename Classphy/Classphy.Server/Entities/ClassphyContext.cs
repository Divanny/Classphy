﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Classphy.Server.Entities;

public partial class ClassphyContext : DbContext
{
    public ClassphyContext(DbContextOptions<ClassphyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<LogActividades> LogActividades { get; set; }

    public virtual DbSet<LogErrores> LogErrores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LogActividades>(entity =>
        {
            entity.HasKey(e => e.idLogActividad);

            entity.Property(e => e.Data)
                .IsRequired()
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Metodo)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.URL)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        modelBuilder.Entity<LogErrores>(entity =>
        {
            entity.HasKey(e => e.idLogError);

            entity.Property(e => e.Fecha).HasColumnType("datetime");
            entity.Property(e => e.Fuente)
                .IsRequired()
                .HasMaxLength(500)
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.Mensaje)
                .IsRequired()
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            entity.Property(e => e.StackTrace)
                .IsRequired()
                .IsUnicode(false)
                .UseCollation("SQL_Latin1_General_CP1_CI_AS");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}