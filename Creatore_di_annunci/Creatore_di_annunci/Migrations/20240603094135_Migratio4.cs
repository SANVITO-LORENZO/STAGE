﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Creatore_di_annunci.Migrations
{
    /// <inheritdoc />
    public partial class Migratio4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Descrizioni",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    _Descrizione = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Descrizioni", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "houses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Piani = table.Column<int>(type: "int", nullable: false),
                    Bagni = table.Column<int>(type: "int", nullable: false),
                    MQuadri = table.Column<int>(type: "int", nullable: false),
                    ascensore = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_houses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Videos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Videos", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Descrizioni");

            migrationBuilder.DropTable(
                name: "houses");

            migrationBuilder.DropTable(
                name: "Videos");
        }
    }
}