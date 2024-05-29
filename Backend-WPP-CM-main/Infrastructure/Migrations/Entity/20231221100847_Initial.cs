using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.Entity
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    licensePlate = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    seats = table.Column<int>(type: "int", nullable: false),
                    imageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    securityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    emailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    employeeNr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hasLicense = table.Column<bool>(type: "bit", nullable: false),
                    role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    departureDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    arrivalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    willReturn = table.Column<bool>(type: "bit", nullable: false),
                    seats = table.Column<int>(type: "int", nullable: false),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    carid = table.Column<int>(type: "int", nullable: false),
                    userid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.id);
                    table.ForeignKey(
                        name: "FK_Reservations_Cars_carid",
                        column: x => x.carid,
                        principalTable: "Cars",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_userid",
                        column: x => x.userid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    seats = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    creationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reservationid = table.Column<int>(type: "int", nullable: true),
                    userid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.id);
                    table.ForeignKey(
                        name: "FK_Requests_Reservations_reservationid",
                        column: x => x.reservationid,
                        principalTable: "Reservations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Requests_Users_userid",
                        column: x => x.userid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "id", "brand", "imageUrl", "isAvailable", "licensePlate", "location", "model", "seats" },
                values: new object[] { 1, "brand", "url", true, "999-XX-9", "location", "model", 5 });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_licensePlate",
                table: "Cars",
                column: "licensePlate",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_reservationid",
                table: "Requests",
                column: "reservationid");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_userid",
                table: "Requests",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_carid",
                table: "Reservations",
                column: "carid");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_userid",
                table: "Reservations",
                column: "userid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
