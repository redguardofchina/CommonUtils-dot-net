﻿// <auto-generated />
using System;
using CommonUtils.Test.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CommonUtils.Test.Web.Migrations
{
    [DbContext(typeof(DefaultDbContext))]
    [Migration("20190416070557_Create")]
    partial class Create
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("CommonUtils.Test.Web.Models.Code", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ConsumTime");

                    b.Property<bool>("Consumed");

                    b.Property<DateTime>("CreateTime");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("Codes");
                });
#pragma warning restore 612, 618
        }
    }
}
