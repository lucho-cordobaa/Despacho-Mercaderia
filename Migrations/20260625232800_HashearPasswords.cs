using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DespachoApp.Migrations
{
    /// <inheritdoc />
    public partial class HashearPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$0zQKyKsro.bNznO5Pw5y3.oYvxnDJbXLksQ3K1Dr1xsCLUda1j9ru");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "admin123");
        }
    }
}
