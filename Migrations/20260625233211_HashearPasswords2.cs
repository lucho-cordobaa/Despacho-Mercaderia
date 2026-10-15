using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DespachoApp.Migrations
{
    /// <inheritdoc />
    public partial class HashearPasswords2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2b$12$uakoOqR1DbXBfLlw0vtQJeYARtBmuY.rfpK9oGJSGKlRh/VdFu2.a");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$0zQKyKsro.bNznO5Pw5y3.oYvxnDJbXLksQ3K1Dr1xsCLUda1j9ru");
        }
    }
}
